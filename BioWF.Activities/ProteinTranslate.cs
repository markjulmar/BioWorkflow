using System.Activities;
using System.ComponentModel;
using Bio;
using Bio.Algorithms.Translation;

namespace BioWF.Activities
{
    /// <summary>
    /// Given a RNA sequence, this activity produces the reverse-transcripted DNA sequence.
    /// </summary>
    public sealed class ProteinTranslate : CodeActivity<ISequence>
    {
        /// <summary>
        /// The RNA sequence to transcribe.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The RNA sequence to translate to Protein.")]
        public InArgument<ISequence> RnaInput { get; set; }

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        protected override ISequence Execute(CodeActivityContext context)
        {
            return ProteinTranslation.Translate(RnaInput.Get(context));
        }
    }
}
