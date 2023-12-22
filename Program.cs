using FlowstorageDesktop.Authentication;
using System;
using System.Windows.Forms;

namespace FlowstorageDesktop {
    internal static class Program
    {
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SplashForm());
            Application.Run(new SignUpForm());

        }
    }
}
