using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace RayTracing
{
    internal class View
    {
        int BasicProgramID;
        int BasicVertexShader;
        int BasicFragmentShader;
        int vbo_position;
        int vao;
        Vector3[] vertdata;
        int attribute_vpos;
        int uniform_aspect;
        int uniform_up;
        int uniform_right;
        int uniform_dir;
        int uniform_pos;

        float aspect = 1.0f;
        Vector3 campos = new Vector3(0, 0, -8);
        Vector3 camdir = new Vector3(0, 0, 1);

        Vector3 camup = new Vector3(0, 1, 0);
        Vector3 camright = new Vector3(1, 0, 0);

        public Vector3 CamPos
        {
            get => campos;
            set => campos = value;
        }        
        public Vector3 CamDir
        {
            get => camdir;
            set => camdir = value;
        }        


        public void loadShader(string filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);

            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);

            string infoLog = GL.GetShaderInfoLog(address);
            int status = 0;
            GL.GetShader(address, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                MessageBox.Show($"Shader compile error: {filename}\n\n{infoLog}");
                Application.Exit();
            }
            Console.WriteLine(infoLog);
            
        }

        public void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();
            loadShader("..\\..\\..\\raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("..\\..\\..\\raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);

            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));

            attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vPosition");
            uniform_pos = GL.GetUniformLocation(BasicProgramID, "campos");
            uniform_dir = GL.GetUniformLocation(BasicProgramID, "camdir");
            uniform_up = GL.GetUniformLocation(BasicProgramID, "camup");
            uniform_right = GL.GetUniformLocation(BasicProgramID, "camright");
            uniform_aspect = GL.GetUniformLocation(BasicProgramID, "aspect");
        }

        public void InitBuffers()
        {
            vertdata = new Vector3[] {
                new Vector3(-1f, -1f, 0f),
                new Vector3( 1f, -1f, 0f),
                new Vector3( 1f,  1f, 0f),
                new Vector3(-1f,  1f, 0f)
            };

            GL.GenVertexArrays(1, out vao);
            GL.BindVertexArray(vao);

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.Uniform3(uniform_pos, campos);
            GL.Uniform3(uniform_dir, camdir);
            GL.Uniform3(uniform_up, camup);
            GL.Uniform3(uniform_right, camright);
            GL.Uniform1(uniform_aspect, aspect);

            GL.UseProgram(BasicProgramID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(BasicProgramID);
            GL.Uniform3(uniform_pos, campos);
            GL.Uniform3(uniform_dir, camdir);
            GL.Uniform3(uniform_up, camup);
            GL.Uniform3(uniform_right, camright);
            GL.Uniform1(uniform_aspect, aspect);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        }

        public void UpdateAspect(int width, int height)
        {
            aspect = (float)width / height;
            GL.Uniform1(uniform_aspect, aspect);
        }
    }
}
