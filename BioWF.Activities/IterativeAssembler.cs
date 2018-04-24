using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bio;
using Bio.Algorithms.Alignment;
using Bio.Algorithms.Assembly;
using Bio.SimilarityMatrices;

namespace BioWF.Activities
{
    /// <summary>
    /// This activity creates an assembly object of the contigs and a consensus from running an assembly algorithm.
    /// </summary>
    public class IterativeAssembler : CodeActivity<IDeNovoAssembly>
    {
        #region Constants
        public const int DefaultMatchScore = 1;
        public const int DefaultMismatchScore = -8;
        public const string DefaultAligner = "Smith-Waterman";
        public const string DefaultMatrix = "DiagonalScoreMatrix";
        public const int DefaultGapOpenCost = -8;
        public const double DefaultMergeThreshold = 4;
        #endregion

        /// <summary>
        /// A list of sequences to assemble.
        /// </summary>
        [RequiredArgument]
        [Description("Sequences to assemble.")]
        public InArgument<IEnumerable<ISequence>> Sequences { get; set; }

        /// <summary>
        /// Match score used when using diagonal match/mismatch matrix
        /// </summary>
        [DefaultValue(DefaultMatchScore)]
        [Description("Match score used when using diagonal match/mismatch matrix")]
        public int MatchScore { get; set; }

        /// <summary>
        /// Mismatch score used when using diagonal match/mismatch matrix
        /// </summary>
        [DefaultValue(DefaultMismatchScore)]
        [Description("Mismatch score used when using diagonal match/mismatch matrix")]
        public int MismatchScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether standard orientation is assumed.
        /// if true, assume that the input sequences are in 5'-to-3' orientation.
        /// This means that only normal and reverse-complement overlaps need to be tested.
        /// if false, need to try both orientations for overlaps.
        /// </summary>
        [Description("Assume input sequences are in 5' to 3' orientation so only normal and reverse-complement overlaps need to be tested.")]
        [DefaultValue(true)]
        public bool AssumeStandardOrientation { get; set; }

        /// <summary>
        /// Overlap required for two sequences to be merged during alignment.
        /// </summary>
        [RequiredArgument]
        [DefaultValue(DefaultMergeThreshold)]
        [Description("Overlap required for two sequences to be merged during alignment")]
        public double MergeThreshold { get; set; }

        /// <summary>
        /// Alignment algorithm to use during assembly
        /// </summary>
        [RequiredArgument]
        [DefaultValue(DefaultAligner)]
        [Category(ActivityConstants.InputGroup)]
        [Description("Name of the aligner to use.")]
        public string AlignerName { get; set; }

        /// <summary>
        /// Similarity matrix to use during alignment
        /// </summary>
        [DefaultValue(DefaultMatrix)]
        [Category(ActivityConstants.InputGroup)]
        [Description("Similarity Matrix to use for comparison.")]
        public string SimilarityMatrix { get; set; }

        /// <summary>
        /// Gap open cost for alignment
        /// </summary>
        [RequiredArgument]
        [DefaultValue(DefaultGapOpenCost)]
        [Category(ActivityConstants.InputGroup)]
        [Description("Gap open cost.")]
        public int GapOpenCost { get; set; }

        /// <summary>
        /// Gap extension cost for alignment
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("Gap extension cost.")]
        public int GapExtensionCost { get; set; }

        /// <summary>
        /// A list of contigs produced by the assembly algorithm.
        /// </summary>
        [Description(@"A list of contigs produced by the assembly algorithm.")]
        public OutArgument<IList<Contig>> Contigs { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public IterativeAssembler()
        {
            MatchScore = DefaultMatchScore;
            MismatchScore = DefaultMismatchScore;
            AlignerName = DefaultAligner;
            GapOpenCost = DefaultGapOpenCost;
            SimilarityMatrix = DefaultMatrix;
            MergeThreshold = DefaultMergeThreshold;
            AssumeStandardOrientation = true;
        }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <returns>
        /// The result of the activity’s execution.
        /// </returns>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override IDeNovoAssembly Execute(CodeActivityContext context)
        {
            // Setup the aligner to use
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

            // Create the assembler
            OverlapDeNovoAssembler assembler = new OverlapDeNovoAssembler
            {
                AssumeStandardOrientation = this.AssumeStandardOrientation,
                MergeThreshold = this.MergeThreshold,
                OverlapAlgorithm = aligner
            };

            var consensus = assembler.Assemble(Sequences.Get(context));
            Contigs.Set(context, ((IOverlapDeNovoAssembly)consensus).Contigs);

            return consensus;
        }
    }
}