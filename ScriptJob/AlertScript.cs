using System.Collections.Specialized;

using Microsoft.SqlServer.Management.Smo;

namespace Script
{
    class AlertScript : SqlScript
    {


        #region Private Members

        //private Microsoft.SqlServer.Management.Smo.Agent.Alert _alert;
        private string _scriptCreate = "";

        #endregion Private Members

        #region Public Constructors

        public AlertScript(Microsoft.SqlServer.Management.Smo.Agent.Alert op)
        {
            AlertName = op.Name;

            StringCollection stringCollection = op.Script(_scriptingOptions);
            _scriptCreate = ScriptCreate(stringCollection);
        }

        #endregion Public Constructors

        #region Public Properties

        public string AlertName
        {
            get; set;
        }

        #endregion Public Properties

        #region Public Override Methods

        public override string ScriptCreate()
        {
            return _scriptCreate;
        }

        public override string ScriptDisable()
        {
            string alertName = AlertName.Replace("'", "''");
            string script = Line1 + @"
PRINT '-- DROP ALERT : {0}';
IF  EXISTS (SELECT [id] FROM msdb.dbo.sysalerts WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_update_alert @name=N'{0}', @enabled=0;
end;
ELSE
BEGIN
    print '-- Alert does not exist : {0}';
END;
            " + Line1;
            return string.Format(script, alertName);

        }

        public override string ScriptDrop()
        {
            string alertName = AlertName.Replace("'", "''");
            string script = Line1 + @"
PRINT '-- DROP ALERT : {0} @';
IF  EXISTS (SELECT [id] FROM msdb.dbo.sysalerts WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_delete_alert @name=N'{0}';
end;
ELSE
BEGIN
    print '-- Alert does not exist : {0}';
END;
" + Line1;
            return string.Format(script, alertName);
        }

        #endregion Public Override Methods

        #region Private Methods

        private string ScriptCreate(StringCollection stringCollection)
        {
            string alertName = SafeSql(AlertName);
            string script = @"
PRINT '-- CREATE ALERT : {0}';
IF  NOT EXISTS (SELECT [id] FROM msdb.dbo.sysalerts WHERE name = N'{0}')
BEGIN
    {1}
END
ELSE
BEGIN
    print '-- Alert exists and cannot be re-created : {0}';
END;
            ";
            script = string.Format(script, alertName, StringCollectionToString(stringCollection));
            return script;
        }

        #endregion Private Methods
    }
}