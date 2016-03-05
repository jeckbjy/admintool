using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace AdminTool
{
    // 选项
    internal class AdminOption
    {
        public string Name = ""; // 显示数据
        public string Data = ""; // 使用数据

        public override string ToString()
        {
            return Name;
        }
    }

    enum BoxStyle
    {
        Text,       // 纯文本
        Combox,     // 默认下来列表和文本
        Option,     // 下拉列表
    }

    // 参数
    internal class AdminArg
    {
        public bool CanEdit = true;
        public bool Base64 = false;
        public bool CanEditOption = false;
        public uint Max = 20;   // 最多保存的选项
        public string Name;     // 名字
        public string Show;     // 显示名字
        public string Data;     // 编辑的数据
        public BoxStyle Style = BoxStyle.Combox;
        public List<AdminOption> Options = new List<AdminOption>();
        // 常用备选项
        public List<string> Items = new List<string>();

        public AdminOption FindOptionByData(string data)
        {
            foreach(var option in Options)
            {
                if (option.Data == data)
                    return option;
            }
            return null;
        }

        public bool AddItem(string item)
        {
            if (Style != BoxStyle.Combox || this.Items.Count > Max)
                return false;
            // 查找，不能重复
            foreach(var data in Items)
            {
                if (data == item)
                    return false;
            }
            Items.Add(item);
            return true;
        }
    }

    // 每个命令
    internal class AdminCmd
    {
        public string Group = "普通";    // 所属组
        public string Name = "";
        public string Cmd = "";
        public string Desc;
        public string Uid;
        // 至少有1个数据
        public List<AdminArg> Args = new List<AdminArg>();

        public AdminArg GetArg(string name)
        {
            for(int i = 0; i < Args.Count; ++i)
            {
                if (Args[i].Name == name)
                    return Args[i];
            }
            return null;
        }

        public string Concat()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Cmd);
            builder.Append(",");
            builder.Append(Uid);
            for (int i = 1; i < Args.Count; ++i )
            {
                AdminArg arg = Args[i];
                builder.Append(",");
                if(arg.Base64)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(arg.Data);
                    string str = Convert.ToBase64String(bytes);
                    builder.Append(str);
                }
                else
                {
                    builder.Append(arg.Data);
                }
            }
            return builder.ToString();
        }

        public string ConcatRecord()
        {
            // 去除了UID信息，因为不会解析uid,而且太长
            StringBuilder builder = new StringBuilder();
            builder.Append(Cmd);
            builder.Append(",$uid");
            for (int i = 1; i < Args.Count; ++i)
            {
                AdminArg arg = Args[i];
                builder.Append(",");
                if (arg.Base64)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(arg.Data);
                    string str = Convert.ToBase64String(bytes);
                    builder.Append(str);
                }
                else
                {
                    builder.Append(arg.Data);
                }
            }
            return builder.ToString();
        }

        public void Init()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Name);
            builder.Append(":");
            builder.Append(Cmd);
            builder.Append(",uid");
            for(int i = 1; i < Args.Count; ++i)
            {
                AdminArg arg = Args[i];
                builder.Append(",");
                builder.Append(arg.Name);
            }
            Desc = builder.ToString();
        }

        public void AddArg(string name, string data, bool canEdit)
        {
            AdminArg arg = new AdminArg();
            arg.Name = name;
            arg.Data = data;
            arg.CanEdit = canEdit;
            Args.Add(arg);
        }
        public override string ToString()
        {
            return Name;
        }

        public bool NeedRecord()
        {
            if (Args.Count == 0)
                return false;

            foreach(var arg in Args)
            {
                if(!arg.CanEdit)
                    continue;
                if (!string.IsNullOrEmpty(arg.Data) || arg.Items.Count > 0)
                    return true;
            }
            return false;
        }
    }

    // 配置文件信息
    class AdminCfg
    {
        public List<AdminCmd> Cmds = new List<AdminCmd>();
        public List<string> UIds = new List<string>();
        public List<string> Hosts = new List<string>();

        public bool AddUID(string uid)
        {
            if (UIds.IndexOf(uid) != -1)
                return false;
            UIds.Add(uid);
            return true;
        }

        public AdminCmd FindCmd(string key)
        {
            foreach(var cmd in Cmds)
            {
                if (cmd.Cmd == key)
                    return cmd;
            }
            return null;
        }

        public bool Load(string path)
        {
            // 加载配置文件
            if (!File.Exists(path))
                return false;
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes;
            XmlElement elem;
            // uid
            UIds.Clear();
            nodes = root.GetElementsByTagName("uid");
            foreach(var node in nodes)
            {
                elem = node as XmlElement;
                string uid = elem.InnerText;
                if (!uid.StartsWith("u"))
                    uid = "u" + uid;
                UIds.Add(uid);
            }

            // hosts
            Hosts.Clear();
            nodes = root.GetElementsByTagName("host");
            foreach(var node in nodes)
            {
                elem = node as XmlElement;
                string host = elem.InnerText;
                Hosts.Add(host);
            }

            Cmds.Clear();
            nodes = root.GetElementsByTagName("cmd");
            foreach(var node in nodes)
            {
                ReadCmd((XmlElement)node);
            }
            // 读取cmd
            return true;
        }

        private void ReadArray(XmlElement root, string name, List<string> values)
        {
            values.Clear();
            XmlNodeList nodes = root.GetElementsByTagName(name);
            if (nodes.Count == 0)
                return;
            XmlElement elem = (XmlElement)nodes[0];
            string attr = elem.GetAttribute("value");
            string[] tokens = attr.Split(new char[]{','});
            foreach(var token in tokens)
            {
                values.Add(token);
            }
        }

        private void ReadCmd(XmlElement node)
        {
            string[] tokens;
            AdminCmd cmd = new AdminCmd();
            Cmds.Add(cmd);
            cmd.Group = node.GetAttribute("group");
            cmd.Name = node.GetAttribute("name");
            //cmd.Cmd = node.GetAttribute("value");
            cmd.Desc = ReadAttribute(node, "note", "");
            string value = node.GetAttribute("value");
            tokens = value.Split(new char[] { ',' });
            cmd.Cmd = tokens[0];
            int tmp;
            for(int i = 1; i < tokens.Length; ++i)
            {
                AdminArg arg = new AdminArg();
                arg.Name = tokens[i].Trim();
                arg.Show = arg.Name;
                if (int.TryParse(arg.Name, out tmp))
                {
                    arg.CanEdit = false;
                    arg.Data = arg.Name;
                }
                else
                {
                    arg.CanEdit = true;
                }
                cmd.Args.Add(arg);
            }

            // 解析详细信息
            XmlNodeList param_nodes = node.GetElementsByTagName("arg");
            foreach(var param in param_nodes)
            {
                XmlElement elem = (XmlElement)param;
                string name = elem.GetAttribute("name");
                AdminArg arg = cmd.GetArg(name);
                if (arg == null)
                    continue;

                if (elem.HasAttribute("show"))
                    arg.Show = elem.GetAttribute("show");
                if (elem.HasAttribute("base64"))
                    arg.Base64 = true;
                if (elem.HasAttribute("style"))
                    arg.Style = ParseStyle(elem.GetAttribute("style"));

                if (elem.HasAttribute("limit"))
                    UInt32.TryParse(elem.GetAttribute("limit"), out arg.Max);

                if(elem.HasAttribute("options"))
                {
                    arg.Style = BoxStyle.Option;
                    tokens = elem.GetAttribute("options").Split(new char[]{'|', ','});
                    foreach(var option in tokens)
                    {
                        int pos = option.IndexOf(':');
                        if(pos == -1)
                            continue;
                        AdminOption op = new AdminOption();
                        op.Name = option.Substring(0, pos).Trim();
                        op.Data = option.Substring(pos + 1).Trim();
                        arg.Options.Add(op);
                    }
                }

                if(elem.HasAttribute("items"))
                {
                    arg.Style = BoxStyle.Combox;
                    tokens = elem.GetAttribute("items").Split(new char[] { ',', '|' });
                    foreach(var item in tokens)
                    {
                        arg.Items.Add(item.Trim());
                    }
                }
            }
        }

        private string ReadAttribute(XmlElement node, string name, string defaultValue)
        {
            if (!node.HasAttribute(name))
                return defaultValue;
            return node.GetAttribute(name);
        }

        private BoxStyle ParseStyle(string style)
        {
            switch(style)
            {
                case "text": return BoxStyle.Text;
                case "combox": return BoxStyle.Combox;
                case "option": return BoxStyle.Option;
            }
            return BoxStyle.Combox;
        }
    }

    // 命令管理
    class AdminMgr
    {
        public const string sConfigPath = "./cmd.xml";
        public const string sUserDataPath = "./data.xml";

        private static AdminMgr instance;
        private AdminForm m_form;

        public AdminCfg Config = new AdminCfg();
        public string LastUID = "";
        private bool m_dirty = false;
        private Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 当前连接
        private string m_host;

        public List<AdminCmd> Cmds
        {
            get
            {
                return Config.Cmds;
            }
        }

        public List<string> Uids
        {
            get
            {
                return Config.UIds;
            }
        }

        public List<string> Hosts
        {
            get
            {
                return Config.Hosts;
            }
        }

        public static AdminMgr Instance()
        {
            if(instance == null)
                instance = new AdminMgr();
            return instance;
        }

        public AdminMgr()
        {
            m_socket.Blocking = false;
            Load();
        }

        public void MarkDirty()
        {
            m_dirty = true;
        }

        public void Load()
        {
            try
            {
                LoadConfig();
                LoadUserData();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Save()
        {
            SaveUserData();
        }

        public bool LoadConfig()
        {
            if(!Config.Load(sConfigPath))
            {
                WriteLog("加载Admin配置失败!");
                return false;
            }

            return true;
        }

        private void LoadUserData()
        {
            // 加载自定义数据
            if (!File.Exists(sUserDataPath))
                return;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(sUserDataPath);
                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes;
                XmlElement  elem;
                nodes = root.GetElementsByTagName("last_uid");
                if(nodes.Count > 0)
                {
                    LastUID = (nodes[0] as XmlElement).InnerText;
                }
                nodes = root.GetElementsByTagName("uid");
                foreach(var node in nodes)
                {
                    elem = node as XmlElement;
                    Config.AddUID(elem.InnerText);
                }

                // 命令
                nodes = root.GetElementsByTagName("cmd");
                foreach(var node in nodes)
                {
                    elem = node as XmlElement;
                    string key = elem.GetAttribute("name");
                    AdminCmd cmd = Config.FindCmd(key);
                    if(cmd == null)
                        continue;
                    XmlNodeList arg_nodes = elem.GetElementsByTagName("arg");
                    foreach(var arg_node in arg_nodes)
                    {
                        XmlElement arg_elem = arg_node as XmlElement;
                        AdminArg arg = cmd.GetArg(arg_elem.GetAttribute("name"));
                        if (arg == null)
                            continue;
                        if (arg_elem.HasAttribute("data"))
                            arg.Data = arg_elem.GetAttribute("data");
                        XmlNodeList val_nodes = arg_elem.GetElementsByTagName("value");
                        foreach(var val_node in val_nodes)
                        {
                            arg.AddItem((val_node as XmlElement).InnerText);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                WriteLog(e.Message);
            }
        }

        private void SaveUserData()
        {
            // 保存历史数据
            if (!m_dirty)
                return;
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlElement root = doc.CreateElement("root");
                doc.AppendChild(root);
                // 写入lastuid
                if(!string.IsNullOrEmpty(LastUID))
                {
                    XmlElement last_uid_elem = doc.CreateElement("last_uid");
                    root.AppendChild(last_uid_elem);
                    last_uid_elem.InnerText = LastUID;
                }

                foreach(var uid in Config.UIds)
                {
                    XmlElement elem = doc.CreateElement("uid");
                    elem.InnerText = uid;
                    root.AppendChild(elem);
                }
                // 写入cmd
                foreach(var cmd in Config.Cmds)
                {
                    if (!cmd.NeedRecord())
                        continue;
                    XmlElement elem_cmd = doc.CreateElement("cmd");
                    elem_cmd.SetAttribute("name", cmd.Cmd);
                    root.AppendChild(elem_cmd);
                    foreach(var arg in cmd.Args)
                    {
                        if (!arg.CanEdit)
                            continue;
                        XmlElement elem_arg = doc.CreateElement("arg");
                        elem_cmd.AppendChild(elem_arg);
                        elem_arg.SetAttribute("name", arg.Name);
                        if (!string.IsNullOrEmpty(arg.Data))
                        {
                            elem_arg.SetAttribute("data", arg.Data);
                        }
                        foreach(var val in arg.Items)
                        {
                            XmlElement elem_val = doc.CreateElement("value");
                            elem_arg.AppendChild(elem_val);
                            elem_val.InnerText = val;
                        }
                    }
                }
                doc.Save(sUserDataPath);
            }
            catch(Exception e)
            {
                WriteLog(e.Message);
            }
        }

        public void setForm(AdminForm form)
        {
            this.m_form = form;
        }

        public void WriteLog(string info)
        {
            if(m_form != null)
                m_form.InvokeWriteLog(info);
        }

        public bool UpdateUID(string uid)
        {
            if(LastUID != uid)
            {
                LastUID = uid;
                m_dirty = true;
            }
            if(Config.AddUID(uid))
            {
                m_dirty = true;
                return true;
            }
            return false;
        }

        public bool Connect()
        {
            string host = m_form.Host;
            if (string.IsNullOrEmpty(host))
                return false;
            if(m_socket.Connected)
            {// 校验是否发生改变
                if (host == m_host)
                    return true;
                WriteLog(string.Format("Disconnect {0}", m_host));
                m_socket.Disconnect(true);
            }
            m_host = host;
            // 尝试连接
            try
            {
                // 解析
                string ip;
                int port;

                int pos = host.IndexOf('(');
                if (pos != -1)
                    host = host.Substring(0, pos);
                pos = host.IndexOf(':');
                if(pos != -1)
                {
                    ip = host.Substring(0, pos);
                    port = Int32.Parse(host.Substring(pos + 1));
                }
                else
                {
                    ip = host;
                    port = 2020;
                }

                m_socket.BeginConnect(ip, port,
                    (ar) =>
                    {
                        try
                        {
                            m_socket.EndConnect(ar);
                            WriteLog(string.Format("Connect {0} succeed;", m_host));
                            Recv();
                        }
                        catch(Exception e)
                        {
                            WriteLog(e.Message);
                        }

                    }, null);
            }
            catch(Exception e)
            {
                WriteLog(e.Message);
            }
            return m_socket.Connected;
        }

        private bool CheckConnect()
        {
            if (!m_socket.Connected)
            {
                Connect();
                if(!m_socket.Connected)
                    WriteLog("需要先连接服务器");
            }
            return m_socket.Connected;
        }

        private void Recv()
        {
            try
            {
                byte[] buffer = new byte[1024];
                m_socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    (ar) => 
                    {
                        try
                        {
                            var length = m_socket.EndReceive(ar);
                            if (length > 0)
                            {
                                byte[] datas = (byte[])ar.AsyncState;
                                //读取出来消息内容
                                var msg = Encoding.UTF8.GetString(datas, 0, length);
                                WriteLog("recv msg:"+ msg);
                                Recv();
                            }
                        }
                        catch(Exception e)
                        {
                            m_socket.Disconnect(true);
                            WriteLog(e.Message);
                        }
    
                    }, buffer);
            }
            catch(Exception e)
            {
                m_socket.Disconnect(true);
                WriteLog(e.Message);
            }
        }

        public void Send(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            // 必须以\n结尾
            if (!msg.EndsWith("\n"))
                msg += "\n";
            if (!CheckConnect())
                return;
            WriteLog("send cmd:" + msg);
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            Send(buffer);
        }

        private void Send(byte[] buffer)
        {
            try
            {
                m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                    (ar) =>
                    {
                        try
                        {
                            m_socket.EndSend(ar);
                            //WriteLog(string.Format("send bytes len = {0}", len));
                        }
                        catch(Exception e)
                        {
                            WriteLog(e.Message);
                        }

                    }, null);
            }
            catch(Exception e)
            {
                m_socket.Disconnect(true);
                WriteLog(e.Message);
            }
        }

        // 执行命令
        public bool Execute(string msg, int count = 1)
        {
            if (!CheckConnect())
                return false;
            for (int i = 0; i < count; ++i)
                Send(msg);
            return true;
        }
    }
}
