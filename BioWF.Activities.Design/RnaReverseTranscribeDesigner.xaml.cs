using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;

namespace BioWF.Activities.Design
{
    // Interaction logic for RnaReverseTranscribeDesigner.xaml
    public partial class RnaReverseTranscribeDesigner
    {
        public RnaReverseTranscribeDesigner()
        {
            InitializeComponent();
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(RnaReverseTranscribe),
                new DesignerAttribute(typeof(RnaReverseTranscribeDesigner)),
                new DescriptionAttribute("Reverse Transcribes a RNA sequence into DNA."),
                new ToolboxBitmapAttribute(typeof(RnaReverseTranscribe), @"Images.rna_transcribe.png"));
        } 
    }
}
