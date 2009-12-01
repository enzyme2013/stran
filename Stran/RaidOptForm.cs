using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using libTravian;

namespace Stran
{
	public partial class RaidOptForm : Form
    {
        #region Fields
        private TextBox[] txtTroops;
        private Label[] lblTroops;
        private Label[] lblMaxTroops;
        private RadioButton[] rdbRaidTypes;
        private RadioButton[] rdbSpyOptions;
        #endregion

        #region Constructor
        public RaidOptForm()
		{
			InitializeComponent();
            this.CreateControlArrays();
            if (MainForm.Options.ContainsKey("AllowMultipleRaids"))
            {
                this.grpSchedule.Visible = true;
            }
        }
        #endregion

        #region Properties
        public MUI mui { get; set; }
		public DisplayLang dl { get; set; }
		public NumericUpDown[] Nums = new NumericUpDown[11];
        public TTInfo TroopsAtHome { get; set; }
		public RaidQueue Return { get; set; }
        #endregion

        #region Methods
        private void RaidOptForm_Load(object sender, EventArgs e)
		{
			mui.RefreshLanguage(this);
			SuspendLayout();
            if (this.Return != null)
            {
                for (int i = 0; i < this.lblTroops.Length; i ++)
                {
                    this.lblTroops[i].Text = DisplayLang.Instance.GetAidLang(this.Return.Tribe, i + 1);
                }

                if (this.Return.Troops != null)
                {
                    for (int i = 0; i < this.Return.Troops.Length; i++)
                    {
                        this.txtTroops[i].Text = this.Return.Troops[i].ToString();
                    }
                }

                if (this.TroopsAtHome != null)
                {
                    for (int i = 0; i < this.TroopsAtHome.Troops.Length; i++)
                    {
                        this.lblMaxTroops[i].Text = string.Format(
                            "({0})",
                            this.TroopsAtHome.Troops[i]);
                    }
                }

                this.rdbRaidTypes[this.Return.RaidType - RaidType.Reinforce].Select();
                this.rdbSpyOptions[this.Return.SpyOption - SpyOption.Resource].Select();

                if (this.Return.Targets != null)
                {
                    foreach (TPoint village in this.Return.Targets)
                    {
                        this.lstTargets.Items.Add(village);
                    }
                }
            }

			ResumeLayout();
		}

        private void CreateControlArrays()
        {
            this.txtTroops = new TextBox[]
            {
                this.txtT1,
                this.txtT2,
                this.txtT3,
                this.txtT4,
                this.txtT5,
                this.txtT6,
                this.txtT7,
                this.txtT8,
                this.txtT9,
                this.txtT10,
                this.txtT11,
            };

            this.lblTroops = new Label[]
            {
                this.lblT1,
                this.lblT2,
                this.lblT3,
                this.lblT4,
                this.lblT5,
                this.lblT6,
                this.lblT7,
                this.lblT8,
                this.lblT9,
                this.lblT10,
                this.lblT11,
            };

            this.lblMaxTroops = new Label[]
            {
                this.lblT1Max,
                this.lblT2Max,
                this.lblT3Max,
                this.lblT4Max,
                this.lblT5Max,
                this.lblT6Max,
                this.lblT7Max,
                this.lblT8Max,
                this.lblT9Max,
                this.lblT10Max,
                this.lblT11Max,
            };

            this.rdbRaidTypes = new RadioButton[]
            {
                this.rdbTypeReinforce,
                this.rdbAttackNormal,
                this.rdbAttackRaid,
            };

            this.rdbSpyOptions = new RadioButton[]
            {
                this.rdbSpyResource,
                this.rdbSpyDefense,
            };
        }

		private void btnOk_Click(object sender, EventArgs e)
		{
            if (this.Return == null)
            {
                this.Return = new RaidQueue();
            }

            this.Return.Troops = new int[11];
            for (int i = 0; i < 11; i++)
            {
                Int32.TryParse(this.txtTroops[i].Text, out this.Return.Troops[i]);
            }

            this.Return.Targets = new List<TPoint>();
            foreach (object village in this.lstTargets.Items)
            {
                this.Return.Targets.Add((TPoint)village);
            }

            this.Return.RaidType = RaidType.Reinforce + this.SelectedRadioButtonIndex(this.rdbRaidTypes);
            this.Return.SpyOption = SpyOption.Resource + this.SelectedRadioButtonIndex(this.rdbSpyOptions);
            this.Return.MaxCount = Convert.ToInt32(this.nudCount.Value);
        }

        private int SelectedRadioButtonIndex(RadioButton[] radioButtons)
        {
            for (int i = 0; i < radioButtons.Length; i++)
            {
                if (radioButtons[i].Checked)
                {
                    return i;
                }
            }

            return -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int x, y;
            if (!Int32.TryParse(this.txtX.Text, out x))
            {
                return;
            }

            if (!Int32.TryParse(this.txtY.Text, out y))
            {
                return;
            }

            TPoint village = new TPoint(x, y);
            this.lstTargets.Items.Add(village);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.lstTargets.SelectedIndex < 0)
            {
                return;
            }

            this.lstTargets.Items.RemoveAt(this.lstTargets.SelectedIndex);
        }

        private void lblMaxTroops_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.lblMaxTroops.Length; i++)
            {
                if (this.lblMaxTroops[i] == sender)
                {
                    string text = this.lblMaxTroops[i].Text;
                    this.txtTroops[i].Text = text.Substring(1, text.Length - 2);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(dialog.OpenFile());
                this.lstTargets.Items.Clear();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.StartsWith(";"))
                    {
                        continue;
                    }

                    TPoint village = TPoint.FromString(line);
                    if (!village.IsEmpty)
                    {
                        this.lstTargets.Items.Add(village);
                    }
                }

                reader.Close();
            }
        }
        #endregion
    }
}
