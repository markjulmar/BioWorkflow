using System;
using System.Diagnostics;
using System.IO;
using System.Activities;

namespace MiscWF.Activities
{
    public sealed class ShellExec : AsyncCodeActivity<int>
    {
        private Process _process;
        public InArgument<string> Filename { get; set; }
        public InArgument<string> Arguments { get; set; }
        public bool UseShellExec { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            string fn = Filename.Get(context);
            if (string.IsNullOrWhiteSpace(fn))
                throw new ArgumentNullException("Filename", "Missing filename to execute.");

            string args = Arguments.Get(context);
            TextWriter tw = context.GetExtension<TextWriter>() ?? Console.Out;
            
            Func<int> exec = () =>
            {
                ProcessStartInfo psi = new ProcessStartInfo(fn)
                {
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = this.UseShellExec,
                };

                if (!string.IsNullOrEmpty(args))
                    psi.Arguments = args;

                _process = Process.Start(psi);
                _process.OutputDataReceived += (sender, eventArgs) => tw.WriteLine(eventArgs.Data);
                _process.ErrorDataReceived += (sender, eventArgs) => tw.WriteLine(eventArgs.Data);
                _process.WaitForExit();

                int rc = _process.ExitCode;
                _process = null;
                return rc;
            };

            context.UserState = exec;
            return exec.BeginInvoke(callback, state);
        }

        protected override int EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Func<int> exec = (Func<int>) context.UserState;
            return exec.EndInvoke(result);
        }

        protected override void Cancel(AsyncCodeActivityContext context)
        {
            if (_process != null)
            {
                _process.Kill();
            }

            base.Cancel(context);
        }
    }
}
