using System.Collections.Specialized;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Script
{
    class OperatorScript : SqlScript
    {
        #region Private Members

        private string _scriptCreate = "";

        #endregion Private Members

        #region Public Constructors

        public OperatorScript(Microsoft.SqlServer.Management.Smo.Agent.Operator op)
        {
            OperatorName = op.Name;

            StringCollection stringCollection = op.Script(_scriptingOptions);
            _scriptCreate = ScriptCreate(stringCollection);
        }

        #endregion Public Constructors

        #region Public Properties

        public string OperatorName
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
            string operatorName = OperatorName.Replace("'", "''");
            string script = Line1 + @"PRINT '-- DROP OPERATOR : {0}';
IF  EXISTS (SELECT [id] FROM msdb.dbo.sysoperators WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_update_operator @name=N'{0}', @enabled=0;
end;
ELSE
BEGIN
    print '-- Operator does not exist : {0}';
END;
            " + Line1;
            return string.Format(script, operatorName); 
        }

        public override string ScriptDrop()
        {
            string operatorName = OperatorName.Replace("'", "''");
            string script = Line1 + @"
PRINT '-- DROP OPERATOR : {0}';
IF  EXISTS (SELECT [id] FROM msdb.dbo.sysoperators WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_delete_operator @name=N'{0}';
end;
ELSE
BEGIN
    print '-- Operator does not exist : {0}';
END;
            " + Line1;
            return string.Format(script, operatorName); 
        }

        public override string ScriptEnable()
        {
            string operatorName = OperatorName.Replace("'", "''");
            string script = Line1 + @"PRINT '-- DROP OPERATOR : {0}';
IF  EXISTS (SELECT [id] FROM msdb.dbo.sysoperators WHERE name = N'{0}')
begin
    EXEC msdb.dbo.sp_update_operator @name=N'{0}', @enabled=1;
end;
ELSE
BEGIN
    print '-- Operator does not exist : {0}';
END;
            " + Line1;
            return string.Format(script, operatorName); 
        }


        #endregion Public Override Methods

        #region Private Methods

        private string ScriptCreate(StringCollection stringCollection)
        {
            string operatorName = OperatorName.Replace("'", "''");
            string script = Line1 + @"PRINT '-- CREATE OPERATOR : {0}';
IF  NOT EXISTS (SELECT [id] FROM msdb.dbo.sysoperators WHERE name = N'{0}')
BEGIN
    {1}
END
ELSE
BEGIN
    print '-- Operator exists and cannot be re-created : {0}';
END;
            ";
            return string.Format(script, operatorName, StringCollectionToString(stringCollection));
        }

        #endregion Private Methods
    }
}