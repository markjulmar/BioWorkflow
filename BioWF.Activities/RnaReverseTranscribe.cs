using System.Activities;
using System.ComponentModel;
using Bio;
using Bio.Algorithms.Translation;

namespace BioWF.Activities
{
    /// <summary>
    /// Given a RNA sequence, this activity produces the reverse-transcripted DNA sequence.
    /// </summary>
    public sealed class RnaReverseTranscribe : CodeActivity<ISequence>
    {
        /// <summary>
        /// The RNA sequence to transcribe.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The RNA sequence to transcribe to DNA.")]
        public InArgument<ISequence> RnaInput { get; set; }

        /// <summary>
        /// The execution method for the activity.
        /// </summary>
        protected override ISequence Execute(CodeActivityContext context)
        {
            return Transcription.ReverseTranscribe(RnaInput.Get(context));
        }
    }
}
