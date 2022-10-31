using System.Collections.Specialized;

using Microsoft.SqlServer.Management.Smo;

namespace Script
{
    class UserDefinedMessageScript  : SqlScript
    {
        #region Public Override Methods

        public override  string ScriptCreate()
        {
            return _ScriptCreate;
        }

        public override string ScriptDrop()
        {
            string sql = @"
IF (EXISTS (
	SELECT  M.message_id, M.language_id, M.severity, M.is_event_logged, M.text
	FROM    sys.messages AS M INNER JOIN sys.syslanguages AS L ON M.language_id = L.lcid
	WHERE     (M.message_id = {0}) AND (M.text = '{3}') AND (L.name = '{1}')
))
BEGIN
	PRINT '   Drop UserDefinedmessage {0} ({1}) : ' + '{3}';
    EXEC sp_dropmessage @msgnum = {0}, @lang = '{1}'; 
END;
";
            sql = string.Format(sql, _userDefinedMessage.ID, _userDefinedMessage.Language, _userDefinedMessage.Severity, _userDefinedMessage.Text.Replace("'", "''"), (_userDefinedMessage.IsLogged ? "TRUE" : "FALSE"));
            return sql;
        }

        #endregion Public Override Methods

        public UserDefinedMessageScript()
        {
        }

        private UserDefinedMessage _userDefinedMessage = null;
        private int _msgnum;
        private string _ScriptCreate;
        public UserDefinedMessageScript(UserDefinedMessage userDefinedMessage)
        {
            _userDefinedMessage = userDefinedMessage;
            MessageID = userDefinedMessage.ID;
            MessageName = string.Format("UserDefinedMessage-{0}({1})", userDefinedMessage.ID, userDefinedMessage.Language);
            _ScriptCreate = string.Format(SqlScriptUpdateMessage(userDefinedMessage), userDefinedMessage.ID, userDefinedMessage.Language, userDefinedMessage.Severity, userDefinedMessage.Text.Replace("'", "''"), (userDefinedMessage.IsLogged ? "TRUE" : "FALSE"));
        }

        private string SqlScriptUpdateMessage(UserDefinedMessage message)
        {
            string sql = @"
IF (EXISTS (
	SELECT  M.message_id, M.language_id, M.severity, M.is_event_logged, M.text
	FROM    sys.messages AS M INNER JOIN sys.syslanguages AS L ON M.language_id = L.lcid
	WHERE     (M.message_id = {0}) AND (M.text = '{3}') AND (L.name = '{1}')
))
BEGIN
	PRINT '   UserDefinedmessage exists {0} ({1}) : ' + '{3}';
END
ELSE
BEGIN
	IF (EXISTS (
	SELECT  M.message_id, M.language_id, M.severity, M.is_event_logged, M.text
	FROM    sys.messages AS M INNER JOIN sys.syslanguages AS L ON M.language_id = L.lcid
	WHERE     (M.message_id = {0}) AND (L.name = '{1}')	
	))
	BEGIN
		PRINT '-- Update userDefinedMessage {0} ({1}) : ' + '{3}';
		EXEC sp_dropmessage @msgnum = {0}, @lang = '{1}';
	END
	PRINT '-- Add new userDefinedMessage {0} ({1}) : ' + '{3}';
	{5};
END;";
            string script = StringCollectionToString(message.Script());
            sql = string.Format(sql, message.ID, message.Language, message.Severity, message.Text.Replace("'", "''"), (message.IsLogged ? "TRUE" : "FALSE"), script);
            return sql;
        }

        public int MessageID { get; set; }

        public string MessageName { get; set; }

    }
}