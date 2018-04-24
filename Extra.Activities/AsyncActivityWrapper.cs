using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Activities;
using MiscWF.Activities.Designers;

namespace MiscWF.Activities
{
    /// <summary>
    /// Creates a truly async activity from a synchronous activity.
    /// http://geekswithblogs.net/DavidPaquette/archive/2010/10/26/wf4-asynchronous-workflows.aspx
    /// </summary>
    [Designer(typeof(AsyncActivityWrapperDesigner))]
    [DesignerCategory("Runtime")]
    public class AsyncActivityWrapper : AsyncCodeActivity
    {
        /// <summary>
        /// The activity to execute on another thread.
        /// </summary>
        [Browsable(false)]
        public ActivityAction Body { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncActivityWrapper()
        {
            Body = new ActivityAction();
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow. 
        /// </summary>
        /// <returns>
        /// The object that saves variable information for an instance of an asynchronous activity.
        /// </returns>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="callback">The method to be called after the asynchronous activity and completion notification have occurred.</param><param name="state">An object that saves variable information for an instance of an asynchronous activity.</param>
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            Task task = Task.Factory.StartNew(_ => 
                WorkflowInvoker.Invoke(CreateDynamicActivity(context), 
                                       GetArgumentsAndVariables(context)), state);
            task.ContinueWith(tr => callback(tr));
            
            return task;
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="result">The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends.</param>
        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
        }

        /// <summary>
        /// Creates a DynamicActivityProperty for each argument and variable in the current context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Activity CreateDynamicActivity(AsyncCodeActivityContext context)
        {
            DynamicActivity result = new DynamicActivity();
            foreach (DynamicActivityProperty dap in
                context.DataContext.GetProperties()
                    .Cast<PropertyDescriptor>()
                    .Select(property => new DynamicActivityProperty
                    {
                        Name = property.Name,
                        Type = typeof (InArgument<>).MakeGenericType(property.PropertyType)
                    }))
            {
                dap.Value = Activator.CreateInstance(dap.Type);
                result.Properties.Add(dap);
            }

            VisualBasic.SetSettings(result, VisualBasic.GetSettings(this));
            result.Implementation = () => Body.Handler;
            return result;
        }

        /// <summary>
        /// Returns a dictionary of all the variables and arguments in scope.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IDictionary<string, object> GetArgumentsAndVariables(AsyncCodeActivityContext context)
        {
            return context.DataContext.GetProperties()
                .Cast<PropertyDescriptor>()
                .ToDictionary(property => property.Name, property => property.GetValue(context.DataContext));
        }
    }
}