using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desktop_box
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            HttpApiServer httpApiServer = new HttpApiServer();
            Form1 mainForm = new Form1();
            httpApiServer.Run(new Controller(mainForm));
           Application.Run(mainForm);
        }
    }
}
