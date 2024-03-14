using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace desktop_box
{
    public partial class FloatingTextForm : Form
    {
        private Timer timer;

        public FloatingTextForm(string text, int timeout)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = this.BackColor; // 设置背景色为透明

            Label label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Font = new Font("Arial", 20);
            label.ForeColor = Color.Black;
            label.BackColor = Color.FromArgb(10, Color.White);
            label.Location = new Point(5, 5); // 设置文字位置
            this.Controls.Add(label);

            timer = new Timer();
            timer.Interval = timeout;
            timer.Tick += (sender, e) => { this.Close(); };
            timer.Start();
        }
    }

}
