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
using Monexa.DET.MonexaAPI;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Monexa.DET
{
    public partial class Form1 : Form
    {
        //private string wsdl = @"https://api.monexa.com/v05_42-beta/SOAP/monexa_webservice.wsdl";
        string Monexa_URL = @"https://api.monexa.com/v05_40/XML/monexa_xml.cgi"; // Only for testing...
        private MonexaWebservice_v05_40 ws;
        private Dictionary<string, string> wsCommands;
        private List<string> commandsToProcess;
        private Authentication auth;
        private BackgroundWorker bwAPI;
        private StringBuilder sbFile;

        private Dictionary<string, List<object>> dataSet;
        private List<object> filteredDataSet;

        private string entityKey;
        private string requestKey;
        private int requestCounter;
        private int errorCounter;
        private int fileCounter;
        private int PAGE_SIZE = 10;

        private Dictionary<string, object> allRequests;
        private Dictionary<string, object> allResponses;
        private Dictionary<string, object> allErrors;
        private List<object> allResults;

        private StringBuilder sbSummary;

        private enum TaskStatus
        {
            InProgress,
            Success,
            Fail,
            WithErrors
        };
        
        public Form1()
        {
            InitializeComponent();
            ConfigureLog4Net();
            BuildCommands();
            InitializeApp();
        }

        #region Properties
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
        private void ConfigureLog4Net()
        {
            File.Delete(Assembly.GetExecutingAssembly().Location + ".log");

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

        private void BuildCommands()
        {
            wsCommands = new Dictionary<string, string>
            {
                { "PingServer", "Web service ping" },                       // OK
                //{ "AllAccounts", "Extract accounts" },                      // OK
                //{ "AccountDetails", "Extract account details" },            // OK
                //{ "AccountPricing", "Extract account pricing" },          <-- API is missing!
                //{ "Subscribers", "Extract subscribers" },                   // OK
                //{ "SubAccounts", "Extract sub-accounts" },                <-- No parent_account_id parameter in SEARCH_SUBSCRIBER
                //{ "Invoices", "Extract invoices" },                         // OK
                //{ "InvoiceDetails", "Extract invoice details" },
                //{ "Transactions", "Extract transactions" },                 // OK
                { "Plans", "Extract plans" },
                //{ "PlanDetails", "Extract plan details" },
                //{ "PriceBooks", "Extract price books" },
                //{ "PriceBookDetails", "Extract price book details" },
                //{ "PriceSheets", "Extract price sheets" },
                //{ "PriceSheetDetails", "Extract price sheet details" },
                //{ "ServiceBundles", "Extract service bundles" },
                //{ "RateModifications", "Extract rate modifications" },
            };

            commandsToProcess = new List<string>();
            foreach(string key in wsCommands.Keys)
            {
                commandsToProcess.Add(key);
            }

            cboCommands.Items.AddRange(commandsToProcess.ToArray());
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
            //if (commandsToProcess.Count > 0)
            //{
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
            //}
        }

        private void ToggleControls(bool enabled)
        {
            btnExtract.Enabled = enabled;
            btnCancel.Visible = !enabled;
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
        #endregion

        #region Event Handlers
        private void btnExtract_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            RunNextCommand();
            //SendSOAP(cbSend.Checked); // For SOAP testing only; DO NOT USE
        }

        private void bwAPI_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (e.Argument.ToString())
            {
                case "PingServer":
                    PingServer();
                    break;
                case "AllAccounts":
                    GetAllAccounts();
                    break;
                case "AccountDetails":
                    GetAccountDetails();
                    break;
                case "AccountPricing":
                    GetAccountPricing();
                    break;
                case "Subscribers":
                    GetSubscribers();
                    break;
                case "SubAccounts":
                    GetSubAccounts();
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
                    //GetPlanDetails();
                    break;
                case "PriceBooks":
                    //GetPriceBooks();
                    break;
                case "PriceBookDetails":
                    //GetPriceBookDetails();
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

                //WriteResults(e.UserState);
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
            if (commandsToProcess[0] != "PingServer")
            {
                WriteToFile();
                WriteErrorFile();

                StringBuilder sb = new StringBuilder()
                    .Append("Received ").Append(allResults.Count)
                    .Append(entityKey).Append(allResults.Count > 1 ? "s" : "");

                if (allErrors.Count > 0)
                {
                    sb.Append(" with ").Append(allErrors.Count)
                        .Append(" error").Append(allErrors.Count > 1 ? "s" : "");
                }

                UpdateProgress(sb.ToString());
            }

            ListViewItem lvi = FindCurrentListViewItem();
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
                txtSummary.Text = "Total web service requests sent: " +
                    allRequests.Count.ToString() + Environment.NewLine +
                    "Success: " + allResponses.Count.ToString() + Environment.NewLine +
                    "Failed: " + allErrors.Count.ToString() + Environment.NewLine;

                ToggleControls(true);
                BuildCommands();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bwAPI.CancelAsync();
        }

        private void cboCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCommand.Text = ((ComboBox)sender).SelectedItem.ToString();
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
                bwAPI_old.ReportProgress(0, ex);
            }
        }

        // OK
        private void GetAllAccounts()
        {
            ResetCounters();

            allResults = new List<object>();

            try
            {
                SearchSubscriberRequest req = new SearchSubscriberRequest();
                req.Authentication = auth;
                req.SEARCH_SUBSCRIBER = new SEARCH_SUBSCRIBER();
                req.SEARCH_SUBSCRIBER.account_type = account_type_enum.all;
                req.SEARCH_SUBSCRIBER.account_typeSpecified = true;

                entityKey = " account";
                requestKey = commandsToProcess[0];
                allRequests.Add(requestKey, req);

                Response_SEARCH_SUBSCRIBER res = ws.searchSubscriber(req);
                allResponses.Add(requestKey, res);

                bwAPI.ReportProgress(100, res);
            }
            catch (Exception ex)
            {
                bwAPI.ReportProgress(0, ex);
            }
        }

        // OK
        private void GetAccountDetails()
        {
            ResetCounters();
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                //.Select(sub => ((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id)
                .Distinct()
                .ToList();

            allResults = new List<object>();

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    //if (sub.Account_Information.account_id != null)
                    if (allRequests.ContainsKey(sub.Account_Information.account_id.ToString()) == false)
                    {
                        LookupSubscriberRequest req = new LookupSubscriberRequest();
                        req.Authentication = auth;
                        req.LOOKUP_SUBSCRIBER = new LOOKUP_SUBSCRIBER();
                        req.LOOKUP_SUBSCRIBER.ItemElementName = ItemChoiceType1.account_id;
                        req.LOOKUP_SUBSCRIBER.Item = sub.Account_Information.account_id.ToString();

                        entityKey = " account detail";
                        requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id.ToString();
                        allRequests.Add(requestKey, req);

                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[] res = ws.lookupSubscriber(req);
                        allResponses.Add(requestKey, res);

                        bwAPI.ReportProgress(100, res);
                    }
                }
                catch (Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }

        // API IS MISSING: SEARCH_PRICE_SHEET
        private void GetAccountPricing()
        {
            ResetCounters();

            allResults = new List<object>();

        }

        // OK
        private void GetSubscribers()
        {
            ResetCounters();
            //filteredDataSet = allSubscribers
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Contact_Information.master_account_id) == false)
                //.Select(sub => ((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Contact_Information.master_account_id)
                //.Distinct()
                .ToList();

            allResults = new List<object>();

            //foreach(string s in filteredDataSet)
            foreach(REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    if (allRequests.ContainsKey(sub.Account_Information.account_id.ToString()) == false)
                    {
                        SearchSubscriberRequest req = new SearchSubscriberRequest();
                        req.Authentication = auth;
                        req.SEARCH_SUBSCRIBER = new SEARCH_SUBSCRIBER();
                        req.SEARCH_SUBSCRIBER.account_id = sub.Account_Information.account_id;

                        entityKey = " subscriber";
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
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }

        // TO BE IMPLEMENTED
        private void GetSubAccounts()
        {
            ResetCounters();
            
            //filteredDataSet = allSubscribers
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                //.Select(sub => ((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id)
                .Distinct()
                .ToList();

            //foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in allSubscribers)
            foreach(string s in filteredDataSet)
            {
                try
                {
                    SearchSubscriberRequest req = new SearchSubscriberRequest();
                    req.Authentication = auth;
                    
                    SEARCH_SUBSCRIBER search = new SEARCH_SUBSCRIBER();
                }
                catch (Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }

        // OK
        private void GetInvoices()
        {
            ResetCounters();
            //filteredDataSet = allSubscribers
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                //.Select(inv => ((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)inv).Account_Information.account_id)
                .ToList();

            allResults = new List<object>();

            //foreach(string s in filteredDataSet)
            foreach(REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    SearchInvoiceRequest req = new SearchInvoiceRequest();
                    req.Authentication = auth;
                    req.SEARCH_INVOICE = new SEARCH_INVOICE();
                    req.SEARCH_INVOICE.information_type = information_type_enum.full;
                    req.SEARCH_INVOICE.ItemElementName = ItemChoiceType2.account_id;
                    //req.SEARCH_INVOICE.Item = s;
                    req.SEARCH_INVOICE.Item = sub.Account_Information.account_id;

                    //entityKey = sub.Account_Information.account_id;
                    entityKey = " invoice";
                    requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id;
                    //requestKey = commandsToProcess[0] + "_" + s.ToString();
                    allRequests.Add(requestKey, req);

                    Invoice_Information[] res = ws.searchInvoice(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                    //}
                }
                catch (Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }
        
        // OK
        private void GetInvoiceDetails()
        {
            ResetCounters();
            filteredDataSet = dataSet["INVOICES"];
            /*filteredDataSet = dataSet["INVOICES"]
                .Where(sub => String.IsNullOrEmpty(((Invoice_Information)sub).Account_Information.account_id) == false)
                .Select(sub => sub.Account_Information.account_id)
                .Distinct()
                .ToList();*/

            foreach(Invoice_Information inv in dataSet["INVOICES"])
            {
                try
                {
                    LookupInvoiceRequest req = new LookupInvoiceRequest();
                    req.Authentication = auth;
                    req.LOOKUP_INVOICE = new LOOKUP_INVOICE();
                    req.LOOKUP_INVOICE.invoice_number = inv.Invoice.invoice_number;
                    req.LOOKUP_INVOICE.information_type = information_type_enum.full;
                    req.LOOKUP_INVOICE.Item = inv.Invoice.Subscriber_Information.Account_Information.account_id;

                    entityKey = " invoice detail";
                    requestKey = commandsToProcess[0] + "_" + inv.Invoice.invoice_number;
                    allRequests.Add(requestKey, req);

                    Response_LOOKUP_INVOICE res = ws.lookupInvoice(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch(Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }
        
        // OK
        private void GetTransactions()
        {
            ResetCounters();
            filteredDataSet = dataSet["ACCOUNTS"]
                .Where(sub => String.IsNullOrEmpty(((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)sub).Account_Information.account_id) == false)
                //.Select(inv => ((REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE)inv).Account_Information.account_id)
                .ToList();

            allResults = new List<object>();

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in filteredDataSet)
            {
                try
                {
                    SearchTransactionRequest req = new SearchTransactionRequest();
                    req.Authentication = auth;
                    req.SEARCH_TRANSACTION = new SEARCH_TRANSACTION();
                    req.SEARCH_TRANSACTION.ItemElementName = ItemChoiceType3.account_id;
                    req.SEARCH_TRANSACTION.Item = sub.Account_Information.account_id;

                    entityKey = " transaction";
                    requestKey = commandsToProcess[0] + "_" + sub.Account_Information.account_id;
                    allRequests.Add(requestKey, req);

                    Transaction[] res = ws.searchTransaction(req);
                    allResponses.Add(requestKey, res);

                    bwAPI.ReportProgress(100, res);
                }
                catch(Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }
        
        // WIP
        private void GetPlans()
        {
            ResetCounters();

            allResults = new List<object>();

            try
            {
                /*ListPlansRequest req = new ListPlansRequest();
                req.LIST_PLANS = new LIST_PLANS();
                req.LIST_PLANS.information_type = information_type_enum.full;

                req.LIST_PLANS.LIST_PRICE_BOOKS = new LIST_PRICE_BOOKS();
                req.LIST_PLANS.LIST_PRICE_BOOKS.information_type = information_type_enum.full;*/

                SearchPlanRequest req = new SearchPlanRequest();
                req.Authentication = auth;
                req.SEARCH_PLAN = new SEARCH_PLAN();
                req.SEARCH_PLAN.provider_name = "*";

                entityKey = " plan";
                requestKey = commandsToProcess[0];
                allRequests.Add(requestKey, req);

                Response_SEARCH_PLAN res = ws.searchPlan(req);
                allResponses.Add(requestKey, res);

                //REP_LIST_PLANS_PLAN_TYPE[] res = ws.listPlans(req);
                //allResponses.Add(requestKey, res);

                bwAPI.ReportProgress(100, res);
            }
            catch(Exception ex)
            {
                bwAPI.ReportProgress(0, ex);
            }
        }
        #endregion

        #region File I/O Functions
        private void WriteToFile()
        {
            string fileName;

            sbFile = new StringBuilder();

            for(int i=0, n=allResults.Count; i<n; i++)
            {
                sbFile.Append(JsonConvert.SerializeObject(allResults[i])).Append(Environment.NewLine);

                if (
                    (i % PAGE_SIZE == 0 && i > 0) || (i == (allResults.Count - 1))
                    )
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
            string fileName;
            int i = 0, n = allErrors.Count;

            sbFile = new StringBuilder();

            foreach(string key in allErrors.Keys)
            {
                sbFile.Append(JsonConvert.SerializeObject(allErrors[key])).Append(Environment.NewLine);

                if ((i % PAGE_SIZE == 0 && i > 0) ||
                    (i == allErrors.Count - 1))
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
                case "AllAccounts":
                    {
                        if (dataSet.ContainsKey("ACCOUNTS") == false)
                            dataSet["ACCOUNTS"] = new List<object>();

                        Response_SEARCH_SUBSCRIBER res = (Response_SEARCH_SUBSCRIBER)obj;

                        for (int i = 0, n = res.Subscriber_Information.Length; i < n; i++)
                        {
                            REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE x = res.Subscriber_Information[i];
                            allResults.Add(x);
                            dataSet["ACCOUNTS"].Add(x);
                            //allSubscribers.Add(x);
                        }

                        UpdateProgress("Received " + res.result_size + " accounts");
                    }
                    break;
                case "AccountDetails":
                    {
                        if (dataSet.ContainsKey("ACCOUNT_DETAILS") == false)
                            dataSet["ACCOUNT_DETAILS"] = new List<object>();

                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[] res = (REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[])obj;

                        //if (cbWriteToFile.Checked == true && requestCounter % PAGE_SIZE == 0 && requestCounter > 0)
                        //{
                        //    WriteToFile();
                        //}

                        //REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE x = res[0];
                        allResults.Add(res[0]);
                        dataSet["ACCOUNT_DETAILS"].Add(res[0]);
                        //sbFile.Append(JsonConvert.SerializeObject(x)).Append(Environment.NewLine);

                        //if (CanWriteToFile() == true)
                        //{
                        //    WriteToFile();
                        //}

                        //requestCounter++;
                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "Subscribers":
                    {
                        if (dataSet.ContainsKey("SUBSCRIBERS") == false)
                            dataSet["SUBSCRIBERS"] = new List<object>();

                        Response_SEARCH_SUBSCRIBER res = (Response_SEARCH_SUBSCRIBER)obj;
                        for (int i = 0, n = res.Subscriber_Information.Length; i < n; i++)
                        {
                            //if (cbWriteToFile.Checked == true && (i % PAGE_SIZE == 0 && i > 0))
                            //{
                            //    WriteToFile();
                            //}

                            REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE x = res.Subscriber_Information[i];
                            allResults.Add(x);
                            dataSet["SUBSCRIBERS"].Add(x);
                            //sbFile.Append(JsonConvert.SerializeObject(x)).Append(Environment.NewLine);
                        }

                        //requestCounter++;
                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "Invoices":
                    {
                        if (dataSet.ContainsKey("INVOICES") == false)
                            dataSet["INVOICES"] = new List<object>();

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
                        if (dataSet.ContainsKey("INVOICE_DETAILS") == false)
                            dataSet["INVOICE_DETAILS"] = new List<object>();

                        Response_LOOKUP_INVOICE res = (Response_LOOKUP_INVOICE)obj;
                        allResults.Add(res);
                        dataSet["INVOICE_DETAILS"].Add(res);

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "Transactions":
                    {
                        if (dataSet.ContainsKey("TRANSACTIONS") == false)
                            dataSet["TRANSACTIONS"] = new List<object>();

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
                        if (dataSet.ContainsKey("PLANS") == false)
                            dataSet["PLANS"] = new List<object>();

                        /*REP_LIST_PLANS_PLAN_TYPE[] res = (REP_LIST_PLANS_PLAN_TYPE[])obj;

                        for (int i = 0, n = res.Length; i < n; i++)*/

                        Response_SEARCH_PLAN res = (Response_SEARCH_PLAN)obj;

                        for(int i = 0, n = res.Plan.Length; i < n; i++)
                        {
                            allResults.Add(res.Plan[i]);
                            dataSet["PLANS"].Add(res.Plan[i]);
                        }

                        UpdateProgress(WriteProgressText());
                    }
                    break;
                default:
                    break;
            }
        }

        // TO BE DELETED
        private bool CanWriteToFile()
        {
            bool output = false;
            output = cbWriteToFile.Checked == true && (allResults.Count % PAGE_SIZE == 0);

            return output;
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

        #endregion

        #region Test Functions
        private void SendSOAP(bool proceed)
        {
            XmlDocument xdoc = WriteRequestXML(cboCommands.SelectedItem.ToString());

            if (proceed == false)
            {
                txtResponse.Text = xdoc.OuterXml;
                ToggleControls(true);
                return;
            }

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(Monexa_URL);
            //wr.Headers.Add("SOAPAction", action);
            wr.ContentType = "text/xml;charset=utf-8";
            wr.Accept = "text/xml";
            wr.Method = "POST";

            //InsertSoapIntoRequest(xdoc, wr);
            using (Stream stream = wr.GetRequestStream())
            {
                xdoc.Save(stream);
            }
            IAsyncResult asyncResult = wr.BeginGetResponse(null, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            string result = "";
            using (WebResponse webResponse = wr.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = rd.ReadToEnd();
                }
                txtResponse.Text = result;
                ToggleControls(true);
            }
        }

        private XmlDocument WriteRequestXML(string command, Dictionary<string, string> args = null)
        {
            XmlDocument doc = new XmlDocument();
            string sXml = GetSoap(command, args);

            //sXml = sXml.Replace("{command}", command);

            if (args != null)
            {
                foreach (string key in args.Keys)
                {
                    sXml = sXml.Replace("{" + key + "}", args[key]);
                }
            }

            txtResponse.Text = sXml;
            doc.LoadXml(sXml);

            return doc;
        }

        private string GetSoap(string command, Dictionary<string, string> args = null)
        {
            //string template = SoapTemplates.SoapRequestNoEnvelope;
            string template = SoapTemplates.SoapEnvelope;

            template = template.Replace("{command}", wsCommands[command]);
            template = template.Replace("{authentication}", SoapTemplates.Authentication);
            template = template.Replace("{administrator_login_name}", "ssp_admin").Replace("{password}", "Qwert12");
            //template = InsertAuth(template);

            return template;
        }
        #endregion
    }
}
