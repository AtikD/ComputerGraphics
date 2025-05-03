using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace RayTracing
{
    public partial class Form1 : Form
    {
        int FrameCount;
        DateTime NextFPSUpdate;
        View view;

        public Form1()
        {
            InitializeComponent();
            NextFPSUpdate = DateTime.Now.AddSeconds(1);
        }

        void displayFPS()
        {
            if (DateTime.Now > NextFPSUpdate)
            {
                this.Text = "RayTraycing Example (fps: " + FrameCount + ")";
                FrameCount = 0;
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
            }
            FrameCount++;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            displayFPS();
            glControl1.Invalidate();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Idle += Application_Idle;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.DepthTest);
            view = new View();
            view.InitShaders();
            view.InitBuffers();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            view.Draw();
            glControl1.SwapBuffers();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            GL.Viewport(glControl1.ClientRectangle);
            if (view != null)
                view.UpdateAspect(glControl1.Width, glControl1.Height);
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            Vector3 newPos = view.CamPos;
            Vector3 newDir = view.CamDir;
            switch (e.KeyCode)
            {
                case Keys.Q:
                    using (debug debugForm = new debug(view.CamPos, view.CamDir))
                    {
                        if (debugForm.ShowDialog() == DialogResult.OK)
                        {
                            newPos = debugForm.CameraPosition;
                            newDir = debugForm.CameraDirection;
                        }
                    }
                    break;
            }
            view.CamPos = newPos;
            view.CamDir = newDir;
        }
    }
}
