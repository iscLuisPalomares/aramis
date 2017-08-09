using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ComprasProject {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        internal struct LASTINPUTINFO {
            public uint cbSize;
            public uint dwTime;
        }
        static System.Timers.Timer aTimer;
        [DllImport("User32.dll")]
        public static extern bool LockWorkStation();
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);
        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime() {
            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            GetLastInputInfo(ref LastUserAction);
            return ((uint)Environment.TickCount - LastUserAction.dwTime);
        }
        public static long GetTickCount() {
            return Environment.TickCount;
        }
        public static long GetLastInputTime() {
            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            if (!GetLastInputInfo(ref LastUserAction)) {
                throw new Exception(GetLastError().ToString());
            }
            return LastUserAction.dwTime;
        }

        public static string username = "";
        public static string userid = "";
        public static string deptoname = "";
        public static string deptoid = "";
        //testing o master SELECT DATABASE
        public static string stringconnection = "Data Source=MEXATLAS\\REQUI;Database=master;User ID=sa;Password=R3qu1@16;";
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += aTimer_Elapsed;
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            aTimer.Start();
            Application.Run(new Login());
        }
        public static bool bloqueada = true;
        static void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            if (GetIdleTime() >= 1000 * 60 * 5 && !bloqueada) {
                //LockWorkStation();
                //MessageBox.Show("bloquear");
                //bloqueada = true;
            }
            if (GetIdleTime() < 100 && !bloqueada) {
                //bloqueada = false;
            }
        }
    }
}
