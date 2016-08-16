using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScannableDataEncoding
{
    public partial class frmMain : Form
    {
        public Core.ScannableDataEncoding SDE;
        public frmMain()
        {
            InitializeComponent();

            SDE = new Core.ScannableDataEncoding(Core.OnColorsOptions.Red, Core.OffColorsOptions.Green);
        }

        private void dataToImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pbImage.Image = SDE.Encode(Encoding.ASCII.GetBytes(txtData.Text));
        }
    }
}
