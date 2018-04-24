using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace MiscWF.Activities
{
    /// <summary>
    /// Reads a line from the console.
    /// </summary>
    [Description("Read a line from the console.")]
    [DesignerCategory("Standard")]
    public sealed class ReadLine : AsyncCodeActivity<string>
    {
        /// <summary>
        /// When implemented in a derived class and using the specified execution context, callback method, and user state, enqueues an asynchronous activity in a run-time workflow. 
        /// </summary>
        /// <returns>
        /// An object.
        /// </returns>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="callback">The method to be called after the asynchronous activity and completion notification have occurred.</param><param name="state">An object that saves variable information for an instance of an asynchronous activity.</param>
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            TextReader textReader = context.GetExtension<TextReader>() ?? Console.In;
            if (textReader == null)
            {
                throw new Exception("Missing TextReader extension for ReadLine activity.");
            }

            var tcs = new TaskCompletionSource<string>(state);
            Task<string> task = textReader.ReadLineAsync();
            task.ContinueWith(tr =>
            {
                // Copy the Task.Result into the Task WF sees.
                if (tr.IsFaulted)
                    tcs.TrySetException(tr.Exception.InnerExceptions);
                else if (tr.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(tr.Result);

                // Invoke the callback so WF sees it.
                if (callback != null)
                    callback(tcs.Task);
            });

            return tcs.Task;
        }

        /// <summary>
        /// When implemented in a derived class and using the specified execution environment information, notifies the workflow runtime that the associated asynchronous activity operation has completed.
        /// </summary>
        /// <returns>
        /// A generic type.
        /// </returns>
        /// <param name="context">Information that defines the execution environment for the <see cref="T:System.Activities.AsyncCodeActivity"/>.</param><param name="result">The implemented <see cref="T:System.IAsyncResult"/> that returns the status of an asynchronous activity when execution ends.</param>
        protected override string EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            try
            {
                return ((Task<string>) result).Result;
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}
