using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;

namespace BioWF.Activities.Design
{
    // Interaction logic for ProteinTranslateDesigner.xaml
    public partial class ProteinTranslateDesigner
    {
        public ProteinTranslateDesigner()
        {
            InitializeComponent();
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(ProteinTranslate),
                new DesignerAttribute(typeof(ProteinTranslateDesigner)),
                new DescriptionAttribute("Translate RNA sequence into Protein sequence"),
                new ToolboxBitmapAttribute(typeof(ProteinTranslate), @"Images.protein_translate.png"));
        } 
    }
}
