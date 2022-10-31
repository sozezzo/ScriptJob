using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;

//using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;

using ScriptJob;

namespace Script
{
    class ServerScript
    {
        #region Private Constant Fields

        private const string Line = "--  --  --  --  --  --  --  --  --  --  --  --  --  --  --  --\r\n";

        #endregion Private Constant Fields

        #region Private Members

        private List<AlertScript> _alertList;
        private AlertCollection _alerts = null;
        private List<JobScript> _jobList = null;
        private JobCollection _jobs;
        private List<UserDefinedMessageScript> _MessageList = null;
        private List<OperatorScript> _operatorList;
        private OperatorCollection _operators = null;
        private Microsoft.SqlServer.Management.Smo.UserDefinedMessageCollection _userDefinedMessage = null;
        private List<UserDefinedMessageScript> _userDefinedMessageList;

        #endregion Private Members

        #region Static Constructors

        static ServerScript()
        {
            ShowMessage = false;
        }

        #endregion Static Constructors

        #region Public Constructors
        // TODO changer parameters -- use it when we will create object
        public ServerScript(string serverInstanceName, bool showMessage) :this (serverInstanceName, showMessage, true, true, true, true, true, "", "", "", "")
        {
            //ServerScript(serverInstanceName, showMessage, true, true, true, true, true, "", "", "", "");
        }

        int nCount = 1;
        int sizeNumber = 4;

        public ServerScript(string serverInstanceName, bool showMessage, bool overWrite, bool scriptdrop, bool scriptcreate, bool scriptenable, bool scriptdisable, string prefix, string prefixNumber, string outPath, string outfile)
        {
            
            ShowMessage = showMessage;
            ServerInstanceName = serverInstanceName;
            Microsoft.SqlServer.Management.Smo.ScriptingOptions so = new Microsoft.SqlServer.Management.Smo.ScriptingOptions();
            Show("-- Connecting to SQL Server : " + serverInstanceName);
            Microsoft.SqlServer.Management.Smo.Server srv = new Microsoft.SqlServer.Management.Smo.Server(serverInstanceName);
            _jobs = srv.JobServer.Jobs;
            _operators = srv.JobServer.Operators;
            _alerts = srv.JobServer.Alerts;
            _userDefinedMessage = srv.UserDefinedMessages;
        }

        #endregion Public Constructors

        #region Public Static Properties

        /// <summary>
        /// Flag to control the output of messages 
        /// </summary>
        public static bool ShowMessage
        {
            get; set;
        }

        #endregion Public Static Properties

        #region Public Properties

        public List<AlertScript> Alert
        {
            get
            {
                Show("-- Load alerts");
                if (_alertList == null)
                {
                    _alertList = new List<AlertScript>();
                    foreach (Alert alert in _alerts)
                    {
                        _alertList.Add(new AlertScript(alert));
                    }
                }
                return _alertList;
            }
        }

        /// <summary>
        /// Job's list 
        /// </summary>
        public List<JobScript> Job
        {
            get
            {
                Show("-- Load jobs");
                if (_jobList == null)
                {
                    _jobList = new List<JobScript>();
                    foreach (Job j in _jobs)
                    {
                        _jobList.Add(new JobScript(j));
                    }
                }
                return _jobList;
            }
        }

        public List<OperatorScript> Operator
        {
            get
            {
                Show("-- Load operators");
                if (_operatorList == null)
                {
                    _operatorList = new List<OperatorScript>();
                    foreach (Operator op in _operators)
                    {
                        _operatorList.Add(new OperatorScript(op));
                    }
                }
                return _operatorList;
            }
        }

        public string ServerInstanceName
        {
            get; set;
        }

        public List<UserDefinedMessageScript> UserDefinedmessage
        {
            get
            {
                Show("-- Load User Defined Message");
                if (_userDefinedMessageList == null)
                {
                    _userDefinedMessageList = new List<UserDefinedMessageScript>();
                    foreach (Microsoft.SqlServer.Management.Smo.UserDefinedMessage m in _userDefinedMessage)
                    {
                        _userDefinedMessageList.Add(new UserDefinedMessageScript(m));
                    }
                }
                return _userDefinedMessageList;
            }
        }

        #endregion Public Properties

        #region Public Indexers

        public JobScript this[string param]
        {
            get
            {
                JobScript ret = _jobList.Find(p => System.String.Equals(p.JobName, param, System.StringComparison.CurrentCultureIgnoreCase));
                if (ret == null)
                {
                    ret = null;
                }
                return (ret);
            }
        }

        public JobScript this[int param]
        {
            get
            {
                return _jobList[param];
            }
        }

        #endregion Public Indexers

        #region Public Methods

