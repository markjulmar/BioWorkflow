using System;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using Bio;

namespace BioWF.Activities.Design
{
    // Interaction logic for CreateSequenceDesigner.xaml
    public partial class CreateSequenceDesigner
    {
        public CreateSequenceDesigner()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            cbAlphabet.ItemsSource = Alphabets.All.Select(a => a.Name);
            cbAlphabet.SelectedItem = CreateSequence.DefaultAlphabet;
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(CreateSequence),
                new DesignerAttribute(typeof(CreateSequenceDesigner)),
                new DescriptionAttribute("Creates a new sequence using a given alphabet and set of values."),
                new ToolboxBitmapAttribute(typeof(CreateSequence), @"Images.create_sequence.png"));
        } 
    }
}
