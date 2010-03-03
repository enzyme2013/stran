using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using libTravian;

namespace Stran
{
    public partial class Rename : Form
    {
        public MUI mui { get; set; }
        public string VillageName { get; set; }
        public int VillageID { get; set; }
        public Travian UpCall { get; set; }

        public Rename()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
        	VillageName = this.tbnewVillagename.Text;
        }

        private void Rename_Load(object sender, EventArgs e)
        {
            mui.RefreshLanguage(this);
            this.lboldVillagename.Text = this.tbnewVillagename.Text = UpCall.TD.Villages[VillageID].Name;
        }
    }
}
