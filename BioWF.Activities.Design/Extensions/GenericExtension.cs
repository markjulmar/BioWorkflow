using System;
using System.Linq;
using System.Windows.Markup;

namespace BioWF.Activities.Design.Extensions
{
    public class GenericExtension : MarkupExtension
    {
        public Type BaseType { get; set; }

        public Type InnerType
        {
            get { return InnerTypes.FirstOrDefault(); }
            set { InnerTypes = new[] {value}; }
        }
        
        public Type[] InnerTypes { get; set; }

        public GenericExtension()
        {
        }

        public GenericExtension(Type type, Type innerType)
        {
            BaseType = type;
            InnerTypes = new[] {innerType};
        }

        public GenericExtension(Type type, params Type[] innerTypes)
        {
            BaseType = type;
            InnerTypes = innerTypes;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return BaseType.MakeGenericType(InnerTypes);
        }
    }
}