using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PrimeHack_Updater
{
    public partial class ISOSelectionDialog : Form
    {
        public ISOSelectionDialog()
        {
            InitializeComponent();
        }

        private void Never(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void Later(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            Close();
        }

        private void Confirm(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
