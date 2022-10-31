namespace Script
{
    /// <summary>
    /// Command line - Arguments parser
    /// Created using VisualStudio 2013
    /// http://www.codeproject.com/Articles/3111/C-NET-Command-Line-Arguments-Parser
    /// </summary>
    public class Arguments
    {
        #region Private Members

        private ArgumentType _defaultParameterNull = new ArgumentType();
        private System.Collections.Generic.List<ArgumentType> _parameters;

        #endregion Private Members

        #region Public Constructors

        public Arguments(System.Collections.Generic.IEnumerable<string> args)
        {
            //_parameters = new System.Collections.Specialized.StringDictionary();
            _parameters = new System.Collections.Generic.List<ArgumentType>();
            System.Text.RegularExpressions.Regex spliter = new System.Text.RegularExpressions.Regex(@"^-{1,2}|^/|=|:", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

            System.Text.RegularExpressions.Regex remover = new System.Text.RegularExpressions.Regex(@"^['""]?(.*?)['""]?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

            string parameter = null;
            string[] parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples:
            // -param1 value1 --param2 /param3:"Test-:-work"
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                        // Found a value (for the last parameter
                        // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            //if (!_parameters.ContainsKey(parameter))
                            if (!Parameters.Exists(p => System.String.Equals(p.Name, parameter, System.StringComparison.CurrentCultureIgnoreCase)))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");

                                //_parameters.Add(parameter, parts[0]);
                                Parameters.Add(new ArgumentType(parameter, parts[0]));

                            }
                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;

                        // Found just a parameter
                    case 2:
                        // The last parameter is still waiting.
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!Parameters.Exists(p => System.String.Equals(p.Name, parameter, System.StringComparison.CurrentCultureIgnoreCase)))
                            {
                                //_parameters.Add(parameter, "true");
                                Parameters.Add(new ArgumentType(parameter, "true"));
                            }
                        }
                        parameter = parts[1];
                        break;

                        // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting.
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!Parameters.Exists(p => System.String.Equals(p.Name, parameter, System.StringComparison.CurrentCultureIgnoreCase)))
                            {
                                //_parameters.Add(parameter, "true");
                                Parameters.Add(new ArgumentType(parameter, "true"));
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        //if (!_parameters.ContainsKey(parameter))
                        if (!Parameters.Exists(p => System.String.Equals(p.Name, parameter, System.StringComparison.CurrentCultureIgnoreCase)))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            //_parameters.Add(parameter, parts[2]);
                            Parameters.Add(new ArgumentType(parameter, parts[2]));
                        }

                        parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (parameter != null)
            {
                //if (!_parameters.ContainsKey(parameter))
                //    _parameters.Add(parameter, "true");
                if (!Parameters.Exists(p => System.String.Equals(p.Name, parameter, System.StringComparison.CurrentCultureIgnoreCase)))
                {
                    Parameters.Add(new ArgumentType(parameter, "true"));
                }
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public System.Collections.Generic.List<ArgumentType> Parameters
        {
            get { return _parameters; }
        }

        #endregion Public Properties

        #region Public Indexers

        // Retrieve a parameter value if it exists
        // (overriding C# indexer property)
        public ArgumentType this[string param]
        {
            get
            {
                ArgumentType ret = Parameters.Find(p => System.String.Equals(p.Name, param, System.StringComparison.CurrentCultureIgnoreCase));
                if (ret == null)
                {
                    ret = _defaultParameterNull;
                    _defaultParameterNull.Name = param;
                    _defaultParameterNull.Value = "";
                    _defaultParameterNull.Defined = false;
                    _defaultParameterNull.HasValue = false;
                }
                return (ret);
            }
        }

        #endregion Public Indexers
    }

    public class ArgumentType
    {
        #region Private Members

        private string _name;
        private string _value;

        #endregion Private Members

        #region Public Constructors

        public ArgumentType()
        {
            Defined = false;
            HasValue = false;
        }

        public ArgumentType(string name, string value)
        {
            Defined = false;
            HasValue = false;
            Name = name;
            if (!string.IsNullOrEmpty(value))
            {
                Value = value;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public bool Defined
        {
            get; internal set;
        }

        public bool HasValue
        {
            get; internal set;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Defined = true;
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                HasValue = true;
            }
        }

        public bool ValueBool
        {
            get {
                try
                {
                    return System.Convert.ToBoolean(_value);
                }
                catch
                {
                    return false;
                }
            }
        }

        public int ValueInt
        {
            get
            {
                try
                {
                    return System.Convert.ToInt32(_value);
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion Public Properties
    }
}