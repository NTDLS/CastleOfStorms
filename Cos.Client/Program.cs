using System;
using System.Linq;
using System.Windows.Forms;

namespace Cos.Client
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Set up global exception handlers
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);
            //Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(GlobalThreadExceptionHandler);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Screen? screen = null;

            if (args.Any(o => o.Equals("/nosplash", StringComparison.InvariantCultureIgnoreCase)) == false)
            {
                using var formStartup = new FormStartup();
                if (formStartup.ShowDialog() == DialogResult.OK)
                {
                    screen = formStartup.CurrentScreen;
                }
                else
                {
                    return;
                }
            }
            else
            {
                screen = Screen.FromPoint(Cursor.Position);
            }

            try
            {
                Application.Run(new FormRenderTarget(screen));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception occurred:\n {ex?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Global exception handler for unhandled exceptions in non-UI threads
        private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // Log or display the exception information
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show($"An unhandled non-UI exception occurred:\n {ex?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Global exception handler for unhandled exceptions in UI threads
        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Log or display the exception information
            MessageBox.Show($"An unhandled UI exception occurred:\n {e.Exception}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
