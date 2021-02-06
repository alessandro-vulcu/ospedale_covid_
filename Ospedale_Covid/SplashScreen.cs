using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Ospedale_Covid
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
            this.Opacity = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Width += 3;
            this.Opacity += 0.025;
            if (panel1.Width >= 700)
            {
                
                timer1.Stop();
                Form1 f = new Form1();
                this.Hide();
                f.ShowDialog();
                this.Close();
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
