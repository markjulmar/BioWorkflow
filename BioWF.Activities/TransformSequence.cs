using System.Activities;
using System.ComponentModel;
using Bio;

namespace BioWF.Activities
{
    /// <summary>
    /// This provides common transformations to a given sequence - reverse and complement.
    /// </summary>
    public class TransformSequence : CodeActivity<ISequence>
    {
        /// <summary>
        /// The sequence to perform the transformation on.
        /// </summary>
        [RequiredArgument]
        [Description("The sequence to perform the transformation on")]
        public InArgument<ISequence> Sequence { get; set; }
        
        /// <summary>
        /// Whether to reverse the sequence on end.
        /// </summary>
        [DefaultValue(true)]
        [Description("Reverse the sequence 3' to 5'")]
        public bool Reverse { get; set; }

        /// <summary>
        /// Whether to complement the sequence
        /// </summary>
        [DefaultValue(true)]
        [Description("Provide the complement to the sequence")]
        public bool Complement { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TransformSequence()
        {
            Reverse = true;
            Complement = true;
        }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <returns>
        /// The result of the activity’s execution.
        /// </returns>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override ISequence Execute(CodeActivityContext context)
        {
            return new DerivedSequence(Sequence.Get(context), Reverse, Complement);
        }
    }
}
