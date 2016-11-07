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
            adminTreeView.Nodes.Clear();

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
            List<string> hosts = m_mgr.Hosts;
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
            List<string> uids = m_mgr.Uids;
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
            AdminCmd cmd = GetCmd();
            if (cmd == null)
                return;

            // set uid
            if (string.IsNullOrEmpty(uidCbx.Text))
            {
                WriteLog("uid不能为空");
                return;
            }

            // 计算uid
            string uid = uidCbx.Text;
            if(char.IsDigit(uid[0]))
                uid = "u"+uid;
            cmd.Uid = uid;
            if (m_mgr.UpdateUID(uid))
                UpdateUID();

            // 发送gm
            string msg = cmd.Concat();
            m_mgr.Execute(msg, (int)countNum.Value);
        }

        // 批量执行
        private void batchBtn_Click(object sender, EventArgs e)
        {
            AdminCmd cmd = GetCmd();
            if (cmd == null)
                return;

            string uidstr = uidListRTB.Text;
            if(string.IsNullOrEmpty(uidstr))
            {
                WriteLog("uid不能为空");
                return;
            }

            string[] uids = uidstr.Split(new char[] { '\n', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if(uids.Length == 0)
            {
                WriteLog("uid不能为空");
                return;
            }

            for(int i = 0; i < uids.Length; ++i)
            {
                string uid = uids[i];
                if(char.IsDigit(uid[0]))
                    uid = "u"+uid;
                cmd.Uid = uid;
                string msg = cmd.Concat();
                m_mgr.Execute(msg, 1);
            }
        }

        private AdminCmd GetCmd()
        {
            // 执行
            TreeNode node = adminTreeView.SelectedNode;
            if (node == null)
                return null;
            AdminCmd cmd = (AdminCmd)node.Tag;
            if (cmd == null)
            {
                WriteLog("请先选中一条指令");
                return null;
            }

            // 回填数据
            for (int i = 0; i < argsPanel.Controls.Count; ++i)
            {
                ArgBox box = (ArgBox)argsPanel.Controls[i];
                AdminArg arg = (AdminArg)box.Tag;
                arg.Data = box.ArgData;
                if (arg.AddItem(arg.Data))
                {
                    m_mgr.MarkDirty();
                }
            }

            return cmd;
        }

        private void adminTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = adminTreeView.SelectedNode;
            statusLabel.Text = "Group:"+ node.Text;
            argsPanel.SuspendLayout();
            foreach(Control control in argsPanel.Controls)
            {
                control.Visible = false;
            }

            if(node.Tag != null)
            {
                AdminCmd cmd = (AdminCmd)node.Tag;
                statusLabel.Text = cmd.Desc;
                // 先创建足够多
                if(argsPanel.Controls.Count < cmd.Args.Count)
                {
                    int count = cmd.Args.Count - argsPanel.Controls.Count;
                    for(int i = 0; i < count; ++i)
                    {
                        ArgBox box = new ArgBox();
                        box.Visible = false;
                        argsPanel.Controls.Add(box);
                    }
                }
                // 初始化arg
                int box_index = 0;
                for (int i = 0; i < cmd.Args.Count; ++i)
                {
                    AdminArg arg = cmd.Args[i];
                    if (!arg.CanEdit)
                        continue;
                    ArgBox box = argsPanel.Controls[box_index++] as ArgBox;
                    box.Visible = true;
                    box.Init(arg);
                }
            }
            argsPanel.ResumeLayout();
            //argsPanel.Controls.Clear();
        }

        private void expandMenu_Click(object sender, EventArgs e)
        {
            adminTreeView.ExpandAll();
        }

        private void collapseMenu_Click(object sender, EventArgs e)
        {
            adminTreeView.CollapseAll();
        }

        private void loadMenu_Click(object sender, EventArgs e)
        {
            m_mgr.LoadConfig();
            InitForm();
        }

    }
}
