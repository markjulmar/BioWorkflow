using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;

namespace BioWF.Activities.Design
{
    // Interaction logic for DnaTranscriptionDesigner.xaml
    public partial class DnaTranscriptionDesigner
    {
        public DnaTranscriptionDesigner()
        {
            InitializeComponent();
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(DnaTranscribe),
                new DesignerAttribute(typeof(DnaTranscriptionDesigner)),
                new DescriptionAttribute("Transcribes a DNA sequence into RNA."),
                new ToolboxBitmapAttribute(typeof(DnaTranscribe), @"Images.dna_transcribe.png"));
        } 
    }
}
