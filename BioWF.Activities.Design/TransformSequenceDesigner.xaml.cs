using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.Drawing;

namespace BioWF.Activities.Design
{
    // Interaction logic for TransformSequenceDesigner.xaml
    public partial class TransformSequenceDesigner
    {
        public TransformSequenceDesigner()
        {
            InitializeComponent();
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(TransformSequence),
                new DesignerAttribute(typeof(TransformSequenceDesigner)),
                new DescriptionAttribute("Reverse and/or complement a sequence."),
                new ToolboxBitmapAttribute(typeof(TransformSequence), @"Images.transform_sequence.png"));
        } 
    }
}
