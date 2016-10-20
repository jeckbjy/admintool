using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminTool
{
    public partial class ArgBox : UserControl
    {
        public ArgBox()
        {
            InitializeComponent();
        }

        public string ArgName
        {
            get
            {
                return nameLabel.Text;
            }
            set
            {
                this.nameLabel.Text = value;
            }
        }

        private string SelectedText
        {
            get
            {
                object obj = dataBox.SelectedItem;
                if (obj == null)
                    return null;
                if (obj.GetType() == typeof(AdminOption))
                {
                    AdminOption op = (AdminOption)obj;
                    return op.Data;
                }
                else
                {
                    return obj.ToString();
                }
            }
        }

        // 获取数据
        public string ArgData
        {
            get
            {
                AdminArg arg = this.Tag as AdminArg;
                if (arg == null)
                    return "";
                string text;
                if(this.dataBox.DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    text = SelectedText;
                }
                else
                {
                    text = dataBox.Text;
                }

                if(arg.Base64)
                {
                    byte[] bytes = Encoding.Default.GetBytes(text);
                    text = Convert.ToBase64String(bytes);
                }
                return text;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                if(this.Style == ComboBoxStyle.DropDownList)
                {
                    AdminArg arg = this.Tag as AdminArg;
                    if (arg == null)
                        return;
                    AdminOption option = arg.FindOptionByData(value);
                    if(option != null)
                        dataBox.Text = option.Name;
                }
                else
                {
                    this.dataBox.Text = value;
                }
            }
        }

        // 设置风格
        public ComboBoxStyle Style
        {
            get
            {
                return dataBox.DropDownStyle;
            }
            set
            {
                this.dataBox.DropDownStyle = value;
            }
        }

        internal void Init(AdminArg arg)
        {
            // 先清空
            this.dataBox.Text = null;
            this.dataBox.Items.Clear();
            dataBox.SuspendLayout();
            switch(arg.Style)
            {
                case BoxStyle.Text:
                    {
                        this.Style = ComboBoxStyle.Simple;
                    }
                    break;
                case BoxStyle.Combox:
                    {// 下拉菜单
                        this.Style = ComboBoxStyle.DropDown;
                        if(arg.Items.Count > 0)
                        {
                            foreach (var item in arg.Items)
                            {
                                dataBox.Items.Add(item);
                            }
                            this.dataBox.SelectedIndex = 0;
                        }
                    }
                    break;
                case BoxStyle.Option:
                    {
                        this.Style = ComboBoxStyle.DropDownList;
                        if(arg.Options != null && arg.Options.Count > 0)
                        {
                            foreach (var option in arg.Options)
                            {
                                dataBox.Items.Add(option);
                            }
                            this.dataBox.SelectedIndex = 0;
                        }
                    }
                    break;
            }

            this.Tag = arg;
            this.ArgName = arg.Show;
            this.ArgData = arg.Data;
            dataBox.ResumeLayout();
        }
    }
}
