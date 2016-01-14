using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminTool
{
    public partial class AdminForm : Form
    {
        delegate void Callback();
        AdminMgr m_mgr;

        public string Host
        {
            get
            {
                return hostCbx.Text;
            }
        }

        public int Port
        {
            get
            {
                return portTbx.Text.Length == 0 ? 0 : Int32.Parse(portTbx.Text);
            }
        }
        public AdminForm()
        {
            m_mgr = AdminMgr.Instance();
            m_mgr.setForm(this);
            InitializeComponent();
            InitForm();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            this.FormClosing += frmMain_FormClosing;
            widgetTip.SetToolTip(hostLabel, "服务器地址");
            widgetTip.SetToolTip(cmdLabel, "命令行直接发送");
            widgetTip.SetToolTip(uidLabel, "prefix:u:uid,o:openid,r:region,g:gameid,a:all");
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_mgr.Save();
        }

        public void InitForm()
        {
            // 初始化命令列表
            Dictionary<string, TreeNode> nodes = new Dictionary<string, TreeNode>();
            List<AdminCmd> cmds = m_mgr.Cmds;
            for (int i = 0; i < cmds.Count; ++i)
            {
                AdminCmd cmd = cmds[i];
                TreeNode node = new TreeNode();
                node.Text = cmd.Name;
                node.Tag = cmd;
                // root?
                TreeNode parent;
                if (!nodes.TryGetValue(cmd.Group, out parent))
                {
                    parent = new TreeNode();
                    parent.Text = cmd.Group;
                    parent.Tag = null;
                    nodes[cmd.Group] = parent;
                    adminTreeView.Nodes.Add(parent);
                }
                parent.Nodes.Add(node);
            }
            // 展开所有
            adminTreeView.ExpandAll();

                // init host
                hostCbx.Items.Clear();
            List<string> hosts = m_mgr.HostList;
            if(hosts.Count > 0)
            {
                for (int i = 0; i < hosts.Count; ++i)
                {
                    hostCbx.Items.Add(hosts[i]);
                }

                hostCbx.Text = hosts[0];
            }
            // init uid
            UpdateUID();
        }

        public void UpdateUID()
        {
            // 初始化uid
            uidCbx.Items.Clear();
            List<string> uids = m_mgr.UIds;
            if(uids.Count > 0)
            {
                for (int i = 0; i < uids.Count; ++i)
                {
                    string uid = uids[i];
                    if (string.IsNullOrEmpty(uid))
                        continue;
                    uidCbx.Items.Add(uid);
                }
            }
            if(!string.IsNullOrEmpty(m_mgr.LastUID))
            {
                uidCbx.Text = m_mgr.LastUID;
            }
            else if(uidCbx.Items.Count > 0)
            {
                uidCbx.SelectedIndex = 0;
            }
        }

        public void UpdateRecord()
        {
            // 更新当前的record
            lastAdminLbx.Items.Clear();
            TreeNode node = adminTreeView.SelectedNode;
            object obj = node.Tag;
            if (obj == null)
                return;
            AdminCmd cmd = obj as AdminCmd;
            List<AdminRecord> records = m_mgr.FindRecord(cmd.Name);
            if (records == null || records.Count == 0)
                return;
            for(int i = 0; i < records.Count; ++i)
            {
                lastAdminLbx.Items.Add(records[i]);
            }
        }

        public void InvokeWriteLog(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            Callback fun = new Callback(delegate()
            {
                WriteLog(msg);
            });
            logRtb.BeginInvoke(fun);
        }

        private void WriteLog(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            logRtb.AppendText(msg);
            if(!msg.EndsWith("\n"))
                logRtb.AppendText("\n");
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            m_mgr.Connect();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            m_mgr.Send(cmdTbx.Text);
        }

        private void procBtn_Click(object sender, EventArgs e)
        {
            // 执行
            TreeNode node = adminTreeView.SelectedNode;
            if (node == null)
                return;
            AdminCmd cmd = (AdminCmd)node.Tag;
            if(cmd == null)
            {
                WriteLog("请先选中一条指令");
                return;
            }
            // set uid
            if (string.IsNullOrEmpty(uidCbx.Text))
            {
                WriteLog("uid不能为空");
                return;
            }
            cmd.Uid = uidCbx.Text;
            // 回填数据
            for(int i = 0; i < argsPanel.Controls.Count; ++i)
            {
                ArgBox box = (ArgBox)argsPanel.Controls[i];
                AdminArg arg = (AdminArg)box.Tag;
                arg.Data = box.ArgData;
            }
            // 发送gm
            string msg = cmd.Concat();
            bool ret = m_mgr.Execute(msg, (int)countNum.Value);
            if(ret)
            {// 记录record
                if(argsPanel.Controls.Count > 0)
                {
                    AdminRecord record = new AdminRecord();
                    for (int i = 0; i < argsPanel.Controls.Count; ++i)
                    {
                        AdminArg arg = (argsPanel.Controls[i] as ArgBox).Tag as AdminArg;
                        record.Args.Add(arg.Data);
                    }
                    record.Cmd = cmd.ConcatRecord();
                    if (!m_mgr.AddRecord(cmd.Name, record))
                    {
                        UpdateRecord();
                    }
                }
                // check uid
                if (m_mgr.UpdateUID(cmd.Uid))
                {
                    UpdateUID();
                }
            }

        }

        private void adminTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateRecord();
            TreeNode node = adminTreeView.SelectedNode;
            statusLabel.Text = "Group:"+ node.Text;
            argsPanel.Controls.Clear();
            if (node.Tag == null)
                return;
            AdminCmd cmd = (AdminCmd)node.Tag;
            statusLabel.Text = cmd.Desc;
            // 初始化arg
            for (int i = 0; i < cmd.Args.Count; ++i)
            {
                AdminArg arg = cmd.Args[i];
                if (!arg.CanEdit)
                    continue;
                ArgBox box = new ArgBox();
                box.ArgName = arg.Name;
                box.ArgData = arg.Data;
                box.Tag = arg;
                if (arg.HasOptions)
                    box.AddOptions(arg.Options, arg.CanEditOption);
                argsPanel.Controls.Add(box);
            }
        }

        private void lastAdminLbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 自动填充数据
            object item = lastAdminLbx.SelectedItem;
            if (item == null)
                return;
            AdminRecord record = item as AdminRecord;
            if (record.Args.Count != argsPanel.Controls.Count)
                return;
            // 设置数据
            for(int i = 0; i < record.Args.Count; ++i)
            {
                ArgBox box = argsPanel.Controls[i] as ArgBox;
                box.ArgData = record.Args[i];
            }
        }

        private void expandMenu_Click(object sender, EventArgs e)
        {
            adminTreeView.ExpandAll();
        }

        private void collapseMenu_Click(object sender, EventArgs e)
        {
            adminTreeView.CollapseAll();
        }

        private void updateMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = adminTreeView.SelectedNode;
            if (node == null)
                return;
            AdminCmd cmd = (AdminCmd)node.Tag;
            if (cmd == null)
                return;
            m_mgr.UpdateRecord(cmd.Name);
            UpdateRecord();
        }

        private void resetMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = adminTreeView.SelectedNode;
            if (node == null)
                return;
            AdminCmd cmd = (AdminCmd)node.Tag;
            if (cmd == null)
                return;
            m_mgr.ClearRecord(cmd.Name);
            UpdateRecord();
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (lastAdminLbx.SelectedIndex < 0)
                return;
            TreeNode node = adminTreeView.SelectedNode;
            if (node == null)
                return;
            AdminCmd cmd = (AdminCmd)node.Tag;
            if (cmd == null)
                return;
            m_mgr.DeleteRecord(cmd.Name, lastAdminLbx.SelectedIndex);
            UpdateRecord();
        }
    }
}
