using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace mtemu
{
    static class Program
    {
        const int WinDefaultDPI = 96;

        /// <summary>
        /// Исправление блюра при включенном масштабировании в ОС windows 8 и выше
        /// </summary>
        public static void DpiFix()
        {
            if (Environment.OSVersion.Version.Major >= 6) {
                SetProcessDPIAware();
            }
        }

        /// <summary>
        /// WinAPI SetProcessDPIAware
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// Исправление размера шрифтов
        /// </summary>
        /// <param name="c"></param>
        public static float DpiFixFonts(Control c)
        {
            Graphics g = c.CreateGraphics();
            float dx = g.DpiX
                , dy = g.DpiY
                , fontsScale = Math.Max(dx, dy) / WinDefaultDPI
            ;
            g.Dispose();
            return fontsScale;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
