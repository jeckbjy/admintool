using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace AdminTool
{
    // 选项
    class AdminOption
    {
        public string Name; // 显示数据
        public string Data; // 使用数据

        public override string ToString()
        {
            return Name;
        }
    }
    // 参数
    class AdminArg
    {
        public bool CanEdit = true;
        public bool Base64 = false;
        public bool CanEditOption = false;
        public string Name;
        public string Data; // 初始值
        public List<AdminOption> Options = new List<AdminOption>();

        public bool HasOptions
        {
            get
            {
                return Options.Count > 0;
            }
        }
    }

    // 每个命令
    class AdminCmd
    {
        public string Group = "普通";    // 所属组
        public string Name;
        public string Cmd;
        public string Uid;
        public string Desc;
        // 至少有1个数据
        public List<AdminArg> Args = new List<AdminArg>();

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
            //if(Show == null)
            //    Show = Name + "[" + Cmd + "]";
            //return Show;
        }
    }

    // 记录
    class AdminRecord : IComparable
    {
        public string Cmd = null;  // 实际发送的gm
        public List<string> Args = new List<string>();  // 参数,用于回填
        public uint Count;  // 调用次数
        public uint Time;   // 上次调用时间
        // 用于显示
        public override string ToString()
        {
            return Cmd;
        }

        public int CompareTo(object obj)
        {
            AdminRecord other = obj as AdminRecord;
            if (Count == other.Count)
                return other.Time.CompareTo(Time);
            return other.Count.CompareTo(Count);
        }
    }

    // 配置文件信息
    class AdminCfg
    {
        public List<AdminCmd> Cmds = new List<AdminCmd>();
        public List<string> UIds = new List<string>();
        public List<string> Hosts = new List<string>();
        public int Port = 2020;
        public int RecordMax = 10;      // 最多保留10条

        public bool Load(string path)
        {
            // 加载配置文件
            if (!File.Exists(path))
                return false;
            StreamReader reader = new StreamReader(path, Encoding.Default);
            while (!reader.EndOfStream)
            {
                string str = reader.ReadLine().Trim();
                if (string.IsNullOrEmpty(str))
                    continue;
                char flag = str[0];
                // 注释行
                if (flag == '#')
                    continue;
                int pos = str.IndexOf('=');
                if (pos == -1)
                    return false;
                string tags = str.Substring(0, pos).Trim();
                string info = str.Substring(pos + 1).Trim();
                if(tags == "port")
                {
                    Port = Int32.Parse(info);
                }
                else
                {
                    string[] tokens = info.Split(',');
                    // trim
                    for (int i = 0; i < tokens.Length; ++i )
                    {
                        tokens[i] = tokens[i].Trim();
                    }
                    // check
                    switch (tags)
                    {
                        case "uids":
                            {
                                UIds.AddRange(tokens);
                            }
                            break;
                        case "host":
                            {
                                Hosts.AddRange(tokens);
                            }
                            break;
                        case "cmd":
                            {
                                if (tokens.Length < 2)
                                    return false;
                                ParseCmd(tokens);
                            }
                            break;
                    }
                }
            }
            reader.Close();
            return true;
        }
        private void ParseCmd(string[] tokens)
        {
            // 解析时使用
            int pos;
            AdminCmd cmd = new AdminCmd();
            Cmds.Add(cmd);
            cmd.Group = tokens[0];
            cmd.Name = tokens[1];
            cmd.Cmd = tokens[2];
            if(tokens.Length > 3)
            {
                int args_start = tokens[3] == "$uid" ? 3 : 2;
                // 解析参数
                for (int i = args_start; i < tokens.Length; ++i)
                {
                    string arg_str = tokens[i];
                    // 忽略无效
                    if (string.IsNullOrEmpty(arg_str))
                        continue;
                    AdminArg arg = new AdminArg();
                    cmd.Args.Add(arg);
                    // 解析属性
                    pos = arg_str.IndexOf('[');
                    if(pos != -1)
                    {
                        int back_pos = arg_str.IndexOf(']', pos);
                        if (back_pos == -1)
                            return;
                        string props = arg_str.Substring(pos + 1, back_pos - pos - 1);
                        arg_str = arg_str.Substring(0, pos);
                        ParseProperty(arg, props);
                    }
                    // 说明是变量
                    if (arg_str[0] == '$')
                    {
                        arg.CanEdit = true;
                        arg.Base64 = false;
                    }
                    else if (arg_str[0] == '@')
                    {// base64
                        arg.CanEdit = true;
                        arg.Base64 = true;
                    }
                    else
                    {
                        arg.CanEdit = false;
                        arg.Base64 = false;
                    }

                    if (arg.CanEdit)
                    {
                        pos = arg_str.IndexOf('=');
                        if (pos == -1)
                        {//无默认值 
                            arg.Name = arg_str.Substring(1).Trim();
                        }
                        else
                        {// 含有默认值
                            arg.Name = arg_str.Substring(1, pos - 1).Trim();
                            arg.Data = arg_str.Substring(pos + 1).Trim();
                        }
                    }
                    else
                    {// 不可编辑，固定值
                        arg.Name = arg.Data = arg_str;
                    }
                }
            }
            cmd.Init();
        }

        private void ParseProperty(AdminArg arg, string props)
        {
            int pos;
            // 解析属性
            string[] tokens = props.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < tokens.Length; ++i)
            {
                string key;
                string value;
                string token = tokens[i].Trim();
                pos = token.IndexOf('=');
                if(pos == -1)
                {
                    key = token;
                    value = null;
                }
                else
                {
                    key = token.Substring(0, pos).Trim();
                    value = token.Substring(pos + 1).Trim();
                }
                // 备选
                if(value != null)
                {
                    if (key == "options" || key == "selected")
                    {
                        arg.CanEditOption = key == "options" ? false : true;
                        string[] opt_tokens = value.Split('|');
                        for (int j = 0; j < opt_tokens.Length; ++j)
                        {
                            AdminOption option = new AdminOption();
                            arg.Options.Add(option);
                            string opt_str = opt_tokens[j].Trim();
                            pos = opt_str.IndexOf(":");
                            if (pos == -1)
                            {// 一致
                                option.Name = opt_str;
                                option.Data = opt_str;
                            }
                            else
                            {
                                option.Name = opt_str.Substring(0, pos).Trim();
                                option.Data = opt_str.Substring(pos + 1).Trim();
                            }
                        }
                    }
                }
            }
        }
    }

    // 命令管理
    class AdminMgr
    {
        public const string sConfigPath = "./cmd.cfg";
        public const string sUserDataPath = "./user.data";
        public const string sUIDsDataPath = "./uids.data";  // 所有最近使用uid
        // 分隔符
        private const char record_code = (char)0x1E;
        private const char unit_code = (char)0x1F;

        private static AdminMgr instance;
        private AdminForm m_form;

        public AdminCfg Config = new AdminCfg();
        public List<string> UIds = new List<string>();      // 最近使用的，以及配置的
        public string LastUID = "";
        public Dictionary<string, List<AdminRecord>> Records = new Dictionary<string, List<AdminRecord>>();
        private Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 当前连接
        private string m_host;
        private int m_port;

        public List<AdminCmd> Cmds
        {
            get
            {
                return Config.Cmds;
            }
        }

        public List<string> HostList
        {
            get
            {
                return Config.Hosts;
            }
        }

        public static uint TimeNow()
        {
            TimeSpan span = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (uint)span.TotalSeconds;
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

        public void Load()
        {
            LoadConfig();
            LoadUIDData();
            LoadUserData();
            foreach(var uid in Config.UIds)
            {
                if (UIds.Contains(uid))
                    continue;
                UIds.Add(uid);
            }
        }

        public void Save()
        {
            SaveUserData();
            SaveUIDData();
        }

        private bool LoadConfig()
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
                Records.Clear();
                StreamReader reader = new StreamReader(sUserDataPath, Encoding.Default);
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    string[] tokens = str.Split(record_code);
                    if (tokens[0] == "record")
                    {
                        if (tokens.Length < 3)
                            continue;
                        List<AdminRecord> record_list = new List<AdminRecord>();
                        string key = tokens[1];
                        for (int i = 2; i < tokens.Length; ++i)
                        {
                            string[] record_tokens = tokens[i].Split(unit_code);
                            if (record_tokens.Length < 2)
                                continue;
                            AdminRecord record = new AdminRecord();
                            record.Cmd = record_tokens[0];
                            for (int j = 1; j < record_tokens.Length; ++j)
                            {
                                record.Args.Add(record_tokens[j]);
                            }
                            record_list.Add(record);
                        }
                        Records[key] = record_list;
                    }
                }
                reader.Close();
            }
            catch(Exception e)
            {
                WriteLog(e.Message);
            }
        }

        private void SaveUserData()
        {
            // 保存历史数据
            try
            {
                FileStream stream = new FileStream(sUserDataPath, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(stream, Encoding.Default);
                foreach (var kv in Records)
                {
                    if (kv.Value.Count == 0)
                        continue;
                    List<AdminRecord> record_list = kv.Value;
                    writer.Write("record");
                    writer.Write(record_code);
                    writer.Write(kv.Key);
                    for (int i = 0; i < record_list.Count; ++i)
                    {
                        writer.Write(record_code);
                        AdminRecord record = record_list[i];
                        writer.Write(record.Cmd);
                        // 写入args
                        for (int j = 0; j < record.Args.Count; ++j)
                        {
                            writer.Write(unit_code);
                            writer.Write(record.Args[j]);
                        }
                    }
                    // 换行
                    writer.WriteLine();
                }
                writer.Flush();
                writer.Close();
            }
            catch(Exception e)
            {
                WriteLog(e.Message);
            }
        }

        private void LoadUIDData()
        {
            if (!File.Exists(sUIDsDataPath))
                return;
            try
            {
                StreamReader reader = new StreamReader(sUIDsDataPath, Encoding.Default);
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    string[] tokens = str.Split(',');
                    if (tokens.Length == 0)
                        continue;
                    LastUID = tokens[0];
                    for(int i = 1; i < tokens.Length; ++i)
                    {
                        UIds.Add(tokens[i]);
                    }
                }
                reader.Close();
            }
            catch(Exception e)
            {
                WriteLog(e.Message);
            }
        }

        private void SaveUIDData()
        {
            try
            {
                FileStream stream = new FileStream(sUIDsDataPath, FileMode.OpenOrCreate);
                StreamWriter writer = new StreamWriter(stream, Encoding.Default);
                writer.Write(LastUID);
                // 先写入最近
                foreach(var uid in UIds)
                {
                    writer.Write(",");
                    writer.Write(uid);
                }
                writer.Flush();
                writer.Close();
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
            LastUID = uid;
            // 记录uid
            if (!UIds.Contains(uid))
            {
                UIds.Add(uid);
                return true;
            }
            return false;
        }

        public List<AdminRecord> FindRecord(string cmd)
        {
            List<AdminRecord> record_list;
            if (Records.TryGetValue(cmd, out record_list))
                return record_list;
            return null;
        }

        public void ClearRecord(string cmd)
        {
            Records.Remove(cmd);
        }

        public void DeleteRecord(string cmd, int index)
        {
            List<AdminRecord> records;
            if (!Records.TryGetValue(cmd, out records))
                return;
            if (index >= records.Count)
                return;
            records.RemoveAt(index);
        }

        public void UpdateRecord(string cmd)
        {
            List<AdminRecord> records;
            if (!Records.TryGetValue(cmd, out records))
                return;
            records.Sort();
            if(records.Count > Config.RecordMax)
            {
                records.RemoveRange(Config.RecordMax, records.Count - Config.RecordMax);
            }
        }

        public bool AddRecord(string cmd, AdminRecord record)
        {
            // 记录record
            record.Count = 1;
            record.Time = TimeNow();
            bool need_find = true;
            List<AdminRecord> record_list;
            if(!Records.TryGetValue(cmd, out record_list))
            {
                record_list = new List<AdminRecord>();
                Records[cmd] = record_list;
                need_find = false;
            }
            if(need_find)
            {
                // 查找是否已经存在
                for (int i = 0; i < record_list.Count; ++i)
                {
                    AdminRecord node = record_list[i];
                    if (node.Cmd == record.Cmd)
                    {
                        node.Count++;
                        node.Time = record.Time;
                        return true;
                    }
                }
            }
            // 没有找到,添加
            record_list.Add(record);
            return false;
        }

        public bool Connect()
        {
            string host = m_form.Host;
            int port = m_form.Port;
            if(m_socket.Connected)
            {// 校验是否发生改变
                if (host == m_host && port == m_port)
                    return true;
                WriteLog(string.Format("Disconnect {0}:{1}", m_host, m_port));
                m_socket.Disconnect(true);
            }
            m_host = host;
            m_port = port;
            // 尝试连接
            try
            {
                //m_socket.BeginConnect(host, port, OnConnect, null);
                m_socket.BeginConnect(m_host, m_port,
                    (ar) =>
                    {
                        try
                        {
                            m_socket.EndConnect(ar);
                            WriteLog(string.Format("Connect {0}:{1} succeed;", m_host, m_port));
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
                            //int len = m_socket.EndSend(ar);
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
