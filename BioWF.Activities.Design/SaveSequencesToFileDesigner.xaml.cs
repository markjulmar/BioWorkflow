using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Model;
using System.ComponentModel;
using System.Drawing;
using Bio.IO;
using Microsoft.Win32;
using System.Linq;
using System.Windows;

namespace BioWF.Activities.Design
{
    // Interaction logic for SaveSequencesToFileDesigner.xaml
    public partial class SaveSequencesToFileDesigner
    {
        public SaveSequencesToFileDesigner()
        {
            InitializeComponent();
        }

        private void OnFindFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = string.Join("|", SequenceFormatters.All.Select(sp =>
                sp.Name + "|*" + sp.SupportedFileTypes.Replace(",", ";*"))) + "|All Files (*.*)|*.*";
            ofd.CheckFileExists = true;
            ofd.Title = "Select sequence file to load";

            if (ofd.ShowDialog() == true)
            {
                ModelProperty item = ModelItem.Properties["Filename"];
                item.SetValue(new InArgument<string> { Expression = new Literal<string>(ofd.FileName) });
            }
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(SaveSequencesToFile),
                new DesignerAttribute(typeof(SaveSequencesToFileDesigner)),
                new DescriptionAttribute("Saves a set of sequences to a supported file format."),
                new ToolboxBitmapAttribute(typeof(SaveSequencesToFile), @"Images.save_sequences.png"));
        } 
    }
}
