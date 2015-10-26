using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutsideSimulator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        ///  We are going to keep this single-threaded because it's simpler to
        ///  handle the D3D API when only the immediate context is required
        /// </summary>
        [STAThread]
        static void Main()
        {
            OutsideSimulatorApp app = OutsideSimulatorApp.GetInstance();
            app.Begin();
        }
    }
}
