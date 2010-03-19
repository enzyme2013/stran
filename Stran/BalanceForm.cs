using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using libTravian;

namespace Stran
{
    public partial class BalanceForm : Form
    {
        #region Fields
        private TextBox[] txt;
        #endregion

        #region Properties
        public TBalancerGroup BalancerGroup { get; set; }
        public TVillage Village { get; set; }
        public MUI mui { get; set; }
        public Travian UpCall { get; set; }
        #endregion

        public BalanceForm()
        {
            InitializeComponent();
        }

        private void BalanceForm_Load(object sender, EventArgs e)
        {
            if (BalancerGroup == null)
            {
                BalancerGroup = TBalancerGroup.GetDefaultTBalancerGroup();
            }
            this.textID.Text = BalancerGroup.ID.ToString();
            this.textMarket.Text = ((int)BalancerGroup.IgnoreMarket).ToString();
            this.textMarketTime.Text = BalancerGroup.IgnoreMarketTime.ToString();
            this.textDescription.Text = BalancerGroup.desciption;
            this.textReadyTime.Text = BalancerGroup.ReadyTime.ToString();
            this.textMinSendResource.Text = BalancerGroup.MinSendResource.ToString();
            this.textMaxSendResource.Text = BalancerGroup.MaxSendResource.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TBalancerGroup group = new TBalancerGroup();
            group.ID = Convert.ToInt32(this.textID.Text);
             
            if (Convert.ToInt32(this.textMarket.Text) == 1)
            {
                group.IgnoreMarket = BalancerQueue.IgnorMarketType.ignore;
            }
            else
            {
                group.IgnoreMarket = BalancerQueue.IgnorMarketType.notignore;
            }
                
            group.IgnoreMarketTime = Convert.ToInt32(this.textMarketTime.Text);
            group.desciption = this.textDescription.Text;
            group.ReadyTime = Convert.ToInt32(this.textReadyTime.Text);

            group.MinSendResource = Convert.ToInt32(textMinSendResource.Text);
            group.MinSendResource = Convert.ToInt32(textMinSendResource.Text);

            BalancerGroup = group;

            
        }
    }
}