        public void CreateAlertScript(string outPath, bool overWrite, string prefix, string prefixNumber, bool scriptDrop, bool scriptCreate, bool scriptEnable, bool scriptDisable, string outputFilename)
        {
            string scriptAllCode = "";
            
            
            try { sizeNumber = Convert.ToInt32("0" + prefixNumber); }
            catch { sizeNumber = 4; }

            foreach (AlertScript alert in Alert)
            {
                Show("-- Script alert : " + alert.AlertName);
                string fullFileName = CreateFileName(outPath, nCount++, sizeNumber, prefix, alert.AlertName, "alert.sql");
                string script = ScriptHeader("Alert", alert.AlertName);
                if (scriptDrop)   script += alert.ScriptDrop() + "\r\n";
                if (scriptCreate) script += alert.ScriptCreate() + "\r\n";
                if (string.IsNullOrEmpty(outputFilename))
                {
                    SaveFile(fullFileName, script, overWrite);
                }
                else
                {
                    scriptAllCode += script + "\r\nGO\r\n";
                }
            }

            if (!string.IsNullOrEmpty(outputFilename))
            {
                string fullFileName = CreateFileName(outPath, 0, 0, prefix, outputFilename, "");
                SaveFile(fullFileName, scriptAllCode, overWrite);
            }
        }

        public void CreateDatabaseMailScript(string outPath, bool overWrite, string prefix, string prefixNumber, bool scriptDrop, bool scriptCreate, bool scriptEnable, bool scriptDisable, string outputFilename)
        {
            
            try { sizeNumber = Convert.ToInt32("0" + prefixNumber); }
            catch { sizeNumber = 4; }

            DatabaseMail dbMail = new DatabaseMail();
            string script = ScriptHeader("Database Mail", "Configuration");

            if (scriptDrop) script += dbMail.Drop(SqlCmd(ServerInstanceName));
            if (scriptCreate) script += dbMail.Create(SqlCmd(ServerInstanceName));

            string scriptAllCode = script;

            string fullFileName = CreateFileName(outPath, nCount++, sizeNumber, prefix, "DatabaseMail", ".sql");
            if (string.IsNullOrEmpty(outputFilename))
            {
                SaveFile(fullFileName, script, overWrite);
            }
            else
            {
                scriptAllCode += script + "\r\nGO\r\n";
            }

            if (!string.IsNullOrEmpty(outputFilename))
            {
                fullFileName = CreateFileName(outPath, 0, 0, prefix, outputFilename, "");
                SaveFile(fullFileName, scriptAllCode, overWrite);
            }
        }


        

        /// <summary>
        /// Create job script
        /// </summary>
        /// <param name="categoryFilter"></param>
        /// <param name="outPath"></param>
        /// <param name="overWrite"></param>
        /// <param name="prefix"></param>
        /// <param name="prefixNumber"></param>
        /// <param name="scriptDrop"></param>
        /// <param name="scriptCreate"></param>
        /// <param name="scriptEnable"></param>
        /// <param name="scriptDisable"></param>
        /// <param name="outputFilename"></param>
        public void CreateJobScript(string categoryFilter, string outPath, bool overWrite, string prefix, string prefixNumber, bool scriptDrop, bool scriptCreate, bool scriptEnable, bool scriptDisable, string outputFilename)
        {
            string scriptAllCode = "";
            
            
            try { sizeNumber = Convert.ToInt32("0" + prefixNumber); }
            catch { sizeNumber = 4; }

            List<JobScript> jobs = JobsFiltreByCategory(categoryFilter);
            foreach (JobScript job in jobs)
            {

                Show("-- Script Job : " + job.JobName);
                string fullFileName = CreateFileName(outPath, nCount++, sizeNumber, prefix, job.JobName, "job.sql");
                string script = ScriptHeader("Job", job.JobName);
                if (scriptDrop) script += job.ScriptDrop() + "\r\n";
                if (scriptCreate) script += job.ScriptCreate() + "\r\n";
                if (scriptEnable) script += job.ScriptEnable() + "\r\n";
                if (scriptDisable) script += job.ScriptDisable() + "\r\n";

                if (string.IsNullOrEmpty(outputFilename))
                {
                    SaveFile(fullFileName, script, overWrite);
                }
                else
                {
                    scriptAllCode += script+"\r\nGO\r\n";
                }
            }

            if (!string.IsNullOrEmpty(outputFilename))
            {
                string fullFileName = CreateFileName(outPath, 0, 0, prefix, outputFilename, "");
                SaveFile(fullFileName, scriptAllCode, overWrite);
            }
        }

