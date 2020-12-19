using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;


namespace Project1
{
    class Model
    {
        private int VBO;
        private int VAO;

        private RawData ModelData;

        private Texture _diffuseMap;
        private Texture _specularMap;
        private Texture _normalMap;

        private float Shine;
        private float Opacity;

        private string FolderPath;

        private List<CubeTranslations> Instances = new List<CubeTranslations>();

        public Model(string filepath, Shader _Shader)
        {
            FolderPath = filepath;
            _diffuseMap = new Texture(FolderPath + @"\Textures\Diff.png");
            _normalMap = new Texture(FolderPath + @"\Textures\Norm.png");
            _specularMap = new Texture(FolderPath + @"\Textures\Spec.png");

            int i = 0;
            foreach (string line in System.IO.File.ReadLines(FolderPath + @"\Material.mat"))
            {
                switch (i)
                {
                    case 0:
                        Shine = float.Parse(line);
                        break;
                    case 1:
                        Opacity = float.Parse(line);
                        break;
                }
                i++;
            }

            ModelData = new RawData(FolderPath + @"\Model.obj");

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ModelData.GetVertices().Length * sizeof(float), ModelData.GetVertices(), BufferUsageHint.StaticDraw);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);

            // All of the vertex attributes have been updated to now have a stride of 8 float sizes.
            var positionLocation = _Shader.GetAttribLocation("aPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 0);

            var normalLocation = _Shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 3 * sizeof(float));

            // The texture coords have now been added too, remember we only have 2 coordinates as the texture is 2d,
            // so the size parameter should only be 2 for the texture coordinates.
            var texCoordLocation = _Shader.GetAttribLocation("aTexCoords");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 11 * sizeof(float), 6 * sizeof(float));

            //Tangent vector
            var TangentLocation = _Shader.GetAttribLocation("aTangent");
            GL.EnableVertexAttribArray(TangentLocation);
            GL.VertexAttribPointer(TangentLocation, 3, VertexAttribPointerType.Float, false, 11 * sizeof(float), 8 * sizeof(float));
        }

        public void RenderObject(Shader _Shader)
        {
            GL.BindVertexArray(VAO);
            _diffuseMap.Use(TextureUnit.Texture0);
            _specularMap.Use(TextureUnit.Texture1);
            _normalMap.Use(TextureUnit.Texture2);
            _Shader.Use();


            _Shader.SetInt("material.diffuse", 0);
            _Shader.SetInt("material.specular", 1);
            _Shader.SetInt("material.normalmap", 2);
            _Shader.SetFloat("material.shininess", Shine);
            _Shader.SetFloat("material.opacity", Opacity);

            foreach (CubeTranslations Cube in Instances)
            {
                Matrix4 model = Matrix4.Identity;

                model *= Matrix4.CreateScale(Cube.getSize());
                model *= Matrix4.CreateTranslation(Cube.getPos());
                

                //model *= Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), 90);
                _Shader.SetMatrix4("model", model);

                _Shader.SetMatrix4("inverse_model",Matrix4.Transpose((Matrix4.Invert(model))));

                GL.DrawArrays(PrimitiveType.Triangles, 0, ModelData.GetFaceCount() * 3);
            }
        }

        public void RenderToShadowMap(Shader _Shader)
        {
            GL.BindVertexArray(VAO);
            _Shader.Use();
            
            foreach (CubeTranslations Cube in Instances)
            {
                Matrix4 model = Matrix4.Identity;

                model *= Matrix4.CreateScale(Cube.getSize());
                model *= Matrix4.CreateTranslation(Cube.getPos());


                //model *= Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), 90);
                _Shader.SetMatrix4("model", model);

                GL.DrawArrays(PrimitiveType.Triangles, 0, ModelData.GetFaceCount() * 3);
            }
        }




        struct CubeTranslations
        {
            public CubeTranslations(Vector3 pos, Vector4 rot, Vector3 size)
            {
                position = pos;
                rotation = rot;
                scale = size;
            }
            public Vector3 getPos()
            {
                return position;
            }
            public Vector3 getSize()
            {
                return scale;
            }

            Vector3 position;
            Vector4 rotation;
            Vector3 scale;
        }
        public void AddInstance(Vector3 Pos, Vector4 Rot, Vector3 Size)
        {
            Instances.Add(new CubeTranslations(Pos, Rot, Size));
        }
        public void AddInstance(Vector3 Pos, Vector3 Size)
        {
            Instances.Add(new CubeTranslations(Pos, new Vector4(1.0f, 1.0f, 1.0f, 0.0f), Size));
        }
        public void AddInstance(Vector3 Pos)
        {
            Instances.Add(new CubeTranslations(Pos, new Vector4(1.0f, 1.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f)));
        }
    }
}
