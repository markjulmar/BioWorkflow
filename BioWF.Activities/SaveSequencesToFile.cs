using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Bio;
using Bio.IO;

namespace BioWF.Activities
{
    /// <summary>
    /// An activity to save a set of sequences to a file.
    /// </summary>
    public sealed class SaveSequencesToFile : CodeActivity
    {
        /// <summary>
        /// File to write sequences to
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("Filename to write sequences to")]
        public InArgument<string> Filename { get; set; }

        /// <summary>
        /// Sequences to write
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("Sequences to write.")]
        public InArgument<IEnumerable<ISequence>> Sequences { get; set; }

        /// <summary>
        /// Output the filename when run
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Output progress to the log.")]
        public bool LogOutput { get; set; }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override void Execute(CodeActivityContext context)
        {
            string filename = Filename.Get(context);
            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByFileName(filename);
            if (formatter == null)
                throw new ArgumentException("Could not determine formatter for " + filename);

            if (LogOutput)
            {
                var tw = context.GetExtension<TextWriter>() ?? Console.Out;
                tw.WriteLine("Writing sequences to " + filename);
            }

            try
            {
                foreach (var s in Sequences.Get(context))
                {
                    formatter.Format(s);
                }
            }
            finally
            {
                formatter.Close();
            }
        }
    }
}
