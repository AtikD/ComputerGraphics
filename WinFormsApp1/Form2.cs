using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        public event EventHandler<int> BrightnessChanged;

        public Form2()
        {
            InitializeComponent();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BrightnessChanged?.Invoke(this, trackBar1.Value);
            this.Hide();
        }

        private void Form2_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        public void ShowAtMenuItem(ToolStripMenuItem menuItem)
        {
            ToolStrip parentMenu = menuItem.Owner;
            Rectangle itemRect = menuItem.Bounds;
            Point screenPoint = parentMenu.PointToScreen(new Point(itemRect.Right, itemRect.Bottom));
            this.ClientSize = new Size(200, 100);
            this.Location = screenPoint;
            this.Show();
            this.Activate();
        }
    }
}