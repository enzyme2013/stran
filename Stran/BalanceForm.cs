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
        //private TextBox[] txt;
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
        	mui.RefreshLanguage(this);
            if (BalancerGroup == null)
            {
                BalancerGroup = TBalancerGroup.GetDefaultTBalancerGroup();
            }
            this.numID.Value = BalancerGroup.ID;
            this.cbxMarket.SelectedIndex = (int)BalancerGroup.IgnoreMarket == 1 ? 1 : 0;
            this.numMarketTime.Value = BalancerGroup.IgnoreMarketTime / 60;
            this.textDescription.Text = BalancerGroup.desciption;
            this.numReadyTime.Value = BalancerGroup.ReadyTime;
            if (Village.Market.MaxMerchant > 0)
            {
                int max = Village.Market.SingleCarry * Village.Market.MaxMerchant;
                max = max > TBalancerGroup.GetDefaultTBalancerGroup().MaxSendResource ? max : TBalancerGroup.GetDefaultTBalancerGroup().MaxSendResource;
                this.numMinSendResource.Enabled = this.numMaxSendResource.Enabled = true;
                this.numMinSendResource.Maximum = max;
                this.numMaxSendResource.Maximum = max;
                this.numMinSendResource.Increment = this.numMaxSendResource.Increment = Village.Market.SingleCarry / 2;
                this.numMinSendResource.Value = BalancerGroup.MinSendResource;
                this.numMaxSendResource.Value = BalancerGroup.MaxSendResource;

                //this.numMaxSendResource.Maximum = BalancerGroup.MaxSendResource != 0 ? BalancerGroup.MaxSendResource : (this.numMaxSendResource.Maximum = tmp > 100000 ? tmp : 100000);
            }
            else
            {
                this.numMinSendResource.Enabled = this.numMaxSendResource.Enabled = false;
                this.numMinSendResource.Value = TBalancerGroup.GetDefaultTBalancerGroup().MinSendResource;
                this.numMaxSendResource.Value = TBalancerGroup.GetDefaultTBalancerGroup().MaxSendResource;
            }
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            TBalancerGroup group = new TBalancerGroup();
            group.ID = Convert.ToInt32(this.numID.Value);
             
            if (this.cbxMarket.SelectedIndex == 1)
            {
                group.IgnoreMarket = BalancerQueue.IgnorMarketType.ignore;
            }
            else
            {
                group.IgnoreMarket = BalancerQueue.IgnorMarketType.notignore;
            }
                
            group.IgnoreMarketTime = Convert.ToInt32(this.numMarketTime.Value) * 60;
            group.desciption = this.textDescription.Text;
            group.ReadyTime = Convert.ToInt32(this.numReadyTime.Value);

            group.MinSendResource = Convert.ToInt32(this.numMinSendResource.Value);
            group.MaxSendResource = Convert.ToInt32(this.numMaxSendResource.Value);

            BalancerGroup = group;
        }
    }
}
