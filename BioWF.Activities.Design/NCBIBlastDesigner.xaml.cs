using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace BioWF.Activities.Design
{
    // Interaction logic for NCBIBlastDesigner.xaml
    public partial class NCBIBlastDesigner
    {
        public NCBIBlastDesigner()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ModelItem.Properties["Program"].SetValue(new InArgument<string> { Expression = new Literal<string>(NCBIBlast.DefaultProgram)});
            ModelItem.Properties["Database"].SetValue(new InArgument<string> { Expression = new Literal<string>(NCBIBlast.DefaultDatabase) });
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(NCBIBlast),
                new DesignerAttribute(typeof(NCBIBlastDesigner)),
                new DescriptionAttribute("Send a set of sequences to NCBI BLAST service."),
                new ToolboxBitmapAttribute(typeof(NCBIBlast), @"Images.ncbi_blast.png"));
        } 
    }
}
