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

        private List<REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE> allSubscribers;

        private List<string> accountIDs;
        private string currentKey;
        private string requestKey;
        private int recCounter;
        private int errorCounter;
        private int fileCounter;
        private int PAGE_SIZE = 10;

        private Dictionary<string, object> allRequests;
        private Dictionary<string, object> allResponses;
        private Dictionary<string, object> allErrors;

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
                { "PingServer", "Web service ping" },
                { "AllAccounts", "Extract accounts" },
                //{ "AccountDetails", "Extract account details" },
                //{ "AccountPricing", "Extract account pricing" },
                //{ "Subscribers", "Extract subscribers" },
                //{ "SubAccounts", "Extract sub-accounts" },
                { "Invoices", "Extract invoices" },
                //{ "InvoiceDetails", "Extract invoice details" },
                //{ "Transactions", "Extract transactions" },
                //{ "Plans", "Extract plans" },
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

            allSubscribers = new List<REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE>();
        }

        private void ResetCounters()
        {
            sbFile = new StringBuilder();
            accountIDs = new List<string>();
            recCounter = 0;
            errorCounter = 0;
            fileCounter = 1;
        }
        #endregion

        #region UI Functions
        private void RunNextCommand()
        {
            //Panel pn = (Panel)Controls.Find("pn" + commandsToProcess[0], true)[0];
            //pn.Visible = true;

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
                    //GetInvoiceDetails();
                    break;
                case "Transactions":
                    //GetTransactions();
                    break;
                case "Plans":
                    //GetPlans();
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

                WriteResults(e.UserState);
            }
            else
            {
                if (commandsToProcess[0] == "PingServer")
                {
                    UpdateProgress("Web service is up!");
                }
                else
                {
                    allResponses.Add(requestKey, e.UserState);
                    recCounter++;

                    WriteResults(e.UserState);
                }
            }
        }

        private void bwAPI_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (commandsToProcess.Count > 0)
            {
                ListViewItem lvi = FindCurrentListViewItem();
                lvi.ImageIndex = (int)TaskStatus.Success;

                commandsToProcess.RemoveAt(0);
            }

            if (commandsToProcess.Count > 0)
                RunNextCommand();
            else
            {
                txtResponse.Text = "Total web service requests sent: " +
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

        #region Web Service Functions
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

        private void GetAllAccounts()
        {
            ResetCounters();

            try
            {
                SearchSubscriberRequest req = new SearchSubscriberRequest();
                req.Authentication = auth;

                SEARCH_SUBSCRIBER search = new SEARCH_SUBSCRIBER();
                search.account_type = account_type_enum.all;
                search.account_typeSpecified = true;
                req.SEARCH_SUBSCRIBER = search;

                requestKey = commandsToProcess[0];
                allRequests.Add(requestKey, search);

                Response_SEARCH_SUBSCRIBER res = ws.searchSubscriber(req);

                bwAPI.ReportProgress(100, res);
            }
            catch (Exception ex)
            {
                bwAPI.ReportProgress(0, ex);
            }
        }

        private void GetAccountDetails()
        {
            ResetCounters();
            accountIDs = allSubscribers
                .Where(sub => String.IsNullOrEmpty(sub.Account_Information.account_id) == false)
                .Select(sub => sub.Account_Information.account_id)
                .ToList();

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in allSubscribers)
            {
                try
                {
                    if (sub.Account_Information.account_id != null)
                    {
                        LookupSubscriberRequest req = new LookupSubscriberRequest();
                        req.Authentication = auth;

                        LOOKUP_SUBSCRIBER search = new LOOKUP_SUBSCRIBER();
                        search.ItemElementName = ItemChoiceType1.account_id;
                        search.Item = sub.Account_Information.account_id.ToString();
                        req.LOOKUP_SUBSCRIBER = search;

                        currentKey = sub.Account_Information.account_id;
                        requestKey = commandsToProcess[0] + "_" + currentKey.ToString();

                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[] res = ws.lookupSubscriber(req);
                        allRequests.Add(requestKey, search);

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
        private void GetAccountPricing()
        {
            ResetCounters();

            
        }

        private void GetSubscribers()
        {
            ResetCounters();

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in allSubscribers)
            {
                try
                {
                    if (sub.Contact_Information.master_account_id != null)
                    {
                        SearchSubscriberRequest req = new SearchSubscriberRequest();
                        req.Authentication = auth;
                        req.SEARCH_SUBSCRIBER = new SEARCH_SUBSCRIBER();
                        req.SEARCH_SUBSCRIBER.account_id = sub.Contact_Information.master_account_id;

                        currentKey = sub.Contact_Information.master_account_id;
                        requestKey = commandsToProcess[0] + "_" + currentKey.ToString();
                        allRequests.Add(requestKey, req);

                        Response_SEARCH_SUBSCRIBER res = ws.searchSubscriber(req);

                        bwAPI.ReportProgress(100, res);
                    }
                }
                catch(Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }
        }

        // TO BE IMPLEMENTED
        private void GetSubAccounts()
        {
            ResetCounters();
            /*
            accountIDs = allSubscribers
                .Where(sub => String.IsNullOrEmpty(sub.Account_Information.account_id) == false)
                .Select(sub => sub.Account_Information.account_id)
                .ToList();

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in allSubscribers)
            {
                try
                {
                    SearchSubscriberRequest req = new SearchSubscriberRequest();
                    req.Authentication = auth;

                    SEARCH_SUBSCRIBER search = new SEARCH_SUBSCRIBER();
                }
            }*/
        }

        private void GetInvoices()
        {
            ResetCounters();

            foreach (REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE sub in allSubscribers)
            {
                try
                {
                    SearchInvoiceRequest req = new SearchInvoiceRequest();
                    req.Authentication = auth;
                    req.SEARCH_INVOICE = new SEARCH_INVOICE();
                    req.SEARCH_INVOICE.information_type = information_type_enum.full;
                    req.SEARCH_INVOICE.ItemElementName = ItemChoiceType2.account_id;
                    req.SEARCH_INVOICE.Item = sub.Account_Information.account_id;

                    currentKey = sub.Account_Information.account_id;
                    requestKey = commandsToProcess[0] + "_" + currentKey.ToString();
                    allRequests.Add(requestKey, req);

                    Invoice_Information[] res = ws.searchInvoice(req);

                    bwAPI.ReportProgress(100, res);
                }
                catch (Exception ex)
                {
                    bwAPI.ReportProgress(0, ex);
                }
            }


        }
        #endregion

        #region File I/O Functions
        private void WriteToFile()
        {
            string fileName;

            fileName = Directory.GetCurrentDirectory() + "\\" + commandsToProcess[0] + "_" +
                fileCounter.ToString() + ".txt";
            File.WriteAllText(fileName, sbFile.ToString());

            fileCounter++;
            sbFile = new StringBuilder();
        }

        private void WriteResults (object obj)
        {
            //string fileName;
            //StringBuilder sb = new StringBuilder();

            switch (commandsToProcess[0])
            {
                case "AllAccounts":
                    {
                        Response_SEARCH_SUBSCRIBER res = (Response_SEARCH_SUBSCRIBER)obj;

                        for (int i = 0, n = res.Subscriber_Information.Length; i < n; i++)
                        {
                            if (cbWriteToFile.Checked == true && (i % PAGE_SIZE == 0 && i > 0))
                            {
                                WriteToFile();
                            }

                            REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE x = res.Subscriber_Information[i];
                            sbFile.Append(JsonConvert.SerializeObject(x)).Append(Environment.NewLine);
                            allSubscribers.Add(x);

                            /*if (string.IsNullOrEmpty(x.Account_Information.account_id) == false)
                            {
                                accountIDs.Add(Int32.Parse(x.Account_Information.account_id));
                            }*/

                            if (cbWriteToFile.Checked == true && (i % PAGE_SIZE > 0 && i == (n - 1)))
                            {
                                WriteToFile();
                            }
                        }

                        UpdateProgress("Received " + res.result_size + " accounts");
                    }
                    break;
                case "AccountDetails":
                    {
                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[] res = (REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE[])obj;

                        if (cbWriteToFile.Checked == true && recCounter % PAGE_SIZE == 0 && recCounter > 0)
                        {
                            WriteToFile();
                        }
                        REP_LOOK_SUB_SUBSCRIBER_INFORMATION_TYPE x = res[0];
                        sbFile.Append(JsonConvert.SerializeObject(x)).Append(Environment.NewLine);

                        if (CanWriteToFile() == true)
                        {
                            WriteToFile();
                        }

                        //recCounter++;
                        UpdateProgress(WriteProgressText());
                    }
                    break;
                case "GetSubscribers":
                    {
                        Response_SEARCH_SUBSCRIBER res = (Response_SEARCH_SUBSCRIBER)obj;
                        for (int i = 0, n = res.Subscriber_Information.Length; i < n; i++)
                        {
                            if (cbWriteToFile.Checked == true && (i % PAGE_SIZE == 0 && i > 0))
                            {
                                WriteToFile();
                            }

                            REP_SEA_SUB_SUBSCRIBER_INFORMATION_TYPE x = res.Subscriber_Information[i];
                            sbFile.Append(JsonConvert.SerializeObject(x)).Append(Environment.NewLine);

                            //recCounter++;
                            UpdateProgress(WriteProgressText());
                        }
                    }
                    break;
                case "Invoices":
                    {
                        Invoice_Information[] res = (Invoice_Information[])obj;
                        for (int i=0, n=res.Length; i<n; i++)
                        {
                            if (cbWriteToFile.Checked == true && (i % PAGE_SIZE == 0 && i > 0))
                            {
                                WriteToFile();
                            }

                            Invoice_Information x = res[i];
                            sbFile.Append(JsonConvert.SerializeObject(x)).Append(Environment.NewLine);

                            UpdateProgress(WriteProgressText());
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private bool CanWriteToFile()
        {
            bool output = false;
            output = cbWriteToFile.Checked == true &&
                (recCounter % PAGE_SIZE > 0 && recCounter == accountIDs.Count);

            return output;
        }

        private string WriteProgressText()
        {
            StringBuilder sb = new StringBuilder();
            /*
            if ((recCounter + errorCounter) < accountIDs.Count)
            {
                sb.Append("IN PROGRESS");
            }
            else
            {
                sb.Append("DONE");
                if (errorCounter > 0)
                {
                    sb.Append(" WITH ERRORS");
                }
            }
            */
            sb.Append("Processed ").Append(recCounter).Append(" of ")
                .Append(accountIDs.Count).Append(" requests");

            ListViewItem lvi = FindCurrentListViewItem();
            if ((recCounter + errorCounter) >= accountIDs.Count)
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
            //sb.Append("Processed ").Append(recCounter).Append(" of ")
            //    .Append(accountIDs.Count).Append(" requests");

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
