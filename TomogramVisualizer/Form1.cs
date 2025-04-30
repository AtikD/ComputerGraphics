using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace TomogramVisualizer
{
    public partial class Form1 : Form
    {
        Bin bin;
        View view;
        bool loaded;
        bool needReload;
        int currentLayer;
        int FrameCount;
        int mode;
        DateTime NextFPSUpdate;

        public Form1()
        {
            InitializeComponent();
            bin = new Bin();
            view = new View();
            loaded = false;
            needReload = true;
            mode = 0;
            currentLayer = 0;
            NextFPSUpdate = DateTime.Now.AddSeconds(1);
        }

        void displayFPS()
        {
            if (DateTime.Now > NextFPSUpdate)
            {
                this.Text = "Tomogram Visualizer (fps: " + FrameCount + ")";
                FrameCount = 0;
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
            }
            FrameCount++;
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (loaded)
            {
                switch (mode)
                {
                    case 0:
                        view.DrawQuads(currentLayer);
                        break;
                    case 1:
                        if (needReload)
                        {
                            view.generateTextureImage(currentLayer);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.drawTexture();
                        break;
                    case 2:
                        view.drawQuadStrip(currentLayer);
                        break;
                }
            }
            glControl1.SwapBuffers();
        }
        

        void Application_Idle(object sender, EventArgs e)
        {
            while (System.Windows.Forms.Application.OpenForms.Count > 0 && glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle += Application_Idle;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            if (mode == 1)
                needReload = true;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BIN files|*.bin";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBin(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                trackBar1.Maximum = Bin.Z - 1;
            }
        }

        private void квадратамиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = 0;
            квадратамиToolStripMenuItem.Enabled = false;
            текстурамиToolStripMenuItem.Enabled = true;
            quadStripToolStripMenuItem.Enabled = true;
        }

        private void текстурамиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = 1;
            квадратамиToolStripMenuItem.Enabled = true;
            текстурамиToolStripMenuItem.Enabled = false;
            quadStripToolStripMenuItem.Enabled = true;
        }

        private void quadStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = 2;
            квадратамиToolStripMenuItem.Enabled = true;
            текстурамиToolStripMenuItem.Enabled = true;
            quadStripToolStripMenuItem.Enabled = false;
        }
    }
}
