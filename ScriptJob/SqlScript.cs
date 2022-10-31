using System.Collections.Specialized;

using Microsoft.SqlServer.Management.Smo;

namespace Script
{
    abstract class SqlScript
    {

        protected readonly ScriptingOptions _scriptingOptions = new ScriptingOptions();

        #region Private Constant Fields

        protected const string Line1 = "--  **********************************************************\r\n";
        protected const string Line2 = "--  --\r\n";

        #endregion Private Constant Fields

        public virtual string ScriptCreate()
        {
            return "";
        }
        public virtual string ScriptDisable()
        {
            return "";
        }
        public virtual string ScriptDrop()
        {
            return "";
        }
        public virtual string ScriptEnable()
        {
            return "";
        }
        /// <summary>
        /// Update ou copy a configuration
        /// </summary>
        /// <returns></returns>
        public virtual string ScriptUpdate()
        {
            return "";  
        }

        public string StringCollectionToString(StringCollection stringCollection)
        {
            string script = "";
            for (int i = 0; i < stringCollection.Count; ++i)
            {
                script += stringCollection[i];
                script += "\r\n;";
                script += Line2;
            }
            if (string.IsNullOrEmpty(script))
            {
                script = "PRINT '-- ** Script not found ** --';";
            }
            return script;
        }

        public string SafeSql(string text)
        {
            return text.Replace("'", "''");
        }

    }
}
