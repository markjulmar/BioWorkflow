using System;
using System.Collections.Generic;
using System.Linq;

namespace BioWFRunner
{
    public sealed class CommandLineParser
    {
        private Dictionary<string, string> _args;

        public string GetDefaultValue()
        {
            return _args.FirstOrDefault(kvp => kvp.Value == null 
                && kvp.Key != null 
                && (!kvp.Key.StartsWith("-") 
                && !kvp.Key.StartsWith("/"))).Key;
        }

        public CommandLineParser(string[] commandLine)
        {
            Parse(commandLine);
        }

        public bool HasValue(string argumentName)
        {
            return _args.ContainsKey(argumentName.ToLowerInvariant());
        }

        public bool GetBoolValue(string argumentName, bool defaultValue = false)
        {
            if (_args.ContainsKey(argumentName.ToLowerInvariant()))
            {
                bool val;
                if (Boolean.TryParse(_args[argumentName], out val))
                    return val;
            }
            return defaultValue;
        }

        public int? GetIntValue(string argumentName, int? defaultValue = null)
        {
            if (_args.ContainsKey(argumentName.ToLowerInvariant()))
            {
                int val;
                if (Int32.TryParse(_args[argumentName], out val))
                    return val;
            }
            return defaultValue;
        }

        public string GetStringValue(string argumentName, string defaultValue = null)
        {
            return _args.ContainsKey(argumentName.ToLowerInvariant()) ? _args[argumentName] : defaultValue;
        }

        public void Parse(string[] args)
        {
            _args = new Dictionary<string, string>();
            foreach (string arg in args)
            {
                string[] words = arg.Split('=');
                if (words.Length == 1)
                {
                    _args[words[0].ToLowerInvariant()] = null;
                }
                else
                {
                    _args[words[0].ToLowerInvariant()] = words[1];
                }
            }
        }
    }
}