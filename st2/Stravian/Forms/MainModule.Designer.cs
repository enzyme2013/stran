using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Stravian
{
	partial class UserControl1
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabStatus = new System.Windows.Forms.TabPage();
			this.listViewBuilding = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.contextMenuBuilding = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.aIModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabUpgrade = new System.Windows.Forms.TabPage();
			this.listView2 = new System.Windows.Forms.ListView();
			this.columnHeader22 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader23 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader24 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader25 = new System.Windows.Forms.ColumnHeader();
			this.listViewQueue = new System.Windows.Forms.ListView();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
			this.contextMenuQueue = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.enableTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuEvent = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.progressBarLogin = new System.Windows.Forms.ProgressBar();
			this.contextMenuVillage = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.button1 = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.listViewVillage = new System.Windows.Forms.ListView();
			this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.listViewEvent = new System.Windows.Forms.ListView();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
			this.tabControl1.SuspendLayout();
			this.tabStatus.SuspendLayout();
			this.contextMenuBuilding.SuspendLayout();
			this.tabUpgrade.SuspendLayout();
			this.contextMenuQueue.SuspendLayout();
			this.contextMenuVillage.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 2);
			this.tabControl1.Controls.Add(this.tabStatus);
			this.tabControl1.Controls.Add(this.tabUpgrade);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(283, 63);
			this.tabControl1.Name = "tabControl1";
			this.tableLayoutPanel1.SetRowSpan(this.tabControl1, 2);
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(294, 544);
			this.tabControl1.TabIndex = 1;
			// 
			// tabStatus
			// 
			this.tabStatus.Controls.Add(this.listViewBuilding);
			this.tabStatus.Location = new System.Drawing.Point(4, 22);
			this.tabStatus.Name = "tabStatus";
			this.tabStatus.Padding = new System.Windows.Forms.Padding(3);
			this.tabStatus.Size = new System.Drawing.Size(286, 518);
			this.tabStatus.TabIndex = 2;
			this.tabStatus.Text = "村庄建筑";
			this.tabStatus.UseVisualStyleBackColor = true;
			// 
			// listViewBuilding
			// 
			this.listViewBuilding.AllowColumnReorder = true;
			this.listViewBuilding.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewBuilding.ContextMenuStrip = this.contextMenuBuilding;
			this.listViewBuilding.Dock = System.Windows.Forms.DockStyle.Left;
			this.listViewBuilding.FullRowSelect = true;
			this.listViewBuilding.GridLines = true;
			this.listViewBuilding.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewBuilding.Location = new System.Drawing.Point(3, 3);
			this.listViewBuilding.Name = "listViewBuilding";
			this.listViewBuilding.Size = new System.Drawing.Size(280, 512);
			this.listViewBuilding.TabIndex = 3;
			this.listViewBuilding.UseCompatibleStateImageBehavior = false;
			this.listViewBuilding.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "坑号";
			this.columnHeader1.Width = 45;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "状态或建筑";
			this.columnHeader2.Width = 200;
			// 
			// contextMenuBuilding
			// 
			this.contextMenuBuilding.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.toolStripMenuItem1,
            this.toolStripMenuItem9,
            this.toolStripSeparator3,
            this.toolStripMenuItem8,
            this.toolStripSeparator4,
            this.newToolStripMenuItem,
            this.toolStripSeparator2,
            this.aIModuleToolStripMenuItem});
			this.contextMenuBuilding.Name = "contextMenuStrip2";
			this.contextMenuBuilding.ShowImageMargin = false;
			this.contextMenuBuilding.Size = new System.Drawing.Size(182, 154);
			this.contextMenuBuilding.Text = "添加到队列";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem4.Text = "&A. 升级建筑 - 添加到队列";
			this.toolStripMenuItem4.Click += new System.EventHandler(this.addToQueueToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem6});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem1.Text = "&U. 升级建筑 - 多次";
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 22);
			this.toolStripMenuItem2.Text = "&2. 添加 2 次";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(135, 22);
			this.toolStripMenuItem3.Text = "&5. 添加 5 次";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(135, 22);
			this.toolStripMenuItem6.Text = "&9. 添加 9 次";
			this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem9.Text = "L. 升级建筑到指定等级...";
			this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(178, 6);
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem8.Text = "&D. 拆除建筑";
			this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(178, 6);
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.newToolStripMenuItem.Text = "&N. 新建建筑";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
			// 
			// aIModuleToolStripMenuItem
			// 
			this.aIModuleToolStripMenuItem.Name = "aIModuleToolStripMenuItem";
			this.aIModuleToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.aIModuleToolStripMenuItem.Text = "&E. 启用人工智能模块";
			this.aIModuleToolStripMenuItem.Click += new System.EventHandler(this.AIModuleToolStripMenuItem_Click);
			// 
			// tabUpgrade
			// 
			this.tabUpgrade.Controls.Add(this.listView2);
			this.tabUpgrade.Location = new System.Drawing.Point(4, 22);
			this.tabUpgrade.Name = "tabUpgrade";
			this.tabUpgrade.Padding = new System.Windows.Forms.Padding(3);
			this.tabUpgrade.Size = new System.Drawing.Size(286, 518);
			this.tabUpgrade.TabIndex = 4;
			this.tabUpgrade.Text = "研发状态";
			this.tabUpgrade.UseVisualStyleBackColor = true;
			// 
			// listView2
			// 
			this.listView2.AllowColumnReorder = true;
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader22,
            this.columnHeader23,
            this.columnHeader24,
            this.columnHeader25});
			this.listView2.Dock = System.Windows.Forms.DockStyle.Left;
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView2.Location = new System.Drawing.Point(3, 3);
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(280, 512);
			this.listView2.TabIndex = 5;
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader22
			// 
			this.columnHeader22.Text = "兵种";
			this.columnHeader22.Width = 80;
			// 
			// columnHeader23
			// 
			this.columnHeader23.Text = "研发";
			// 
			// columnHeader24
			// 
			this.columnHeader24.Text = "攻击";
			this.columnHeader24.Width = 50;
			// 
			// columnHeader25
			// 
			this.columnHeader25.Text = "防御";
			this.columnHeader25.Width = 50;
			// 
			// listViewQueue
			// 
			this.listViewQueue.AllowColumnReorder = true;
			this.listViewQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader8,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader9});
			this.tableLayoutPanel1.SetColumnSpan(this.listViewQueue, 3);
			this.listViewQueue.ContextMenuStrip = this.contextMenuQueue;
			this.listViewQueue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewQueue.FullRowSelect = true;
			this.listViewQueue.GridLines = true;
			this.listViewQueue.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewQueue.Location = new System.Drawing.Point(583, 63);
			this.listViewQueue.Name = "listViewQueue";
			this.tableLayoutPanel1.SetRowSpan(this.listViewQueue, 2);
			this.listViewQueue.Size = new System.Drawing.Size(384, 544);
			this.listViewQueue.TabIndex = 0;
			this.listViewQueue.UseCompatibleStateImageBehavior = false;
			this.listViewQueue.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "村";
			this.columnHeader4.Width = 40;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "坑";
			this.columnHeader5.Width = 45;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "类别";
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "状态";
			this.columnHeader6.Width = 80;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "延迟";
			this.columnHeader7.Width = 50;
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "建筑名";
			this.columnHeader9.Width = 80;
			// 
			// contextMenuQueue
			// 
			this.contextMenuQueue.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.toolStripSeparator5,
            this.enableTimerToolStripMenuItem,
            this.toolStripMenuItem5,
            this.toolStripMenuItem7});
			this.contextMenuQueue.Name = "contextMenuStrip3";
			this.contextMenuQueue.ShowCheckMargin = true;
			this.contextMenuQueue.ShowImageMargin = false;
			this.contextMenuQueue.Size = new System.Drawing.Size(196, 154);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.deleteToolStripMenuItem.Text = "&D. 删除";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.clearToolStripMenuItem.Text = "C. 清空";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			// 
			// moveUpToolStripMenuItem
			// 
			this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
			this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
			this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.moveUpToolStripMenuItem.Text = "&U. 上移";
			this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
			// 
			// moveDownToolStripMenuItem
			// 
			this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
			this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
			this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.moveDownToolStripMenuItem.Text = "&D. 下移";
			this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(192, 6);
			// 
			// enableTimerToolStripMenuItem
			// 
			this.enableTimerToolStripMenuItem.Checked = true;
			this.enableTimerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableTimerToolStripMenuItem.Name = "enableTimerToolStripMenuItem";
			this.enableTimerToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.enableTimerToolStripMenuItem.Text = "&E. 延迟倒计时正在工作";
			this.enableTimerToolStripMenuItem.Click += new System.EventHandler(this.enableTimerToolStripMenuItem_Click);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(192, 6);
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(195, 22);
			this.toolStripMenuItem7.Text = "Refresh";
			this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
			// 
			// contextMenuEvent
			// 
			this.contextMenuEvent.Name = "contextMenuEvent";
			this.contextMenuEvent.Size = new System.Drawing.Size(61, 4);
			// 
			// progressBarLogin
			// 
			this.progressBarLogin.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressBarLogin.Location = new System.Drawing.Point(3, 585);
			this.progressBarLogin.Maximum = 5;
			this.progressBarLogin.Name = "progressBarLogin";
			this.progressBarLogin.Size = new System.Drawing.Size(274, 22);
			this.progressBarLogin.TabIndex = 10;
			this.progressBarLogin.Visible = false;
			// 
			// contextMenuVillage
			// 
			this.contextMenuVillage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem});
			this.contextMenuVillage.Name = "contextMenuStrip1";
			this.contextMenuVillage.ShowImageMargin = false;
			this.contextMenuVillage.Size = new System.Drawing.Size(74, 26);
			this.contextMenuVillage.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(73, 22);
			this.refreshToolStripMenuItem.Text = "刷新";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 5000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(962, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(22, 22);
			this.button1.TabIndex = 5;
			this.button1.Text = "X";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 6;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.label4, 4, 0);
			this.tableLayoutPanel1.Controls.Add(this.progressBarLogin, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.listViewVillage, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.listViewQueue, 3, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(970, 610);
			this.tableLayoutPanel1.TabIndex = 12;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.SystemColors.Info;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(290, 4);
			this.label1.Margin = new System.Windows.Forms.Padding(10, 4, 10, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(130, 52);
			this.label1.TabIndex = 11;
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.SystemColors.Info;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(440, 4);
			this.label2.Margin = new System.Windows.Forms.Padding(10, 4, 10, 4);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 52);
			this.label2.TabIndex = 12;
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.SystemColors.Info;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Location = new System.Drawing.Point(590, 4);
			this.label3.Margin = new System.Windows.Forms.Padding(10, 4, 10, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(130, 52);
			this.label3.TabIndex = 13;
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.BackColor = System.Drawing.SystemColors.Info;
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Location = new System.Drawing.Point(740, 4);
			this.label4.Margin = new System.Windows.Forms.Padding(10, 4, 10, 4);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(130, 52);
			this.label4.TabIndex = 14;
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listViewVillage
			// 
			this.listViewVillage.AllowColumnReorder = true;
			this.listViewVillage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader15,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14});
			this.listViewVillage.ContextMenuStrip = this.contextMenuVillage;
			this.listViewVillage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewVillage.Font = new System.Drawing.Font("Tahoma", 8F);
			this.listViewVillage.FullRowSelect = true;
			this.listViewVillage.GridLines = true;
			this.listViewVillage.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewVillage.HideSelection = false;
			this.listViewVillage.Location = new System.Drawing.Point(3, 3);
			this.listViewVillage.MultiSelect = false;
			this.listViewVillage.Name = "listViewVillage";
			this.tableLayoutPanel1.SetRowSpan(this.listViewVillage, 2);
			this.listViewVillage.ShowItemToolTips = true;
			this.listViewVillage.Size = new System.Drawing.Size(274, 576);
			this.listViewVillage.TabIndex = 15;
			this.listViewVillage.UseCompatibleStateImageBehavior = false;
			this.listViewVillage.View = System.Windows.Forms.View.Details;
			this.listViewVillage.SelectedIndexChanged += new System.EventHandler(this.listViewVillage_Changed);
			this.listViewVillage.Click += new System.EventHandler(this.listViewVillage_Click);
			// 
			// columnHeader11
			// 
			this.columnHeader11.Text = "#";
			this.columnHeader11.Width = 25;
			// 
			// columnHeader15
			// 
			this.columnHeader15.Text = "?";
			this.columnHeader15.Width = 25;
			// 
			// columnHeader12
			// 
			this.columnHeader12.Text = "村名";
			this.columnHeader12.Width = 110;
			// 
			// columnHeader13
			// 
			this.columnHeader13.Text = "坐标";
			this.columnHeader13.Width = 55;
			// 
			// columnHeader14
			// 
			this.columnHeader14.Text = "ID";
			this.columnHeader14.Width = 50;
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPage2);
			this.tabControl2.Controls.Add(this.tabPage3);
			this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl2.Location = new System.Drawing.Point(0, 0);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(984, 642);
			this.tabControl2.TabIndex = 13;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tableLayoutPanel1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(976, 616);
			this.tabPage2.TabIndex = 0;
			this.tabPage2.Text = "队列";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.listViewEvent);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(976, 616);
			this.tabPage3.TabIndex = 1;
			this.tabPage3.Text = "事件";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// listViewEvent
			// 
			this.listViewEvent.AllowColumnReorder = true;
			this.listViewEvent.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader10,
            this.columnHeader16});
			this.listViewEvent.ContextMenuStrip = this.contextMenuEvent;
			this.listViewEvent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewEvent.FullRowSelect = true;
			this.listViewEvent.GridLines = true;
			this.listViewEvent.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewEvent.Location = new System.Drawing.Point(3, 3);
			this.listViewEvent.Name = "listViewEvent";
			this.listViewEvent.Size = new System.Drawing.Size(970, 610);
			this.listViewEvent.TabIndex = 5;
			this.listViewEvent.UseCompatibleStateImageBehavior = false;
			this.listViewEvent.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "触发条件";
			this.columnHeader3.Width = 300;
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "动作";
			this.columnHeader10.Width = 300;
			// 
			// columnHeader16
			// 
			this.columnHeader16.Text = "状态";
			this.columnHeader16.Width = 320;
			// 
			// UserControl1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tabControl2);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "UserControl1";
			this.Size = new System.Drawing.Size(984, 642);
			this.tabControl1.ResumeLayout(false);
			this.tabStatus.ResumeLayout(false);
			this.contextMenuBuilding.ResumeLayout(false);
			this.tabUpgrade.ResumeLayout(false);
			this.contextMenuQueue.ResumeLayout(false);
			this.contextMenuVillage.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.tabControl2.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.TabControl tabControl1;
		public System.Windows.Forms.TabPage tabStatus;
		public System.Windows.Forms.ListView listViewQueue;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		public System.Windows.Forms.ListView listViewBuilding;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ContextMenuStrip contextMenuQueue;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem enableTimerToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuVillage;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuBuilding;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.Button button1;
		public ProgressBar progressBarLogin;
		public Timer timer1;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem moveUpToolStripMenuItem;
		private ToolStripMenuItem moveDownToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem aIModuleToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItem1;
		private ToolStripMenuItem toolStripMenuItem2;
		private ToolStripMenuItem toolStripMenuItem3;
		private ToolStripMenuItem toolStripMenuItem4;
		private TableLayoutPanel tableLayoutPanel1;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private ToolStripMenuItem toolStripMenuItem6;
		private ContextMenuStrip contextMenuEvent;
		private ToolStripSeparator toolStripMenuItem5;
		private ToolStripMenuItem toolStripMenuItem7;
		public ListView listViewVillage;
		private ColumnHeader columnHeader11;
		private ColumnHeader columnHeader12;
		private ColumnHeader columnHeader13;
		private ColumnHeader columnHeader14;
		private ColumnHeader columnHeader15;
		private ToolStripMenuItem toolStripMenuItem8;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripMenuItem toolStripMenuItem9;
		private TabPage tabUpgrade;
		public ListView listView2;
		private ColumnHeader columnHeader22;
		private ColumnHeader columnHeader23;
		private ColumnHeader columnHeader24;
		private ColumnHeader columnHeader25;
		private TabControl tabControl2;
		private TabPage tabPage2;
		private TabPage tabPage3;
		public ListView listViewEvent;
		private ColumnHeader columnHeader3;
		private ColumnHeader columnHeader10;
		private ColumnHeader columnHeader16;
	}
}
