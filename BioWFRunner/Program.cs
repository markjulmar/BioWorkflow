using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xaml;

namespace BioWFRunner
{
    /// <summary>
    /// Simple program used to run a workflow from the command line.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            CommandLineParser parser = new CommandLineParser(args);
            string filename = parser.GetDefaultValue();
            if (string.IsNullOrEmpty(filename))
            {
                ShowHelp();
                Console.WriteLine("Missing: filename");
                return;
            }

            if (!File.Exists(filename))
            {
                ShowHelp();
                Console.WriteLine("Could not find {0}", filename);
                return;
            }

            XamlXmlReader reader = new XamlXmlReader(filename, new XamlXmlReaderSettings { LocalAssembly = typeof(Program).Assembly });
            var workflow = ActivityXamlServices.Load(reader);
            Dictionary<string, object> inputs;
            if (!CollectArguments(workflow as DynamicActivity, parser, out inputs))
            {
                return;
            }

            try
            {
                var invoker = new WorkflowInvoker(workflow);
                invoker.Extensions.Add(Console.Out);
                var results = invoker.Invoke(inputs);
                if (results != null)
                {
                    foreach (var item in results)
                    {
                        Console.WriteLine("  {0} = {1}", item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Workflow failed: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static bool CollectArguments(DynamicActivity activity, CommandLineParser parser, out Dictionary<string, object> inputs)
        {
            inputs = new Dictionary<string, object>();

            foreach (var prop in activity.Properties)
            {
                if (!parser.HasValue(prop.Name))
                {
                    ShowHelp();
                    Console.WriteLine("Missing argument " + prop.Name);
                    Console.WriteLine();
                    Console.WriteLine("Workflow needs the following parameters:");
                    foreach (var p in activity.Properties)
                    {
                        Console.WriteLine("{0} - {1}", p.Name, (prop.Type.GetGenericArguments().FirstOrDefault() ?? prop.Type).Name);
                    }
                    return false;
                }
                    
                Type innerType = prop.Type.GetGenericArguments().FirstOrDefault() ?? prop.Type;
                if (innerType == typeof (int))
                {
                    inputs.Add(prop.Name, parser.GetIntValue(prop.Name));
                }
                else if (innerType == typeof(bool))
                {
                    inputs.Add(prop.Name, parser.GetBoolValue(prop.Name));
                }
                else 
                {
                    inputs.Add(prop.Name, parser.GetStringValue(prop.Name));
                }
            }

            return true;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("BioWFRunner 1.0");
            Console.WriteLine("   Usage: BioWFRunner [filename] opt1=value opt2=value opt3=value");
        }
    }
}
