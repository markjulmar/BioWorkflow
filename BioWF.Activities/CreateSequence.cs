using System;
using System.Activities;
using System.ComponentModel;
using System.Linq;
using Bio;

namespace BioWF.Activities
{
    /// <summary>
    /// This activity creates a new sequence using a given alphabet and set of values.
    /// </summary>
    public sealed class CreateSequence : CodeActivity<ISequence>
    {
        public const string DefaultAlphabet = "Dna";

        /// <summary>
        /// The ID of the sequence
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Optional unique identifier for sequence.")]
        public string ID { get; set; }

        /// <summary>
        /// The alphabet of the sequence.
        /// </summary>
        [DefaultValue(DefaultAlphabet)]
        [Category(ActivityConstants.InputGroup)]
        [Description("The alphabet for the sequence - DNA, RNA, Protein, etc.  Defaults to DNA.")]
        public string AlphabetName { get; set; }

        /// <summary>
        /// The characters making up the sequence
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The string sequence values expressed in the alphabet.")]
        public InArgument<string> SequenceData { get; set; }

        public CreateSequence()
        {
            AlphabetName = DefaultAlphabet;
        }

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        protected override ISequence Execute(CodeActivityContext context)
        {
            string alphaName = (AlphabetName ?? DefaultAlphabet).ToLowerInvariant();
            IAlphabet alphabet = Alphabets.All.FirstOrDefault(a => a.Name.ToLowerInvariant() == alphaName);
            if (alphabet == null)
                throw new ArgumentException("Unknown alphabet name");

            // Generate the sequence
            return new Sequence(alphabet, SequenceData.Get(context)) { ID = this.ID };
        }
    }
}
