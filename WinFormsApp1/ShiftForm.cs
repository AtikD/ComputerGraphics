using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ShiftForm : Form
    {
        public event EventHandler<int> ShiftChanged;
        public ShiftForm()
        {
            InitializeComponent();
        }

        private void ShiftForm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShiftChanged?.Invoke(this, trackBar1.Value);
            this.Hide();
        }

        public void ShowAtMenuItem(ToolStripMenuItem menuItem)
        {
            ToolStrip parentMenu = menuItem.Owner;

            Rectangle itemRect = menuItem.Bounds;
            Point screenPoint = parentMenu.PointToScreen(new Point(itemRect.Right, itemRect.Bottom));

            this.Location = screenPoint;
            this.Show();
            this.Activate();
        }
    }
}
