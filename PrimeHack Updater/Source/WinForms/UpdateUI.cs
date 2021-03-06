﻿using System;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PrimeHack_Updater.Source.WinForms
{
    public partial class UpdateUI : Form
    {
        public UpdateUI()
        {
            LoadFont();

            InitializeComponent();
            Show();

            ISOSelection();
        }

        public UpdateUI(string url)
        {
            LoadFont();

            InitializeComponent();
            Show();

            new Thread(() => Updater.Update(url)).Start();
        }

        public void writeLine(string line)
        {
            BeginInvoke((Action)(() => console.AppendText(line+"\r\n")));
        }

        public void ISOSelection()
        {
            writeLine("ISO selector");

            immersiveMode.Checked = Updater.cfg.getImmersiveMode();
            portableMode.Checked = File.Exists("./portable.txt");

            UpdatePanel.Hide();
            SelectionPanel.Show();

            FlashWindowEx(this);
        }

        private void Yes_Click(object sender, EventArgs e)
        {
            Updater.cfg.setISOPath(path_box.Text);
            Updater.runPrimeHack(path_box.Text);
        }

        private void Never_Click(object sender, EventArgs e)
        {
            Updater.cfg.setISOPath("NEVER");
            Updater.runPrimeHack("NEVER");
        }

        private void Later_Click(object sender, EventArgs e)
        {
            Updater.runPrimeHack("");
        }

        private void browse_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Title = "Select your Metroid Prime: Trilogy or Metroid Prime GC dump";
            filedialog.Filter = "All GC/Wii files|*.elf;*.dol;*.gcm;*.tgc;*.iso;*.wbfs;*.ciso;*.gcz;*.wad;*.dff;*.wia;*.rvz|ISO Files|*.iso|GCZ Files|*.gcz|WIA Files|*.wia|RVZ Files|*.rvz";
            filedialog.FilterIndex = 1;

            if (STAShowDialog(filedialog) == DialogResult.OK)
            {
                path_box.Text = filedialog.FileName;
            }
        }

        private void ImmersiveChecked(object sender, EventArgs e)
        {
            Updater.cfg.setImmersiveMode(immersiveMode.Checked);
        }

        private void PortableChecked(object sender, EventArgs e)
        {      
            if (portableMode.Checked && !File.Exists("./portable.txt"))
              File.Create("./portable.txt").Close();
            else if (!portableMode.Checked && File.Exists("./portable.txt")) 
              File.Delete("./portable.txt");
        }

        public void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                progressBar.Value = e.ProgressPercentage;
            }));
        }

        public void FinishedInstalling()
        {
            if (!Updater.IsPathValid(Updater.cfg.getISOPath()))
                BeginInvoke((Action)(() => ISOSelection()));
            else Updater.runPrimeHack(Updater.cfg.getISOPath());
        }

        public static DialogResult STAShowDialog(FileDialog dialog)
        {
            DialogState state = new DialogState();

            state.dialog = dialog;

            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);

            t.SetApartmentState(System.Threading.ApartmentState.STA);

            t.Start();

            t.Join();

            return state.result;
        }

        public class DialogState
        {
            public DialogResult result;
            public FileDialog dialog;

            public void ThreadProcShowDialog()
            {
                result = dialog.ShowDialog();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public const UInt32 FLASHW_ALL = 3;
        public const UInt32 FLASHW_TIMERNOFG = 12;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();

        // Source: https://stackoverflow.com/questions/556147/how-to-quickly-and-easily-embed-fonts-in-winforms-app-in-c-sharp/23519499#23519499
        public void LoadFont()
        {
            byte[] fontData = Properties.Resources.Deface_Regular_v1;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.Deface_Regular_v1.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Deface_Regular_v1.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
        }
  }
}
