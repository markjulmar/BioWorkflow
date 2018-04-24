using System;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;

namespace BioWF.Activities.Design
{
    // Interaction logic for AlignSequencePairDesigner.xaml
    public partial class AlignSequencePairDesigner
    {
        public AlignSequencePairDesigner()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            cbAligners.ItemsSource = SequenceAligners.All.Select(sa => sa.Name);
            cbMatrices.ItemsSource = Enum.GetNames(typeof (SimilarityMatrix.StandardSimilarityMatrix));
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(AlignSequencePair),
                new DesignerAttribute(typeof(AlignSequencePairDesigner)),
                new DescriptionAttribute("Align two sequences against each other and return the consensus."),
                new ToolboxBitmapAttribute(typeof(AlignSequencePair), @"Images.pair_align.png"));
        } 
    }
}
