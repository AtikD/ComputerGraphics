namespace WinFormsApp1
{
    partial class ShiftForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            trackBar1 = new TrackBar();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(trackBar1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 100);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(60, 65);
            button1.Name = "button1";
            button1.Size = new Size(84, 23);
            button1.TabIndex = 2;
            button1.Text = "Применить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(94, 40);
            label1.Name = "label1";
            label1.Size = new Size(13, 15);
            label1.TabIndex = 1;
            label1.Text = "0";
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(10, 10);
            trackBar1.Maximum = 100;
            trackBar1.Minimum = -100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(180, 45);
            trackBar1.TabIndex = 0;
            trackBar1.TickFrequency = 20;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // ShiftForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(200, 100);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            MaximumSize = new Size(200, 100);
            MinimumSize = new Size(200, 100);
            Name = "ShiftForm";
            ShowInTaskbar = false;
            Text = "ShiftForm";
            Deactivate += ShiftForm_Deactivate;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private TrackBar trackBar1;
        private Button button1;
    }
}