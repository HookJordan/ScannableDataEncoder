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
            //convert the data to an image 
            pbImage.Image = SDE.Encode(Encoding.ASCII.GetBytes(txtData.Text));
        }

        private void imageToDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //convert the image to data 
            txtData.Text = Encoding.ASCII.GetString(SDE.Decode((Bitmap)pbImage.Image));
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create a save file dialog for the image 
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                //bmps only at this time 
                sfd.Filter = "bitmap files (*.bmp)|*.bmp"; 

                //if file path is selected 
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    //save the image at a bmp 
                    pbImage.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create an open file dialog to find a new image 
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //set the filter for bitmaps 
                ofd.Filter = "bitmap files (*.bmp)|*.bmp";

                //if a file is selected 
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //load the image 
                    pbImage.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //close the gui 
            Application.Exit();
        }

        private void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //launch jv programming website
            System.Diagnostics.Process.Start("http://jvprogramming.com");
        }

        private void developersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //launch github project for colab information 
            System.Diagnostics.Process.Start("https://github.com/HookJordan/ScannableDataEncoder");
        }

        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //launch the git hub wiki page for additional informaiton 
            System.Diagnostics.Process.Start("https://github.com/HookJordan/ScannableDataEncoder/wiki");
        }
    }
}
