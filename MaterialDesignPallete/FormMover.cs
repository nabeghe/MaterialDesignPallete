using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MaterialDesignPallete
{
    public class FormMover
    {
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        public static void Move(Form frm)
        {
            ReleaseCapture();
            SendMessage(frm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        public static void AddMover(Form frm, Control mover, MouseButtons key = MouseButtons.Left)
        {
            mover.MouseDown += (object sender, MouseEventArgs e) =>
            {
                if (e.Button == key) FormMover.Move(frm);
            };
        }
    }
}
