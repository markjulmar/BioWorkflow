using System.Activities;
using System.ComponentModel;
using Bio;
using Bio.Algorithms.Translation;

namespace BioWF.Activities
{
    /// <summary>
    /// Given a DNA sequence, this activity produces the transcripted RNA sequence.
    /// </summary>
    public sealed class DnaTranscribe : CodeActivity<ISequence>
    {
        /// <summary>
        /// The DNA sequence to transcribe.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The DNA sequence to transcribe.")]
        public InArgument<ISequence> DnaInput { get; set; }

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        protected override ISequence Execute(CodeActivityContext context)
        {
            return Transcription.Transcribe(DnaInput.Get(context));
        }
    }
}
