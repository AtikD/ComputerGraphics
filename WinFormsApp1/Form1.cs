using System.ComponentModel;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap image;
        Bitmap newImage;
        BrightnessForm brightnessPopup;
        ShiftForm shiftPopup;
        public Form1()
        {
            InitializeComponent();

            brightnessPopup = new BrightnessForm();
            shiftPopup = new ShiftForm();
            brightnessPopup.BrightnessChanged += brightnessPopup_BrightnessChanged;
            shiftPopup.ShiftChanged += shiftPopup_ShiftChanged;
        }

        private void îòêğûòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.bmp|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
                ôèëüòğûToolStripMenuItem.Enabled = true;
            }
        }

        private void èíâåğñèÿToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }


        private void îòòåíêèÑåğîãîToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ñåïèÿToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            button1.Invoke(new Action(() => button1.Enabled = true));
            Bitmap newImage = ((Filters)e.Argument).processImage(image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
                this.newImage = newImage;
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = newImage;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void ğàçìûòèåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussuanFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ÿğêîñòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            brightnessPopup.ShowAtMenuItem((ToolStripMenuItem)sender);
        }

        private void brightnessPopup_BrightnessChanged(object sender, int value)
        {
            Filters filter = new BrightnessFilter(value);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ñäâèãToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shiftPopup.ShowAtMenuItem((ToolStripMenuItem)sender);
        }

        private void shiftPopup_ShiftChanged(object sender, int value)
        {
            Filters filter = new OffsetFilter(value);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void òèñíåíèåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters fitler = new EmbossFilter();
            backgroundWorker1.RunWorkerAsync(fitler);
        }

        private void ñåğûéÌèğToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new GrayWorldFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void ëèíåéíîåĞàñòÿæåíèåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new LinearStretchFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void èäåàëüíûéÎòğàæàòåëüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new PerfectReflectorFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void ğàñøèğåíèåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new DilationFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void ñóæåíèåToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new ErosionFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void ìåäèàíàToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void ñîáåëÿToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }

        private void ùàğğàToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filters = new SharrFilter();
            backgroundWorker1.RunWorkerAsync(filters);
        }
    }
}
