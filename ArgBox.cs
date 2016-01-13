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
                if (dataBox.DropDownStyle == ComboBoxStyle.DropDownList)
                    return SelectedText;
                return dataBox.Text;
            }
            set
            {
                this.dataBox.Text = value;
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

        internal void AddOptions(List<AdminOption> options, bool canEdit)
        {
            for(int i = 0; i < options.Count; ++i)
            {
                dataBox.Items.Add(options[i]);
            }
            dataBox.SelectedIndex = 0;
            if (canEdit)
                Style = ComboBoxStyle.DropDown;
            else
                Style = ComboBoxStyle.DropDownList;
        }
    }
}
