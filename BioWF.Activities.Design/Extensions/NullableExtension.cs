using System;
using System.Windows.Markup;

namespace BioWF.Activities.Design.Extensions
{
    public class NullableExtension : MarkupExtension
    {
        Type InnerType { get; set; }

        public NullableExtension()
        {
        }

        public NullableExtension(Type type)
        {
            InnerType = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return typeof(Nullable<>).MakeGenericType(InnerType);
        }
    }
}
