using System;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;

namespace BioWF.Activities.Design
{
    // Interaction logic for IterativeAssemblerDesigner.xaml
    public partial class IterativeAssemblerDesigner
    {
        public IterativeAssemblerDesigner()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            cbAligners.ItemsSource = SequenceAligners.All.Select(sa => sa.Name);
            cbMatrices.ItemsSource = Enum.GetNames(typeof(SimilarityMatrix.StandardSimilarityMatrix));

            cbMatrices.SelectionChanged += CbMatricesOnSelectionChanged;
        }

        private void CbMatricesOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = (string) cbMatrices.SelectedItem;
            if (string.Compare("DiagonalSimilarityMatrix", name, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                tbMatchScore.IsEnabled = false;
                tbMismatchScore.IsEnabled = false;
            }
            else
            {
                tbMatchScore.IsEnabled = true;
                tbMismatchScore.IsEnabled = true;
            }
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(IterativeAssembler),
                new DesignerAttribute(typeof(IterativeAssemblerDesigner)),
                new DescriptionAttribute("Given a list of input sequences, this activity creates an assembly object of the contigs " +
                 "and a consensus from running a simple sequence assembler algorithm."),
                new ToolboxBitmapAttribute(typeof(IterativeAssembler), @"Images.assembly.png"));
        }
    }
}
