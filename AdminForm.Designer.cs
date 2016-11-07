namespace AdminTool
{
    partial class AdminForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.adminTreeView = new System.Windows.Forms.TreeView();
            this.treeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.argsBox = new System.Windows.Forms.GroupBox();
            this.argsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.cmdGroupBox = new System.Windows.Forms.GroupBox();
            this.hostCbx = new System.Windows.Forms.ComboBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.cmdTbx = new System.Windows.Forms.TextBox();
            this.cmdLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.countNum = new System.Windows.Forms.NumericUpDown();
            this.uidCbx = new System.Windows.Forms.ComboBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.uidLabel = new System.Windows.Forms.Label();
            this.procBtn = new System.Windows.Forms.Button();
            this.connectBtn = new System.Windows.Forms.Button();
            this.logRtb = new System.Windows.Forms.RichTextBox();
            this.widgetTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.uidListRTB = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.batchBtn = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.treeMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.argsBox.SuspendLayout();
            this.cmdGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.countNum)).BeginInit();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileFToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(739, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileFToolStripMenuItem
            // 
            this.fileFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMenu});
            this.fileFToolStripMenuItem.Name = "fileFToolStripMenuItem";
            this.fileFToolStripMenuItem.Size = new System.Drawing.Size(53, 21);
            this.fileFToolStripMenuItem.Text = "File(&F)";
            // 
            // loadMenu
            // 
            this.loadMenu.Name = "loadMenu";
            this.loadMenu.Size = new System.Drawing.Size(119, 22);
            this.loadMenu.Text = "Load(&L)";
            this.loadMenu.Click += new System.EventHandler(this.loadMenu_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.adminTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(739, 646);
            this.splitContainer1.SplitterDistance = 245;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.TabStop = false;
            // 
            // adminTreeView
            // 
            this.adminTreeView.ContextMenuStrip = this.treeMenuStrip;
            this.adminTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adminTreeView.HideSelection = false;
            this.adminTreeView.Location = new System.Drawing.Point(0, 0);
            this.adminTreeView.Name = "adminTreeView";
            this.adminTreeView.Size = new System.Drawing.Size(245, 646);
            this.adminTreeView.TabIndex = 1;
            this.adminTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.adminTreeView_AfterSelect);
            // 
            // treeMenuStrip
            // 
            this.treeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandMenu,
            this.collapseMenu});
            this.treeMenuStrip.Name = "adminTreeMenuStrip";
            this.treeMenuStrip.Size = new System.Drawing.Size(101, 48);
            // 
            // expandMenu
            // 
            this.expandMenu.Name = "expandMenu";
            this.expandMenu.Size = new System.Drawing.Size(100, 22);
            this.expandMenu.Text = "展开";
            this.expandMenu.Click += new System.EventHandler(this.expandMenu_Click);
            // 
            // collapseMenu
            // 
            this.collapseMenu.Name = "collapseMenu";
            this.collapseMenu.Size = new System.Drawing.Size(100, 22);
            this.collapseMenu.Text = "折叠";
            this.collapseMenu.Click += new System.EventHandler(this.collapseMenu_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.argsBox);
            this.splitContainer2.Panel1.Controls.Add(this.cmdGroupBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.logRtb);
            this.splitContainer2.Size = new System.Drawing.Size(488, 646);
            this.splitContainer2.SplitterDistance = 431;
            this.splitContainer2.SplitterWidth = 6;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.TabStop = false;
            // 
            // argsBox
            // 
            this.argsBox.Controls.Add(this.argsPanel);
            this.argsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argsBox.Location = new System.Drawing.Point(0, 216);
            this.argsBox.Name = "argsBox";
            this.argsBox.Size = new System.Drawing.Size(488, 215);
            this.argsBox.TabIndex = 11;
            this.argsBox.TabStop = false;
            this.argsBox.Text = "参数";
            // 
            // argsPanel
            // 
            this.argsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.argsPanel.Location = new System.Drawing.Point(3, 17);
            this.argsPanel.Name = "argsPanel";
            this.argsPanel.Size = new System.Drawing.Size(482, 195);
            this.argsPanel.TabIndex = 0;
            // 
            // cmdGroupBox
            // 
            this.cmdGroupBox.Controls.Add(this.batchBtn);
            this.cmdGroupBox.Controls.Add(this.label1);
            this.cmdGroupBox.Controls.Add(this.uidListRTB);
            this.cmdGroupBox.Controls.Add(this.hostCbx);
            this.cmdGroupBox.Controls.Add(this.sendBtn);
            this.cmdGroupBox.Controls.Add(this.cmdTbx);
            this.cmdGroupBox.Controls.Add(this.cmdLabel);
            this.cmdGroupBox.Controls.Add(this.label4);
            this.cmdGroupBox.Controls.Add(this.countNum);
            this.cmdGroupBox.Controls.Add(this.uidCbx);
            this.cmdGroupBox.Controls.Add(this.hostLabel);
            this.cmdGroupBox.Controls.Add(this.uidLabel);
            this.cmdGroupBox.Controls.Add(this.procBtn);
            this.cmdGroupBox.Controls.Add(this.connectBtn);
            this.cmdGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmdGroupBox.Location = new System.Drawing.Point(0, 0);
            this.cmdGroupBox.Name = "cmdGroupBox";
            this.cmdGroupBox.Size = new System.Drawing.Size(488, 216);
            this.cmdGroupBox.TabIndex = 0;
            this.cmdGroupBox.TabStop = false;
            this.cmdGroupBox.Text = "命令";
            // 
            // hostCbx
            // 
            this.hostCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hostCbx.FormattingEnabled = true;
            this.hostCbx.Location = new System.Drawing.Point(45, 16);
            this.hostCbx.Name = "hostCbx";
            this.hostCbx.Size = new System.Drawing.Size(333, 20);
            this.hostCbx.TabIndex = 1;
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(389, 47);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
            this.sendBtn.TabIndex = 5;
            this.sendBtn.Text = "发送";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // cmdTbx
            // 
            this.cmdTbx.Location = new System.Drawing.Point(45, 48);
            this.cmdTbx.Name = "cmdTbx";
            this.cmdTbx.Size = new System.Drawing.Size(333, 21);
            this.cmdTbx.TabIndex = 4;
            // 
            // cmdLabel
            // 
            this.cmdLabel.AutoSize = true;
            this.cmdLabel.Location = new System.Drawing.Point(10, 52);
            this.cmdLabel.Name = "cmdLabel";
            this.cmdLabel.Size = new System.Drawing.Size(23, 12);
            this.cmdLabel.TabIndex = 114;
            this.cmdLabel.Text = "cmd";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(275, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 102;
            this.label4.Text = "次数";
            // 
            // countNum
            // 
            this.countNum.Location = new System.Drawing.Point(312, 81);
            this.countNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countNum.Name = "countNum";
            this.countNum.Size = new System.Drawing.Size(66, 21);
            this.countNum.TabIndex = 7;
            this.countNum.TabStop = false;
            this.countNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // uidCbx
            // 
            this.uidCbx.FormattingEnabled = true;
            this.uidCbx.Location = new System.Drawing.Point(45, 81);
            this.uidCbx.Name = "uidCbx";
            this.uidCbx.Size = new System.Drawing.Size(224, 20);
            this.uidCbx.TabIndex = 6;
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(10, 20);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(29, 12);
            this.hostLabel.TabIndex = 104;
            this.hostLabel.Text = "host";
            // 
            // uidLabel
            // 
            this.uidLabel.AutoSize = true;
            this.uidLabel.Location = new System.Drawing.Point(10, 85);
            this.uidLabel.Name = "uidLabel";
            this.uidLabel.Size = new System.Drawing.Size(23, 12);
            this.uidLabel.TabIndex = 112;
            this.uidLabel.Text = "uid";
            // 
            // procBtn
            // 
            this.procBtn.Location = new System.Drawing.Point(389, 80);
            this.procBtn.Name = "procBtn";
            this.procBtn.Size = new System.Drawing.Size(75, 23);
            this.procBtn.TabIndex = 8;
            this.procBtn.Text = "执行";
            this.procBtn.UseVisualStyleBackColor = true;
            this.procBtn.Click += new System.EventHandler(this.procBtn_Click);
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(389, 15);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 3;
            this.connectBtn.Text = "连接";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // logRtb
            // 
            this.logRtb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logRtb.Location = new System.Drawing.Point(0, 0);
            this.logRtb.Name = "logRtb";
            this.logRtb.ReadOnly = true;
            this.logRtb.Size = new System.Drawing.Size(488, 209);
            this.logRtb.TabIndex = 0;
            this.logRtb.TabStop = false;
            this.logRtb.Text = "";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 671);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(739, 22);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // uidListRTB
            // 
            this.uidListRTB.Location = new System.Drawing.Point(45, 117);
            this.uidListRTB.Name = "uidListRTB";
            this.uidListRTB.Size = new System.Drawing.Size(333, 93);
            this.uidListRTB.TabIndex = 115;
            this.uidListRTB.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 116;
            this.label1.Text = "uids";
            // 
            // batchBtn
            // 
            this.batchBtn.Location = new System.Drawing.Point(389, 148);
            this.batchBtn.Name = "batchBtn";
            this.batchBtn.Size = new System.Drawing.Size(75, 23);
            this.batchBtn.TabIndex = 117;
            this.batchBtn.Text = "批量执行";
            this.batchBtn.UseVisualStyleBackColor = true;
            this.batchBtn.Click += new System.EventHandler(this.batchBtn_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 693);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AdminForm";
            this.Text = "AdminTool";
            this.Load += new System.EventHandler(this.AdminForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.treeMenuStrip.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.argsBox.ResumeLayout(false);
            this.cmdGroupBox.ResumeLayout(false);
            this.cmdGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.countNum)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMenu;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RichTextBox logRtb;
        private System.Windows.Forms.GroupBox argsBox;
        private System.Windows.Forms.GroupBox cmdGroupBox;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.TextBox cmdTbx;
        private System.Windows.Forms.Label cmdLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown countNum;
        private System.Windows.Forms.ComboBox uidCbx;
        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.Label uidLabel;
        private System.Windows.Forms.Button procBtn;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.FlowLayoutPanel argsPanel;
        private System.Windows.Forms.ComboBox hostCbx;
        private System.Windows.Forms.ToolTip widgetTip;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.TreeView adminTreeView;
        private System.Windows.Forms.ContextMenuStrip treeMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem expandMenu;
        private System.Windows.Forms.ToolStripMenuItem collapseMenu;
        private System.Windows.Forms.Button batchBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox uidListRTB;
    }
}