        public void CreateMessageScript(string outPath, bool overWrite, string prefix, string prefixNumber, bool scriptDrop, bool scriptCreate, bool scriptEnable, bool scriptDisable, string outputFilename)
        {
            string scriptAllCode = "";
            
            
            try { sizeNumber = Convert.ToInt32("0" + prefixNumber); }
            catch { sizeNumber = 4; }

            foreach (UserDefinedMessageScript msg in UserDefinedmessage)
            {
                Show("-- Script message Id : " + msg.MessageID);
                string fullFileName = CreateFileName(outPath, nCount++, sizeNumber, prefix, msg.MessageName, "message.sql");
                string script = ScriptHeader("UserDefinedmessage", msg.MessageName);
                if (scriptDrop) script += msg.ScriptDrop() + "\r\n";
                if (scriptCreate) script += msg.ScriptCreate() + "\r\n";
                if (string.IsNullOrEmpty(outputFilename))
                {
                    SaveFile(fullFileName, script, overWrite);
                }
                else
                {
                    scriptAllCode += script + "\r\nGO\r\n";
                }
            }

            if (!string.IsNullOrEmpty(outputFilename))
            {
                string fullFileName = CreateFileName(outPath, 0, 0, prefix, outputFilename, "");
                SaveFile(fullFileName, scriptAllCode, overWrite);
            }
        }

        public void CreateOperatorScript(string outPath, bool overWrite, string prefix, string prefixNumber, bool scriptDrop, bool scriptCreate, bool scriptEnable, bool scriptDisable, string outputFilename)
        {
            string scriptAllCode = "";
            
            
            try { sizeNumber = Convert.ToInt32("0" + prefixNumber); }
            catch { sizeNumber = 4; }

            foreach (OperatorScript op in Operator)
            {
                Show("-- Script operator : " + op.OperatorName);
                string fullFileName = CreateFileName(outPath, nCount++, sizeNumber, prefix, op.OperatorName, "operator.sql");
                string script = ScriptHeader("Operator", op.OperatorName);
                if (scriptDrop) script += op.ScriptDrop() + "\r\n";
                if (scriptCreate) script += op.ScriptCreate() + "\r\n";
                if (string.IsNullOrEmpty(outputFilename))
                {
                    SaveFile(fullFileName, script, overWrite);
                }
                else
                {
                    scriptAllCode += script + "\r\nGO\r\n";
                }
            }

            if (!string.IsNullOrEmpty(outputFilename))
            {
                string fullFileName = CreateFileName(outPath, 0, 0, prefix, outputFilename, "");
                SaveFile(fullFileName, scriptAllCode, overWrite);
            }
        }

        public string GetSourceServerFromAlwaysOn(string alwaysOn)
        {
            JobScript js = new JobScript();
            string sql = js.ScriptGetServer(true);
            return Db.ExecuteGetValue(SqlCmd(alwaysOn),sql);
        }

        public string GetTargetServerFromAlwaysOn(string alwaysOn)
        {
            JobScript js = new JobScript();
            string sql = js.ScriptGetServer(false);
            return Db.ExecuteGetValue(SqlCmd(alwaysOn), sql);
        }

