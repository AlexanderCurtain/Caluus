using System;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace Project1
{
    public class Shader
    {
        public int Handle;
        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string vertexPath, string fragmentPath)
        {
            string VertexShaderSource;

            int VertexShader;

            int FragmentShader;

            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);


            GL.CompileShader(VertexShader);
            string infoLogVert = GL.GetShaderInfoLog(VertexShader);

            Console.WriteLine(infoLogVert);
            

            Console.WriteLine(infoLogVert);

            GL.CompileShader(FragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);

                System.Console.WriteLine(infoLogFrag);
            

            System.Console.WriteLine(infoLogVert);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);


            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Next, allocate the dictionary to hold the locations.
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
        }


        public void Use()
        {
            GL.UseProgram(Handle);
        }

        private bool disposedValue = false;

        public void SetInt(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform1(location, value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }
         public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }
        public void SetMatrix3(string name, Matrix3 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix3(_uniformLocations[name], true, ref data);
        }
    }
        
}
