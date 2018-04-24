using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Bio;
using Bio.IO;

namespace BioWF.Activities
{
    /// <summary>
    /// A code activity to parse a set of sequences from a file.
    /// </summary>
    public sealed class LoadSequencesFromFile : CodeActivity<IEnumerable<ISequence>>
    {
        /// <summary>
        /// File to read sequences from
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The filename to parse the data from.")]
        public InArgument<string> Filename { get; set; }

        /// <summary>
        /// Optional alphabet to use when reading
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Optional alphabet to use when reading from file.")]
        public string DesiredAlphabet { get; set; }

        /// <summary>
        /// Output the filename when run
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Output progress to the log window.")]
        public bool LogOutput { get; set; }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override IEnumerable<ISequence> Execute(CodeActivityContext context)
        {
            string filename = Filename.Get(context);
            ISequenceParser parser = SequenceParsers.FindParserByFileName(filename);
            if (parser == null)
                throw new ArgumentException("Could not determine parser for " + filename);

            string alphaName = DesiredAlphabet;
            if (!string.IsNullOrEmpty(alphaName))
            {
                alphaName = alphaName.ToLowerInvariant();
                IAlphabet alphabet = Alphabets.All.FirstOrDefault(a => a.Name.ToLowerInvariant() == alphaName);
                if (alphabet == null)
                    throw new ArgumentException("Unknown alphabet name");

                parser.Alphabet = alphabet;
            }

            if (LogOutput)
            {
                var tw = context.GetExtension<TextWriter>() ?? Console.Out;
                tw.WriteLine("Reading sequences from " + filename);
            }

            return parser.Parse();
        }
    }
}
