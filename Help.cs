using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOfGOD
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(228, 236, 237);
            label3.BackColor = Color.FromArgb(81, 120, 145);
            label1.BackColor = Color.FromArgb(81, 120, 145);
            label2.ForeColor = Color.FromArgb(81, 120, 145);
            label4.ForeColor = Color.FromArgb(81, 120, 145);
            label5.ForeColor = Color.FromArgb(81, 120, 145);
            label6.ForeColor = Color.FromArgb(81, 120, 145);
            label7.ForeColor = Color.FromArgb(81, 120, 145);
            label8.ForeColor = Color.FromArgb(81, 120, 145);
            label9.ForeColor = Color.FromArgb(81, 120, 145);
        }
    }

}
