using System;
using System.Windows.Forms;

namespace ScenarioEdit
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var screen = Screen.FromPoint(Cursor.Position);

            try
            {
                Application.Run(new FormMain(screen));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception occurred:\n {ex?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
