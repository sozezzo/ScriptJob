using System;

namespace Script
{
    class Program
    {
        #region Private Static Methods
 
        private static Arguments _commandLine;
        static void Main(string[] args)
        {
            
            ShowInfo();

            #region Arguments
            
            _commandLine = new Arguments(args);
            string command = _commandLine["command"].Value.ToLower();
            if (!_commandLine["command"].HasValue)
            {   
                ShowHelp("Missing command");
                return;
            }

            bool scriptcreate = _commandLine["scriptcreate"].ValueBool;
            bool scriptdrop = _commandLine["scriptdrop"].ValueBool;
            bool scriptenable = _commandLine["scriptenable"].ValueBool;
            bool scriptdisable = _commandLine["scriptdisable"].ValueBool;
            bool setjobdisable = _commandLine["setjobdisable"].ValueBool;
            bool confirmation = _commandLine["y"].ValueBool;
            bool overWrite = _commandLine["OverWrite"].ValueBool;


            ArgumentType alwaysOnServer = _commandLine["AlwaysOnServer"];
            ArgumentType sourceServer = _commandLine["SourceServer"];
            ArgumentType targetServer = _commandLine["TargetServer"];
            ArgumentType categoryFilter = _commandLine["CategoryFilter"];
            ArgumentType outPath = _commandLine["outpath"];
            ArgumentType backupoutpath = _commandLine["backupoutpath"];
            ArgumentType prefix = _commandLine["Prefix"];
            ArgumentType prefixNumber = _commandLine["PrefixNumber"];
            ArgumentType outfile = _commandLine["Outfile"];

            ShowArguments();
            
            #endregion Arguments

            switch (command)
            {

                case "all":

                    #region createscript
                    
                    Show("-- Create All Script");
                    if (!outPath.HasValue) ShowHelp("Missing outPath");
                    if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                    ServerScript createAll = new ServerScript(sourceServer.Value, true);
                    if (!(scriptdrop || scriptcreate || scriptenable || scriptdisable))
                    {
                        ShowHelp("No script option");
                    }
                    if (overWrite)
                    {
                        Show("-- Output Path : " + outPath.Value);
                        Show("-- Files will be overwritten.");
                        if (!WaitConfirmation(confirmation)) return;
                    }

                    createAll.CreateDatabaseMailScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    createAll.CreateOperatorScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    createAll.CreateMessageScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    createAll.CreateAlertScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    createAll.CreateJobScript(categoryFilter.Value, outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    
                    #endregion createscript

                    break;

                case "job":

                    #region createscript
                    
                    Show("-- Create Job Script");
                    if (!outPath.HasValue) ShowHelp("Missing outPath");
                    if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                    ServerScript createJob = new ServerScript(sourceServer.Value, true);
                    if (!(scriptdrop || scriptcreate || scriptenable || scriptdisable))
                    {
                        ShowHelp("No script option");
                    }
                    if (overWrite)
                    {
                        Show("-- Output Path : " + outPath.Value);
                        Show("-- Files will be overwritten.");
                        if (!WaitConfirmation(confirmation)) return;
                    }
                    createJob.CreateJobScript(categoryFilter.Value, outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    
                    #endregion createscript

                    break;

                case "mail":

                    #region createscript
                    Show("-- Create Operator Script");
                    if (!outPath.HasValue) ShowHelp("Missing outPath");
                    if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                    ServerScript createDatabaseMail = new ServerScript(sourceServer.Value, true);

                    if (overWrite)
                    {
                        Show("-- Output Path : " + outPath.Value);
                        Show("-- Files will be overwritten.");
                        if (!WaitConfirmation(confirmation)) return;
                    }
                    createDatabaseMail.CreateDatabaseMailScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    #endregion createscript

                    break;

                case "operator":

                    #region createscript
                    Show("-- Create Operator Script");
                    if (!outPath.HasValue) ShowHelp("Missing outPath");
                    if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                    ServerScript createOperator = new ServerScript(sourceServer.Value, true);
                    //if (!(scriptdrop || scriptcreate || scriptenable || scriptdisable))
                    if (!(scriptdrop || scriptcreate))
                    {
                        ShowHelp("No script option");
                    }
                    if (overWrite)
                    {
                        Show("-- Output Path : " + outPath.Value);
                        Show("-- Files will be overwritten.");
                        if (!WaitConfirmation(confirmation)) return;
                    }
                    createOperator.CreateOperatorScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    #endregion createscript

                    break;

                case "alert":

                    #region createscript
                    Show("-- Create Alert Script");
                    if (!outPath.HasValue) ShowHelp("Missing outPath");
                    if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                    ServerScript createAlert = new ServerScript(sourceServer.Value, true);
                    //if (!(scriptdrop || scriptcreate || scriptenable || scriptdisable))
                    if (!(scriptdrop || scriptcreate))
                    {
                        ShowHelp("No script option");
                    }
                    if (overWrite)
                    {
                        Show("-- Output Path : " + outPath.Value);
                        Show("-- Files will be overwritten.");
                        if (!WaitConfirmation(confirmation)) return;
                    }
                    createAlert.CreateAlertScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    #endregion createscript

                    break;

                case "message":

                    #region createscript
                    Show("-- Create User Defined Messages Script");
                    if (!outPath.HasValue) ShowHelp("Missing outPath");
                    if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                    ServerScript createMessage = new ServerScript(sourceServer.Value, true);
                    //if (!(scriptdrop || scriptcreate || scriptenable || scriptdisable))
                    if (!(scriptdrop || scriptcreate))
                    {
                        ShowHelp("No script option");
                    }
                    if (overWrite)
                    {
                        Show("-- Output Path : " + outPath.Value);
                        Show("-- Files will be overwritten.");
                        if (!WaitConfirmation(confirmation)) return;
                    }
                    createMessage.CreateMessageScript(outPath.Value, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                    #endregion createscript

                    break;

                case "synchroserver":

                    #region synchroserver
                    
                    Show("-- Synchronise servers --> Source SQL Server to overwrite target SQL Server");
                    Show("--   * User defined messages");
                    Show("--   * Operators");
                    Show("--   * Alerts");
                    Show("--   * Jobs");
                    Show("--   ");
                    if (alwaysOnServer.HasValue)
                    {
                        ServerScript sourceJob1 = new ServerScript(alwaysOnServer.Value, true);
                        targetServer = new ArgumentType("targetServer", sourceJob1.GetTargetServerFromAlwaysOn(alwaysOnServer.Value));
                        sourceServer = new ArgumentType("SourceServer", sourceJob1.GetSourceServerFromAlwaysOn(alwaysOnServer.Value));
                        if (!sourceServer.HasValue || sourceServer.Value == "") ShowHelp("Missing sourceServer");
                        if (!targetServer.HasValue || targetServer.Value == "") ShowHelp("Missing targetServer");
                        Show("-- Source SQL Server (Primary)  : " + sourceServer.Value);
                        Show("-- Target SQL Server (Secondary): " + targetServer.Value);
                    }
                    else
                    {
                        if (!sourceServer.HasValue) ShowHelp("Missing sourceServer");
                        if (!targetServer.HasValue) ShowHelp("Missing targetServer");
                        Show("-- Source SQL Server : " + sourceServer.Value);
                        Show("-- Target SQL Server : " + targetServer.Value);
                    }
                  
                    if (!WaitConfirmation(confirmation)) return;

                    ServerScript sourceJob = new ServerScript(sourceServer.Value, true);

                    if (backupoutpath.Defined)
                    {
                        string now = Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd_hhmmss");
                        string backupoutpathP = string.Format("{0}{1}{3}\\{2}_P", backupoutpath.Value, backupoutpath.Value.Substring(backupoutpath.Value.Length - 1, 1) == "\\" ? "" : "\\", sourceServer.Value, now);
                        string backupoutpathS = string.Format("{0}{1}{3}\\{2}_S", backupoutpath.Value, backupoutpath.Value.Substring(backupoutpath.Value.Length - 1, 1) == "\\" ? "" : "\\", targetServer.Value, now);
                        overWrite = true;
                        scriptcreate = true;
                        scriptdrop = false;
                        scriptenable = true;
                        scriptdisable = false;
                        Show("--");
                        Show("-- ** **");
                        Show("-- ** Backup : " + sourceServer.Value+" **");
                        Show("-- ** **");
                        sourceJob.CreateDatabaseMailScript(backupoutpathP, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        sourceJob.CreateOperatorScript(backupoutpathP, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        sourceJob.CreateMessageScript(backupoutpathP, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        sourceJob.CreateAlertScript(backupoutpathP, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        sourceJob.CreateJobScript(categoryFilter.Value, backupoutpathP, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);

                        scriptenable = false;
                        Show("--");
                        Show("-- ** **");
                        Show("-- ** Backup : " + targetServer.Value + " **");
                        Show("-- ** **");
                        ServerScript targetJob = new ServerScript(sourceServer.Value, true);
                        targetJob.CreateDatabaseMailScript(backupoutpathS, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        targetJob.CreateOperatorScript(backupoutpathS, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        targetJob.CreateMessageScript(backupoutpathS, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        targetJob.CreateAlertScript(backupoutpathS, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        targetJob.CreateJobScript(categoryFilter.Value, backupoutpathS, overWrite, prefix.Value, prefixNumber.Value, scriptdrop, scriptcreate, scriptenable, scriptdisable, outfile.Value);
                        Show("--");
                    }

                    Show("--");
                    Show("-- ** ");
                    Show("-- ** Server to Server synchronization Source to Target **");
                    sourceJob.SynchroServer(sourceServer.Value, targetServer.Value, categoryFilter.Value, setjobdisable);
                    Show("--");

                    #endregion synchroserver

                    break;

                default:
                    ShowHelp("Command not valid.");
                    break;
            }


        }

        private static bool WaitConfirmation(bool confirmation)
        {
            if (confirmation) return true;

            Console.WriteLine("\r\nAttention data can be lost.");

            do
            {
                Console.WriteLine("\r\nPress the Escape (Esc) key to quit or 'y' (Y) key to continue: ");
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.Escape) return false;
                if (cki.Key == ConsoleKey.Y) return true;
            } while (true);
            
        }

        private static void PrintArgs()
        {
            //string strRegex = @"(?<name>-{1,2}\S*)(?:[=:]?|\s+)(?<value>[^-\s].*?)?(?=\s+[-\/]|$)";
            //Regex myRegex = new Regex(strRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //string strTargetString = @"--switch value1 --switch-2 ""c:\folder 1\file1.txt"" -switch3 fffffff --switch4 -switch5 ""c:\mypath""  --stest:on";

            //var v = myRegex.Split(strTargetString);
            //Console.WriteLine(v);

            //Regex regex = new Regex(@"(?<name>-{1,2}\S*)(?:[=:]?|\s+)(?<value> [^-\s].*?)?(?=\s+[-\/]|$)");

            //string commands = "--switch value1 --switch2 \"c:\\folder 1\\file1.txt\" -switch-3 value-3 --switch4 -switch5";

            //List< KeyValuePair< string, string> >  matches = (from match in regex.Matches(commands).Cast<match>() select new KeyValuePair< string, string> (match.Groups["switch"].Value, match.Groups["value"].Value)).ToList();

            ////foreach (KeyValuePair< string, string> _match in matches)
            ////{
            ////    Console.WriteLine("switch:" + _match.Key + "  value:" + _match.Value);
            ////}
        }

        private static void UpdateJobTargetServer(string serverName, JobScript job)
        {

        }

        private static void ShowArguments()
        {

            const string message = @"::
:: Parameters :
::";

            Show(message);
            foreach (var c in _commandLine.Parameters)
            {
                Show("      "+(c.HasValue ? string.Format("/{0}={1}", c.Name, c.Value) : string.Format("/{0}", c.Name)));
            }
            Show("");
        }

        private static void ShowInfo()
        {
            const string message = @"
-------------------------------------------------------------------------------
   Script Job   ::     Create Script/Synchronise for SQL Server (2012/2014)
-------------------------------------------------------------------------------

* Script Job / Alert / Operator

";
            Show(message);
            
        }

        private static void ShowHelp(string text)
        {
            const string message = @"
Usage  :: ScriptJob /command=<commands> /SourceServer=<SourceSqlServer> [options]


::
:: Command to create Script Options :
::

  /command=all                    :: Create sql script of all objects
  /command=job                    :: Create sql script of jobs
  /command=alert                  :: Create sql script of alerts
  /command=operator               :: Create sql script of operators
  /command=message                :: Create sql script of user defined messages
  /command=mail                   :: Create sql script of Database Mail Configuration 
                                     You always append a configuration. You do not drop a configuration. 

  /SourceServer=<Server\Instance> :: SQL Server and instance name

  /outpath=<MyPath>               :: Path to output files

:: options

  /scriptdrop                     :: Create script to drop

  /scriptcreate                   :: Create script to create

  /setjobdisable                  :: Set Job status = Disable

  /CategoryFilter=<param>         :: Filter jobs by category
  
  /Overwrite                      :: Over write all output files (default value is false)

  /Prefix=<text>                  :: Prefix to save files (default value is empty)

  /PrefixNumber=<number>          :: Use numbet as prefix to save files (default value is empty)
                                     ex.: 001.filename.sql


::
:: Command to synchronise objects between two servers :
:: Warning, you must be sure
::

  /command=synchroserver            :: synchronise servers   

  /SourceServer=<Server\Instance>   :: Source SQL Server and instance name 
  /TargetServer=<Server\Instance>   :: Target SQL Server and instance name 

  /AlwaysOnServer=<Server\Instance> :: AlwaysOn SQL Server and instance name 
                                       * equivalent to use: /SourceServer=PrimaryServer 
                                                            /TargetServer=SecondaryServer

  /backupoutpath                    :: Path to backup script 
                                       Status value==<P>rimary|<S>econdary
                                       %path%\yyyy-mm-dd-hhmmss_<server name>_%status%

:: options
  
  /setjobdisable                    :: Create script to set status = Disable (default value is 'to keep the status')

  /y                                :: No confirmation to synchronise (default value is 'to confirme' when you can lost data.)


Example:

scriptjob /SourceServer=""MyDBServerName\""  /command=job /OverWrite /scriptcreate /scriptdrop /scriptdisable /CategoryFilter:MyCategory  /outpath=C:\TEMP\Script\

";


            Show(message);
            Show(text);



            Environment.Exit(-1);
        }

        private static void Show(string text)
        {
            Console.WriteLine(text);
        }

        #endregion Private Static Methods
    }
}