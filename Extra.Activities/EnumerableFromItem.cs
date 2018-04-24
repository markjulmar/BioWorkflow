using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;

namespace MiscWF.Activities
{
    /// <summary>
    /// Activity to generate a enumerable from an item so it may be passed into other
    /// activities expecting enumerable sequences.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Description("Generates an enumerable from a single item.")]
    [DesignerCategory("Collection")]
    public sealed class EnumerableFromItem<T> : CodeActivity<IEnumerable<T>>
    {
        /// <summary>
        /// The item to turn into a list.
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("Item to turn into an enumerable.")]
        public InArgument<T> Item { get; set; }

        /// <summary>
        /// When implemented in a derived class, performs the execution of the activity.
        /// </summary>
        /// <param name="context">The execution context under which the activity executes.</param>
        protected override IEnumerable<T> Execute(CodeActivityContext context)
        {
            T item = Item.Get(context);
            return new[] {item};
        }
    }
}
