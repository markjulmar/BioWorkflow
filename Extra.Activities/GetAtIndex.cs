using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MiscWF.Activities
{
    /// <summary>
    /// Retrieve a specific sequence item by index
    /// </summary>
    [Description("Retrieves a specific index for an IEnumerable or IList.")]
    [DesignerCategory("Collection")]
    public sealed class GetAtIndex<T> : CodeActivity<T>
    {
        /// <summary>
        /// Index to retrieve
        /// </summary>
        [Category(ActivityConstants.InputGroup)]
        [Description("The index to retrieve.")]
        public InArgument<int> Index { get; set; }

        /// <summary>
        /// Index to retrieve
        /// </summary>
        [RequiredArgument]
        [Category(ActivityConstants.InputGroup)]
        [Description("The sequence to pull an item from.")]
        public InArgument<IEnumerable<T>> Enumerator { get; set; }

        /// <summary>
        /// When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name="context">The execution context in which the activity executes.</param>
        protected override T Execute(CodeActivityContext context)
        {
            IEnumerable<T> e = Enumerator.Get(context);
            int index = Index.Get(context);
            IList<T> tryList = e as IList<T>;
            
            return tryList != null 
                ? tryList[index] 
                : e.Skip(index).FirstOrDefault();
        }
    }
}