        public List<JobScript> JobsFiltreByCategory(string categoryFilter)
        {
            if (string.IsNullOrEmpty(categoryFilter)) return Job;
            return Job.FindAll(p => String.Equals(p.JobCategory, categoryFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        public SqlCommand SqlCmd(string serverName)
        {
            string connectionString = string.Format("Data Source={0};Initial Catalog=msdb;Integrated Security=True;Max Pool Size=250;MultipleActiveResultSets=True;Connect Timeout=30;Application Name=ScriptJob", serverName);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            //Show("-- Connection String : " + connectionString);
            SqlCommand cmd = new SqlCommand { Connection = sqlConnection, CommandType = CommandType.Text };
            sqlConnection.Open();
            return cmd;
        }

        /// <summary>
        /// Synchronise jobs between two SQL Servers 
        /// </summary>
        /// <param name="sourceServer"></param>
        /// <param name="targetServer"></param>
        /// <param name="categoryFilter"></param>
        /// <param name="setJobStatusDisable"></param>
        public void SynchroServer(string sourceServer, string targetServer, string categoryFilter, bool setJobStatusDisable)
        {
            SqlCommand cmdTarget = SqlCmd(targetServer);
            SqlCommand cmdSource = SqlCmd(sourceServer);

            #region Synchronise UserDefinedmessage

            foreach (UserDefinedMessageScript msg in UserDefinedmessage)
            {
                Show("-- Synchronise User Defined UserDefinedmessage : " + msg.MessageID.ToString());
                //if (!ExecuteNonQuery(cmdTarget, msg.ScriptDrop())) continue;
                if (!Db.ExecuteNonQuery(cmdTarget, msg.ScriptCreate())) continue;
            }

            #endregion Synchronise UserDefinedmessage

            #region Synchronise Operator

            foreach (OperatorScript op in Operator)
            {
                Show("-- Synchronise Operator : " + op.OperatorName);
                if (!Db.ExecuteNonQuery(cmdTarget, op.ScriptDrop())) continue;
                if (!Db.ExecuteNonQuery(cmdTarget, op.ScriptCreate())) continue;
                //if (!ExecuteNonQuery(cmd, op.ScriptDisable())) continue;
            }

            #endregion Synchronise Operator

            #region Synchronise Alert

            foreach (AlertScript alert in Alert)
            {
                Show("-- Synchronise Alert : " + alert.AlertName);
                if (!Db.ExecuteNonQuery(cmdTarget, alert.ScriptDrop())) continue;
                if (!Db.ExecuteNonQuery(cmdTarget, alert.ScriptCreate())) continue;
                //if (!ExecuteNonQuery(cmd, alert.ScriptDisable())) continue;
            }

            #endregion Synchronise Alert

            #region Synchronise DatabaseMail configuration
            Show("-- Synchronise DatabaseMail");
            DatabaseMail dbMail = new DatabaseMail();
            //if (!Db.ExecuteNonQuery(cmdTarget, dbMail.ScriptDrop())) continue;
            //if (!Db.ExecuteNonQuery(cmdTarget, dbMail.ScriptCreate())) continue;
            //if (!ExecuteNonQuery(cmd, alert.ScriptDisable())) continue;
            #endregion Synchronise DatabaseMail configuration

            #region Synchronise job

            List<JobScript> jobs = JobsFiltreByCategory(categoryFilter);
            foreach (JobScript job in jobs)
            {
                if (NeedToUpdateJob(cmdTarget, cmdSource, job.JobName))
                {
                    Show(string.Format("-- Synchronise job : {0}", job.JobName));
                    if (!Db.ExecuteNonQuery(cmdTarget, job.ScriptDrop())) continue;
                    if (!Db.ExecuteNonQuery(cmdTarget, job.ScriptCreate())) continue;
                    if (setJobStatusDisable)
                    {
                        if (!Db.ExecuteNonQuery(cmdTarget, job.ScriptDisable())) continue;
                    }
                }
                else
                {
                    Show(string.Format("   Synchronised job : {0} .", job.JobName));
                }
            }

            #endregion Synchronise job
        }

        #endregion Public Methods

        #region Private Static Methods

        private static string CreateFileName(string path, int prefixNumber, int prefixNumberSize, string prefix, string name, string ext)
        {
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            if (path.Substring(path.Length - 1, 1) != "\\") {path += "\\";}
            string formatString = new String('0', prefixNumberSize);
            string sprefixNumber = prefixNumber.ToString(formatString) + ".";
            string ret = string.Format("{0}{1}{2}{3}", path, (prefixNumberSize > 0 ? sprefixNumber : ""), prefix, SafeFileName(name + "." + ext)).Replace("..",".");
            return ret;
        }

        private static string SafeFileName(string fileName)
        {
            fileName = fileName.Replace("\\", "-");
            fileName = fileName.Replace(":", "-");
            fileName = fileName.Replace("/", "-");
            return fileName;
        }

        private static void SaveFile(string fullPathname, string script, bool overWrite)
        {
            //script = ScriptHeader() + script;
            string filename = fullPathname;
            try
            {

                #region overWrite

                FileInfo fn = new FileInfo(filename);
                try
                {
                    if (fn.Exists && overWrite)
                    {
                        fn.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(filename);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.Source);
                    return;
                }

                #endregion overWrite

                #region Write

                TextWriter tw = null;

                try
                {
                    tw = new StreamWriter(filename, true, Encoding.UTF8);
                    tw.WriteLine(script);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(filename);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.Source);
                    return;
                }

                #endregion Write the message

                #region Close file and clean up

                try
                {
                    tw.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                // clean up
                fn = null;
                tw = null;

                #endregion Close file and clean up

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void Show(string text)
        {
            if (ShowMessage)
            Console.WriteLine(text);
        }

        #endregion Private Static Methods

        #region Private Methods

        private bool NeedToUpdateJob(SqlCommand cmdTarget, SqlCommand cmdSource, string jobName)
        {
            DateTime dateSource = Db.ExecuteGetValue(cmdSource, JobScript.ScriptGet(jobName), DateTime.MinValue);
            DateTime dateTarget = Db.ExecuteGetValue(cmdTarget, JobScript.ScriptGet(jobName), DateTime.MinValue);

            return (dateSource > dateTarget);
        }

        private string ScriptHeader(string scriptType, string scriptName)
        {
            string ret = string.Format("-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --\r\n-- Script {0} : {2}\r\n-- by Sozezzo\r\n-- {1}\r\n-- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --\r\nPRINT @@SERVERNAME;\r\n", scriptType, DateTime.Now.ToLongDateString(), scriptName);
            return ret;
        }

        #endregion Private Methods
    }
}