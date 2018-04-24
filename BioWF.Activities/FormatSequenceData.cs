using System.Activities;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Bio;

namespace BioWF.Activities
{
    /// <summary>
    /// Formats a sequence into a human-readable string.
    /// </summary>
    [Description("Formats a sequence into a human-readable string")]
    public sealed class FormatSequenceData : CodeActivity<string>
    {
        /// <summary>
        /// The input sequence to process.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The input sequence to process.")]
        public InArgument<ISequence> Sequence { get; set; }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override string Execute(CodeActivityContext context)
        {
            var sequence = Sequence.Get(context);

            StringBuilder buff = new StringBuilder();

            buff.AppendLine(sequence.ID);
            buff.Append("Statistics: ");
            buff.Append(sequence.Count);
            buff.Append(" Total");

            var statistics = new SequenceStatistics(sequence);
            foreach (var symbol in sequence.Alphabet)
            {
                buff.AppendFormat(" - {0}:", (char) symbol);
                buff.Append(statistics.GetCount(symbol));
            }

            buff.AppendLine();
            buff.AppendLine();

            for (int i = 0; i < sequence.Count; i++)
            {
                if ((i % 50) == 0)
                {
                    string num = (i + 1).ToString(CultureInfo.InvariantCulture);
                    int pad = 5 - num.Length;
                    StringBuilder buff2 = new StringBuilder();
                    for (int j = 0; j < pad; j++)
                        buff2.Append(' ');
                    buff2.Append(num);
                    buff.Append(buff2);
                }

                if ((i % 10) == 0)
                    buff.Append(' ');

                buff.Append((char)sequence[i]);

                if ((i % 50) == 49)
                    buff.AppendLine();
            }
            buff.AppendLine();
            
            return buff.ToString();
        }
    }
}
