using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JA_mod_tool
{
    public partial class Localization : Form
    {


        public Localization()
        {
            InitializeComponent();
        }

        public void locCancelB_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void locOKb_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
