using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Monexa.DET.MonexaAPI;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Monexa.DET
{
    public partial class MainForm : Form
    {
        #region Private Variables
        private MonexaWebservice_v05_40 ws;
        private Dictionary<string, string> wsCommands;
        private List<string> commandsToProcess;
        private Authentication auth;
        private BackgroundWorker bwAPI;
        private StringBuilder sbFile;

        private Dictionary<string, List<object>> dataSet;
        private List<object> filteredDataSet;

        private StringBuilder sbSummary;

        private string entityKey;
        private string requestKey;
        private int requestCounter;
        private int errorCounter;
        private int fileCounter;
        private bool pendingCancel;

        private Dictionary<string, object> allRequests;
        private Dictionary<string, object> allResponses;
        private Dictionary<string, object> allErrors;
        private List<object> allResults;

        private enum TaskStatus
        {
            InProgress,
            Success,
            Fail,
            WithErrors
        };

        #endregion

        public MainForm()
        {
            InitializeComponent();
            ConfigureLog4Net();
            BuildCommands();
            InitializeApp();
        }

        #region Public Properties
        public Dictionary<string, object> Requests
        {
            get => allRequests;
        }

        public Dictionary<string, object> Responses
        {
            get => allResponses;
        }

        public Dictionary<string, object> Errors
        {
            get => allErrors;
        }
        #endregion

        #region Init Functions
        // This function is for logging serialized requests and responses into a .log file
        private void ConfigureLog4Net()
        {
            try
            {
                File.Delete(Assembly.GetExecutingAssembly().Location + ".log");
                return;
                var appender = new FileAppender
                {
                    Layout = new SimpleLayout(),
                    File = Assembly.GetExecutingAssembly().Location + ".log",
                    Encoding = Encoding.UTF8,
                    AppendToFile = true,
                    LockingModel = new FileAppender.MinimalLock()
                };
                appender.ActivateOptions();
                BasicConfigurator.Configure(appender);
            }
            catch (Exception ex)
            {

            }
        }

        private void BuildCommands()
        {
            wsCommands = new Dictionary<string, string>
            {
                { "PingServer", "Web service ping" },                       // OK
                { "Accounts", "Extract accounts" },                         // OK
                { "AccountDetails", "Extract account details" },            // OK
                //{ "AccountPricing", "Extract account pricing" },            // <-- API cannot be found: SEARCH_PRICE_SHEET
                { "Subscribers", "Extract subscribers" },                   // OK
                //{ "SubAccounts", "Extract sub-accounts" },                  // <-- No parent_account_id parameter in SEARCH_SUBSCRIBER
                { "Invoices", "Extract invoices" },                         // OK
                { "InvoiceDetails", "Extract invoice details" },            // OK
                { "Transactions", "Extract transactions" },                 // OK
                { "Plans", "Extract plans" },                               // OK
                { "PlanDetails", "Extract plan details" },                  // OK
                { "PriceBooks", "Extract price books" },                    // OK
                //{ "PriceBookDetails", "Extract price book details" },       // OK
                //{ "PriceSheets", "Extract price sheets" },                  // <-- API cannot be found: SEARCH_PRICE_SHEET
                //{ "PriceSheetDetails", "Extract price sheet details" },     // <-- API cannot be found: LOOKUP_PRICE_SHEET
                //{ "ServiceBundles", "Extract service bundles" },            // <-- API cannot be found: SEARCH_SERVICE_BUNDLE
                //{ "RateModifications", "Extract rate modifications" },      // <-- API cannot be found: SEARCH_RATE_MODIFICATIONS
            };

            commandsToProcess = new List<string>();
            foreach(string key in wsCommands.Keys)
            {
                commandsToProcess.Add(key);
            }
        }

        private void InitializeApp()
        {
            if (ws == null)
                ws = new MonexaWebservice_v05_40();

            if (auth == null)
                auth = Authentication;

            allRequests = new Dictionary<string, object>();
            allResponses = new Dictionary<string, object>();
            allErrors = new Dictionary<string, object>();
            dataSet = new Dictionary<string, List<object>>();

            sbSummary = new StringBuilder();
            pendingCancel = false;
        }

        private void ResetCounters()
        {
            sbFile = new StringBuilder();
            filteredDataSet = new List<object>();
            requestCounter = 0;
            errorCounter = 0;
            fileCounter = 1;
            allResults = new List<object>();
            allErrors = new Dictionary<string, object>();
        }
        #endregion

        #region UI Functions
        private void RunNextCommand()
        {
            bwAPI = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            bwAPI.DoWork += bwAPI_DoWork;
            bwAPI.ProgressChanged += bwAPI_ProgressChanged;
            bwAPI.RunWorkerCompleted += bwAPI_RunWorkerCompleted;

            ListViewItem lvi = FindCurrentListViewItem();
            lvi.ImageIndex = (int)TaskStatus.InProgress;
            lvi.ForeColor = System.Drawing.Color.Black;

            bwAPI.RunWorkerAsync(commandsToProcess[0]);
        }

        private void ClearTaskList()
        {
            foreach(ListViewItem lvi in listTasks.Items)
            {
                if (wsCommands.ContainsValue(lvi.Text.Trim()))
                {
                    lvi.ForeColor = SystemColors.GrayText;
                    lvi.ImageIndex = -1;
                }
            }
        }

        private void ToggleControls(bool enabled)
        {
            btnExtract.Enabled = enabled;
            //btnCancel.Visible = !enabled;
        }

        private void UpdateProgress(string s)
        {
            ListViewItem lvi = FindCurrentListViewItem();
            lvi.SubItems[1].Text = s;
        }

        private ListViewItem FindCurrentListViewItem()
        {
            ListViewItem lvi = null;

            for (int i=0, n=listTasks.Items.Count; i<n; i++)
            {
                ListViewItem l = listTasks.Items[i];
                if (l.Text.IndexOf(wsCommands[commandsToProcess[0]]) >= 0)
                {
                    lvi = l;
                }
            }

            return lvi;
        }

        private void WriteException(Exception ex)
        {
            string msg = "Error for " + requestKey + " >> ";

            if (ex is SoapException)
            {
                msg += ex.Message + ": " + ((SoapException)ex).Detail.InnerText;
            }
            else
            {
                msg += ex.Message + ": " + ex.InnerException.ToString();
            }

            bwAPI.ReportProgress(0, msg);
        }

        private string WriteProgressText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Processed ").Append(requestCounter).Append(" of ")
                .Append(filteredDataSet.Count).Append(" requests");

            ListViewItem lvi = FindCurrentListViewItem();
            if ((requestCounter + errorCounter) >= filteredDataSet.Count)
            {
                if (errorCounter > 0)
                {
                    lvi.ImageIndex = (int)TaskStatus.WithErrors;
                }
                else
                {
                    lvi.ImageIndex = (int)TaskStatus.Success;
                }
            }

            return sb.ToString();
        }

        private void ResetForm()
        {
            ToggleControls(true);
            BuildCommands();
            pendingCancel = false;
        }
        #endregion

        #region Event Handlers
        private void btnExtract_Click(object sender, EventArgs e)
        {
            txtSummary.Clear();

            ClearTaskList();

            ToggleControls(false);
            RunNextCommand();
        }

        private void bwAPI_DoWork(object sender, DoWorkEventArgs e)
        {
            if (bwAPI.CancellationPending || pendingCancel == true)
            {
                e.Cancel = true;
            }

            switch (e.Argument.ToString())
            {
                case "PingServer":
                    PingServer();
                    break;
                case "Accounts":
                    GetAllAccounts();
                    break;
                case "AccountDetails":
                    GetAccountDetails();
                    break;
                case "AccountPricing":
                    //GetAccountPricing();
                    break;
                case "Subscribers":
                    GetSubscribers();
                    break;
                case "SubAccounts":
                    //GetSubAccounts();
                    break;
                case "Invoices":
                    GetInvoices();
                    break;
                case "InvoiceDetails":
                    GetInvoiceDetails();
                    break;
                case "Transactions":
                    GetTransactions();
                    break;
                case "Plans":
                    GetPlans();
                    break;
                case "PlanDetails":
                    GetPlanDetails();
                    break;
                case "PriceBooks":
                    GetPriceBooks();
                    break;
                case "PriceBookDetails":
                    GetPriceBookDetails();
                    break;
                case "PriceSheets":
                    //GetPriceSheets();
                    break;
                case "PriceSheetDetails":
                    //GetPriceSheetDetails();
                    break;
                case "ServiceBundles":
                    //GetServiceBundles();
                    break;
                case "Rate Modifications":
                    //GetRateModifications();
                    break;
                default:
                    break;
            }
        }

        private void bwAPI_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                allErrors.Add(requestKey, e.UserState);
                errorCounter++;
            }
            else
            {
                if (commandsToProcess[0] == "PingServer")
                {
                    UpdateProgress("Web service is up!");
                }
                else
                {
                    requestCounter++;

                    if (e != null)
                        WriteResults(e.UserState);
                }
            }
        }

        private void bwAPI_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ListViewItem lvi;

            if (pendingCancel == true || e.Cancelled == true)
            {
                lvi = FindCurrentListViewItem();
                if (lvi != null)
                {
                    lvi.ImageIndex = (int)TaskStatus.Fail;
                    lvi.SubItems[1].Text = "Operation cancelled by user";
                }

                commandsToProcess.Clear();
                ResetForm();

                return;
            }

            if (commandsToProcess[0] != "PingServer")
            {

                WriteToFile();
                WriteErrorFile();

                StringBuilder sb = new StringBuilder()
                    .Append("Received ").Append(allResults.Count)
                    .Append(entityKey).Append(allResults.Count != 1 ? "s" : "");

                if (allErrors.Count > 0)
                {
                    sb.Append(" with ").Append(allErrors.Count)
                        .Append(" error").Append(allErrors.Count != 1 ? "s" : "");
                }

                sbSummary.Append(sb.ToString()).Append(Environment.NewLine);

                UpdateProgress(sb.ToString());
            }

            lvi = FindCurrentListViewItem();
            if (allErrors.Count > 0)
            {
                lvi.ImageIndex = (int)TaskStatus.WithErrors;
            }
            else
            {
                lvi.ImageIndex = (int)TaskStatus.Success;
            }

            commandsToProcess.RemoveAt(0);

            if (commandsToProcess.Count > 0) {
                RunNextCommand();
            }
            else
            {
                sbSummary.Append("Total web service requests sent: ")
                    .Append(allRequests.Count).Append(Environment.NewLine)
                    .Append("Success: ").Append(allResponses.Count).Append(Environment.NewLine)
                    .Append("Failed: ").Append(allErrors.Count)
                    .Append(Environment.NewLine);

                txtSummary.Text = sbSummary.ToString();

                ResetForm();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bwAPI.CancelAsync();
            pendingCancel = true;
        }
        #endregion

        #region Web Service Functions
        private Authentication Authentication
        {
            get
            {
                return new Authentication
                {
                    ItemElementName = ItemChoiceType.administrator_login_name,
                    Item = "ssp_admin",
                    password = "Qwert12"
                };
            }
        }

        // OK
        private void PingServer()
        {
            try
            {
                ws.pingServer(auth);
                requestKey = commandsToProcess[0];

                bwAPI.ReportProgress(100, "Web service is up!");
            }
            catch(Exception ex)
            {
                WriteException(ex);
            }
        }

        // OK
        private void GetAllAccounts()
        {
            ResetCounters();

            allResults = new List<object>();
            entityKey = " account";

            try
            {
                SearchSubscriberRequest req = new SearchSubscriberRequest();
                req.Authentication = auth;
                req.SEARCH_SUBSCRIBER = new SEARCH_SUBSCRIBER();
                req.SEARCH_SUBSCRIBER.account_type = account_type_enum.all;
                req.SEARCH_SUBSCRIBER.account_typeSpecified = true;

                requestKey = commandsToProcess[0];
                allRequests.Add(requestKey, req);

                Response_SEARCH_SUBSCRIBER res = ws.searchSubscriber(req);
                allResponses.Add(requestKey, res);

                bwAPI.ReportProgress(100, res);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        // OK
        private void GetAccountDetails()
        {
            ResetCounters();
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                .Distinct()
                .ToList();

            allResults = new List<object>();
            entityKey = " account detail";

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    if (allRequests.ContainsKey(sub.Account_Information.account_id.ToString()) == false)
                    {
                        LookupSubscriberRequest req = new LookupSubscriberRequest();
                        req.Authentication = auth;
                        req.LOOKUP_SUBSCRIBER = new LOOKUP_SUBSCRIBER();
                        req.LOOKUP_SUBSCRIBER.ItemElementName = ItemChoiceType1.account_id;
                        req.LOOKUP_SUBSCRIBER.Item = sub.Account_Information.account_id.ToString();

                        requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id.ToString();
                        allRequests.Add(requestKey, req);

                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[] res = ws.lookupSubscriber(req);
                        allResponses.Add(requestKey, res);

                        bwAPI.ReportProgress(100, res);
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }
        }

        // INACTIVE - SEARCH_PRICE_SHEET API cannot be found
        private void GetAccountPricing()
        {
            ResetCounters();

            allResults = new List<object>();

        }

        // OK
        private void GetSubscribers()
        {
            ResetCounters();
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Contact_Information.master_account_id) == false)
                .ToList();

            allResults = new List<object>();
            entityKey = " subscriber";

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    if (allRequests.ContainsKey(sub.Account_Information.account_id.ToString()) == false)
                    {
                        SearchSubscriberRequest req = new SearchSubscriberRequest();
                        req.Authentication = auth;
                        req.SEARCH_SUBSCRIBER = new SEARCH_SUBSCRIBER();
                        req.SEARCH_SUBSCRIBER.account_id = sub.Account_Information.account_id;

                        requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id;

                        Response_SEARCH_SUBSCRIBER res = null;
                        if (allRequests.ContainsKey(requestKey) == false)
                        {
                            allRequests.Add(requestKey, req);

                            res = ws.searchSubscriber(req);
                            allResponses.Add(requestKey, res);
                        }

                        bwAPI.ReportProgress(100, res);
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }
        }

        // INACTIVE - No parent_account_id parameter in SEARCH_SUBSCRIBER
        private void GetSubAccounts()
        {
            ResetCounters();
            
            allResults = new List<object>();

        }

        // OK
        private void GetInvoices()
        {
            ResetCounters();
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                .ToList();

            allResults = new List<object>();
            entityKey = " invoice";

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    SearchInvoiceRequest req = new SearchInvoiceRequest();
                    req.Authentication = auth;
                    req.SEARCH_INVOICE = new SEARCH_INVOICE();
                    req.SEARCH_INVOICE.information_type = information_type_enum.full;
                    req.SEARCH_INVOICE.ItemElementName = ItemChoiceType2.account_id;
                    req.SEARCH_INVOICE.Item = sub.Account_Information.account_id;

                    requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id;
                    allRequests.Add(requestKey, req);

                    Invoice_Information[] res = ws.searchInvoice(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }
        }
        
        // OK
        private void GetInvoiceDetails()
        {
            ResetCounters();
            filteredDataSet = dataSet["INVOICES"];

            allResults = new List<object>();
            entityKey = " invoice detail";

            foreach (Invoice_Information inv in dataSet["INVOICES"])
            {
                try
                {
                    LookupInvoiceRequest req = new LookupInvoiceRequest();
                    req.Authentication = auth;
                    req.LOOKUP_INVOICE = new LOOKUP_INVOICE();
                    req.LOOKUP_INVOICE.invoice_number = inv.Invoice.invoice_number;
                    req.LOOKUP_INVOICE.information_type = information_type_enum.full;
                    req.LOOKUP_INVOICE.Item = inv.Invoice.Subscriber_Information.Account_Information.account_id;

                    requestKey = commandsToProcess[0] + "_" + inv.Invoice.invoice_number;
                    allRequests.Add(requestKey, req);

                    Response_LOOKUP_INVOICE res = ws.lookupInvoice(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch(Exception ex)
                {
                    WriteException(ex);
                }
            }
        }
        
        // OK
        private void GetTransactions()
        {
            ResetCounters();
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                .ToList();

            allResults = new List<object>();
            entityKey = " transaction";

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    SearchTransactionRequest req = new SearchTransactionRequest();
                    req.Authentication = auth;
                    req.SEARCH_TRANSACTION = new SEARCH_TRANSACTION();
                    req.SEARCH_TRANSACTION.ItemElementName = ItemChoiceType3.account_id;
                    req.SEARCH_TRANSACTION.Item = sub.Account_Information.account_id;

                    requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id;
                    allRequests.Add(requestKey, req);

                    Transaction[] res = ws.searchTransaction(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch(Exception ex)
                {
                    WriteException(ex);
                }
            }
        }
        
        // OK
        private void GetPlans()
        {
            ResetCounters();

            allResults = new List<object>();

            try
            {
                SearchPlanRequest req = new SearchPlanRequest();
                req.Authentication = auth;
                req.SEARCH_PLAN = new SEARCH_PLAN();
                req.SEARCH_PLAN.provider_name = "*";

                entityKey = " plan";
                requestKey = commandsToProcess[0];
                allRequests.Add(requestKey, req);

                Response_SEARCH_PLAN res = ws.searchPlan(req);
                allResponses.Add(requestKey, res);

                bwAPI.ReportProgress(100, res);
            }
            catch(Exception ex)
            {
                WriteException(ex);
            }
        }
        
        // OK
        private void GetPlanDetails()
        {
            ResetCounters();
            filteredDataSet = dataSet["PLANS"]
                .ToList();

            allResults = new List<object>();
            entityKey = " plan detail";

            foreach(REP_SEARCH_PLAN_PLAN_TYPE plan in filteredDataSet)
            {
                try
                {
                    LookupPlanRequest req = new LookupPlanRequest();
                    req.Authentication = auth;
                    req.LOOKUP_PLAN = new LOOKUP_PLAN();
                    req.LOOKUP_PLAN.provider_id = plan.provider_id;
                    req.LOOKUP_PLAN.plan_id = plan.plan_id;

                    requestKey = commandsToProcess[0] + "_" + plan.plan_id;
                    allRequests.Add(requestKey, req);

                    Response_LOOKUP_PLAN res = ws.lookupPlan(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }
        }
        
        // OK
        private void GetPriceBooks()
        {
            ResetCounters();

            allResults = new List<object>();
            entityKey = " price book";

            foreach(Response_LOOKUP_PLAN plan in dataSet["PLANDETAILS"])
            {
                try
                {
                    Price_Book[] books = null;

                    if (plan.Plan.Price_Book != null)
                    {
                        books = plan.Plan.Price_Book;
                        foreach (Price_Book book in plan.Plan.Price_Book)
                        {
                            requestKey = commandsToProcess[0] + "_" + book.internal_price_book_id;
                            allResponses.Add(requestKey, book);
                        }
                    }

                    bwAPI.ReportProgress(100, books);
                }
                catch(Exception ex)
                {
                    WriteException(ex);
                }
            }
        }

        // OK
        private void GetPriceBookDetails()
        {
            ResetCounters();
            filteredDataSet = dataSet["PRICEBOOK"]
                .ToList();

            allResults = new List<object>();
            entityKey = " price book detail";

            foreach(Price_Book book in filteredDataSet)
            {
                try
                {
                    LookupPriceBookRequest req = new LookupPriceBookRequest();
                    req.Authentication = auth;
                    req.LOOKUP_PRICE_BOOK = new LOOKUP_PRICE_BOOK();
                    req.LOOKUP_PRICE_BOOK.Item = book.internal_price_book_id;

                    requestKey = commandsToProcess[0] + "_" + book.internal_price_book_id;
                    allRequests.Add(requestKey, req);

                    Response_LOOKUP_PRICE_BOOK res = ws.lookupPriceBook(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch(Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }

        // INACTIVE - SEARCH_PRICE_SHEET API cannot be found
        private void GetPriceSheets()
        {
            ResetCounters();

            allResults = new List<object>();
            entityKey = " price sheet";

        }

        // INACTIVE - LOOKUP_PRICE_SHEET API cannot be found
        private void GetPriceSheetDetails()
        {
            ResetCounters();

            allResults = new List<object>();
            entityKey = " price sheet detail";

        }

        // INACTIVE - SEARCH_SERVICE_BUNDLE API cannot be found
        private void GetServiceBundles()
        {
            ResetCounters();

            allResults = new List<object>();
            entityKey = " service bundle";

        }
        
        // INACTIVE - SEARCH_RATE_MODIFICATIONS API cannot be found
        private void GetRateMOdifications()
        {
            ResetCounters();

            allResults = new List<object>();
            entityKey = " rate modification";

        }
        #endregion

        #region File I/O Functions
        private void WriteToFile()
        {
            if (cbWriteToFile.Checked == false)
                return;

            string fileName;
            int pageSize = int.Parse(numPageSize.Value.ToString());

            sbFile = new StringBuilder();
            
            for(int i=0, n=allResults.Count; i<n; i++)
            {
                sbFile.Append(JsonConvert.SerializeObject(allResults[i])).Append(Environment.NewLine);

                if (((i+1) % pageSize == 0 && i > 0) || (i == (allResults.Count - 1)))
                {
                    fileName = Directory.GetCurrentDirectory() + "\\" + commandsToProcess[0] + "_" +
                        fileCounter.ToString() + ".txt";

                    File.WriteAllText(fileName,
                        sbFile.Length > 0 ? sbFile.ToString() : "No results found");

                    fileCounter++;
                    sbFile = new StringBuilder();
                }
            }
        }

        private void WriteErrorFile()
        {
            if (cbWriteToFile.Checked == false)
                return;

            string fileName;
            int pageSize = int.Parse(numPageSize.Value.ToString());
            int i = 0, n = allErrors.Count;

            sbFile = new StringBuilder();

            foreach(string key in allErrors.Keys)
            {
                sbFile.Append(JsonConvert.SerializeObject(allErrors[key])).Append(Environment.NewLine);

                if (((i+1) % pageSize == 0 && i > 0) || (i == allErrors.Count - 1))
                {
                    fileName = Directory.GetCurrentDirectory() + "\\" + commandsToProcess[0] + "_errors.txt";

                    File.WriteAllText(fileName,
                        sbFile.Length > 0 ? sbFile.ToString() : "No errors found");

                    sbFile = new StringBuilder();
                }

                i++;
            }
        }

        private void WriteResults (object obj)
        {
            switch (commandsToProcess[0])
            {
                case "Accounts":
                    {
                        InitDataSet("ACCOUNTS");

                        Response_SEARCH_SUBSCRIBER res = (Response_SEARCH_SUBSCRIBER)obj;

                        for (int i = 0, n = res.Subscriber_Information.Length; i < n; i++)
                        {
                            REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE x = res.Subscriber_Information[i];
                            allResults.Add(x);
                            dataSet["ACCOUNTS"].Add(x);
                        }

                        UpdateProgress("Received " + res.result_size + " accounts");
                    }
                    break;
                case "AccountDetails":
                    {
                        InitDataSet("ACCOUNT_DETAILS");

                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[] res = (REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[])obj;

                        allResults.Add(res[0]);
                        dataSet["ACCOUNT_DETAILS"].Add(res[0]);

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "AccountPricing":
                    {
                        // INACTIVE - API cannot be found: SEARCH_PRICE_SHEET
                    }
                    break;
                case "Subscribers":
                    {
                        InitDataSet("SUBSCRIBERS");

                        Response_SEARCH_SUBSCRIBER res = (Response_SEARCH_SUBSCRIBER)obj;
                        for (int i = 0, n = res.Subscriber_Information.Length; i < n; i++)
                        {
                            REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE x = res.Subscriber_Information[i];
                            allResults.Add(x);
                            dataSet["SUBSCRIBERS"].Add(x);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "SubAccounts":
                    {
                        // INACTIVE - No parent_account_id parameter in SEARCH_SUBSCRIBER
                    }
                    break;
                case "Invoices":
                    {
                        InitDataSet("INVOICES");

                        Invoice_Information[] res = (Invoice_Information[])obj;

                        for (int i=0, n=res.Length; i<n; i++)
                        {
                            allResults.Add(res[i]);
                            dataSet["INVOICES"].Add(res[i]);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "InvoiceDetails":
                    {
                        InitDataSet("INVOICE_DETAILS");

                        Response_LOOKUP_INVOICE res = (Response_LOOKUP_INVOICE)obj;
                        allResults.Add(res);
                        dataSet["INVOICE_DETAILS"].Add(res);

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "Transactions":
                    {
                        InitDataSet("TRANSACTIONS");

                        Transaction[] res = (Transaction[])obj;
                        
                        for (int i=0, n=res.Length; i<n; i++)
                        {
                            allResults.Add(res[i]);
                            dataSet["TRANSACTIONS"].Add(res[i]);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "Plans":
                    {
                        InitDataSet("PLANS");

                        Response_SEARCH_PLAN res = (Response_SEARCH_PLAN)obj;

                        for(int i = 0, n = res.Plan.Length; i < n; i++)
                        {
                            allResults.Add(res.Plan[i]);
                            dataSet["PLANS"].Add(res.Plan[i]);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "PlanDetails":
                    {
                        InitDataSet("PLANDETAILS");

                        Response_LOOKUP_PLAN res = (Response_LOOKUP_PLAN)obj;
                        allResults.Add(res);
                        dataSet["PLANDETAILS"].Add(res);

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "PriceBooks":
                    {
                        InitDataSet("PRICEBOOKS");

                        if (obj != null)
                        {
                            Price_Book res = (Price_Book)obj;
                            allResults.Add(res);
                            dataSet["PRICEBOOKS"].Add(res);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "PriceBookDetails":
                    {
                        InitDataSet("PRICEBOOKDETAILS");

                        if (obj!= null)
                        {
                            Price_Book res = (Price_Book)obj;
                            allResults.Add(res);
                            dataSet["PRICEBOOKDETAILS"].Add(res);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "PriceSheets":
                    {
                        // INACTIVE - API cannot be found: SEARCH_PRICE_SHEET
                    }
                    break;
                case "PriceSheetDetails":
                    {
                        // INACTIVE - API cannot be found: SEARCH_PRICE_SHEET
                    }
                    break;
                case "ServiceBundles":
                    {
                        // INACTIVE - API cannot be found: SEARCH_SERVICE_BUNDLE
                    }
                    break;
                case "RateModifications":
                    {
                        // INACTIVE - API cannot be found: SEARCH_RATE_MODIFICATIONS
                    }
                    break;
                default:
                    break;
            }
        }

        private void InitDataSet(string key)
        {
            if (dataSet.ContainsKey(key) == false)
                dataSet[key] = new List<object>();
        }
        #endregion
    }
}
