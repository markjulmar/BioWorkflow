using System.ComponentModel;
using Bio;
using Bio.Algorithms.Alignment;
using System;
using System.Activities;
using System.Linq;
using Bio.SimilarityMatrices;

namespace BioWF.Activities
{
    /// <summary>
    /// Simple activity to perform a pairwise alignment
    /// </summary>
    public sealed class AlignSequencePair : CodeActivity<ISequenceAlignment>
    {
        #region Constants
        public const string DefaultAligner = "Smith-Waterman";
        public const string DefaultMatrix = "DiagonalScoreMatrix";
        public const int DefaultGapOpenCost = -8;
        #endregion

        /// <summary>
        /// First sequence to align.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("First sequence to align.")]
        public InArgument<ISequence> FirstSequence { get; set; }

        /// <summary>
        /// Second sequence to align.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("Second sequence to align.")]
        public InArgument<ISequence> SecondSequence { get; set; }

        /// <summary>
        /// The aligner algorithm name to use.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(DefaultAligner)]
        [Category(ActivityConstants.InputGroup)]
        [Description("Name of the aligner to use.")]
        public string AlignerName { get; set; }

        /// <summary>
        /// Similarity matrix to use during alignment.
        /// </summary>
        [DefaultValue(DefaultMatrix)]
        [Category(ActivityConstants.InputGroup)]
        [Description("Similarity Matrix to use for comparison.")]
        public string SimilarityMatrix { get; set; }

        /// <summary>
        /// Gap open cost to use during alignment.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(DefaultGapOpenCost)]
        [Category(ActivityConstants.InputGroup)]
        [Description("Gap open cost.")]
        public int GapOpenCost { get; set; }

        /// <summary>
        /// Gap extension cost to use during alignment.
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Gap extension cost.")]
        public int GapExtensionCost { get; set; }

        /// <summary>
        /// Consensus generated from alignment.
        /// </summary>
        [Category(ActivityConstants.OutputGroup)]
        public OutArgument<ISequence> Consensus { get; set; }

        /// <summary>
        /// First sequence result from alignment.
        /// </summary>
        [Category(ActivityConstants.OutputGroup)]
        public OutArgument<ISequence> FirstResult { get; set; }

        /// <summary>
        /// Second sequence result from alignment.
        /// </summary>
        [Category(ActivityConstants.OutputGroup)]
        public OutArgument<ISequence> SecondResult { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AlignSequencePair()
        {
            GapOpenCost = DefaultGapOpenCost;
            SimilarityMatrix = DefaultMatrix;
            AlignerName = DefaultAligner;
        }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <returns>
        /// The result of the activity’s execution.
        /// </returns>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override ISequenceAlignment Execute(CodeActivityContext context)
        {
            string alignerName = (AlignerName ?? DefaultAligner).ToLowerInvariant();
            var aligner = SequenceAligners.All.FirstOrDefault(sa => sa.Name.ToLowerInvariant() == alignerName);
            if (aligner == null)
                throw new ArgumentException("Could not find aligner: " + alignerName);

            aligner.GapOpenCost = GapOpenCost;
            aligner.GapExtensionCost = GapExtensionCost;

            var smName = SimilarityMatrix ?? DefaultMatrix;
            SimilarityMatrix.StandardSimilarityMatrix sm;
            if (Enum.TryParse(smName, true, out sm))
            {
                aligner.SimilarityMatrix = new SimilarityMatrix(sm);
            }

            ISequenceAlignment result;
            if (GapOpenCost == GapExtensionCost || GapExtensionCost == 0)
            {
                result = aligner.AlignSimple(new[] {FirstSequence.Get(context), SecondSequence.Get(context)}).First();
            }
            else
            {
                result = aligner.Align(new[] { FirstSequence.Get(context), SecondSequence.Get(context) }).First();
            }

            IPairwiseSequenceAlignment pwAlignment = result as IPairwiseSequenceAlignment;
            if (pwAlignment != null)
            {
                if (pwAlignment.PairwiseAlignedSequences.Count > 0)
                {
                    FirstResult.Set(context, pwAlignment.PairwiseAlignedSequences[0].FirstSequence);
                    SecondResult.Set(context, pwAlignment.PairwiseAlignedSequences[0].SecondSequence);
                    Consensus.Set(context, pwAlignment.PairwiseAlignedSequences[0].Consensus);
                }
            }

            return result;
        }
    }
}
