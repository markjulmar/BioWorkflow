using System.Activities.Presentation.Metadata;

namespace BioWF.Activities.Design
{
    public sealed class RegisterMetadata : IRegisterMetadata
    {
        public void Register()
        {
            RegisterAll();
        }

        public static void RegisterAll()
        {
            var builder = new AttributeTableBuilder();
            
            // Register each type
            CreateSequenceDesigner.RegisterMetadata(builder);
            LoadSequencesFromFileDesigner.RegisterMetadata(builder);
            DnaTranscriptionDesigner.RegisterMetadata(builder);
            RnaReverseTranscribeDesigner.RegisterMetadata(builder);
            SaveSequencesToFileDesigner.RegisterMetadata(builder);
            ProteinTranslateDesigner.RegisterMetadata(builder);
            NCBIBlastDesigner.RegisterMetadata(builder);
            AlignSequencePairDesigner.RegisterMetadata(builder);
            IterativeAssemblerDesigner.RegisterMetadata(builder);
            TransformSequenceDesigner.RegisterMetadata(builder);
            
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
