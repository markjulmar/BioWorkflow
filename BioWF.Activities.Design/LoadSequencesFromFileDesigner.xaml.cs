using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Converters;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Model;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using Bio;
using Bio.IO;
using Microsoft.Win32;

namespace BioWF.Activities.Design
{
    // Interaction logic for LoadSequencesFromFileDesigner.xaml
    public partial class LoadSequencesFromFileDesigner
    {
        public LoadSequencesFromFileDesigner()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnFindFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = string.Join("|", SequenceParsers.All.Select(sp =>
                sp.Name + "|*" + sp.SupportedFileTypes.Replace(",", ";*"))) + "|All Files (*.*)|*.*";
            ofd.CheckFileExists = true;
            ofd.Title = "Select sequence file to load";

            if (ofd.ShowDialog() == true)
            {
                ModelProperty item = ModelItem.Properties["Filename"];
                item.SetValue(new InArgument<string> { Expression = new Literal<string>(ofd.FileName) });
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var alphabets = Alphabets.All.Select(a => Tuple.Create(a.Name, a.Name)).ToList();
            alphabets.Insert(0, Tuple.Create<string, string>("(Auto-Detect)", null));
            cbAlphabet.ItemsSource = alphabets;
        }

        public static void RegisterMetadata(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(LoadSequencesFromFile),
                new DesignerAttribute(typeof(LoadSequencesFromFileDesigner)),
                new DescriptionAttribute("Loads sequences from a supported file format."),
                new ToolboxBitmapAttribute(typeof(LoadSequencesFromFile), @"Images.load_sequences.png"));
        } 
    }
}
