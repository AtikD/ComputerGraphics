using OpenTK.Mathematics;

namespace RayTracing
{
    public partial class debug : Form
    {
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraDirection { get; set; }

        public debug(Vector3 curPos, Vector3 curDir)
        {
            InitializeComponent();
            textBox1.Text = curPos.X.ToString();
            textBox2.Text = curPos.Y.ToString();
            textBox3.Text = curPos.Z.ToString();

            textBox4.Text = curDir.X.ToString();
            textBox5.Text = curDir.Y.ToString();
            textBox6.Text = curDir.Z.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CameraPosition = new Vector3(float.Parse(textBox1.Text), float.Parse(textBox2.Text), float.Parse(textBox3.Text));
                CameraDirection = new Vector3(float.Parse(textBox4.Text), float.Parse(textBox5.Text), float.Parse(textBox6.Text));
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Неверный формат данных!");
            }
        }
    }
}
