using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;


namespace Project1
{
    public class window : GameWindow
    {

        private Shader _lightingShader;
        private Shader _ShadowmappingShader;
        private Shader _DebugMap;

        private DirectionShadowFBO DSFBO;

        private Camera _camera;

        private controlmethod _Control = new controlmethod();

        readonly string shaderfolderpath = @"C:\Users\CoolerMaster\Documents\TheCallusGameEngine\Callus\Caluus\Caluus\Shaders\";

        readonly string modelfolderpath = @"C:\Users\CoolerMaster\Documents\TheCallusGameEngine\Callus\Caluus\Caluus\Models\";
        /*/
        private Model Overpass;
        private Model ConcretePlate;
        private Model Barrier;
        /*/

        private Model Plane;
        private Model Cube;

        List<Model> RenderList = new List<Model>();

        public window(int width, int height, string title)
            : base(width, height, new GraphicsMode(new ColorFormat(8, 8, 8, 0),
      24, // Depth bits
      8,  // Stencil bits
      4   // FSAA samples
    ))
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.0f, 0.4f, 0.7f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            
            
            _lightingShader = new Shader(shaderfolderpath + "texture.vert", shaderfolderpath + "texture.frag");
            _ShadowmappingShader = new Shader(shaderfolderpath + "depthFBO.vert", shaderfolderpath + "depthFBO.frag");
            _DebugMap = new Shader(shaderfolderpath + "debug.vert", shaderfolderpath + "debug.frag");


            Plane = new Model(modelfolderpath + "FlatPlane", _lightingShader);
            Cube = new Model(modelfolderpath + "SimpleCube", _lightingShader);

            DSFBO = new DirectionShadowFBO(1024, 1024);

            var proj = Matrix4.CreateOrthographicOffCenter(-10, 10, -10, 10, 1.0f, 100.5f);
            var light = Matrix4.LookAt(new Vector3(.1f, 10, .1f), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            Matrix4 lightSpace = Matrix4.Mult(proj, light);
            _ShadowmappingShader.Use();
            _ShadowmappingShader.SetMatrix4("Projection", proj);
            _ShadowmappingShader.SetMatrix4("View", light);

            _camera = new Camera(Vector3.UnitZ * 3, 1.6f, 45.0f);

            Plane.AddInstance(new Vector3(0.0f, -0.2f, 0.0f));

            Cube.AddInstance(new Vector3(0.0f, 1.5f, 0.0f), new Vector3(0.5f));
            Cube.AddInstance(new Vector3(2.0f, 0.0f, 1.0f), new Vector3(0.5f));
            Cube.AddInstance(new Vector3(-1.0f, 0.0f, 2.0f), new Vector3(0.25f));
            Cube.AddInstance(new Vector3(-2.0f, 1.0f, -1.0f), new Vector3(0.05f));

            Cube.AddInstance(new Vector3(-2.0f, 1.0f, -1.0f) + new Vector3(0.1f, 10f, 5.1f), new Vector3(1f));
            Cube.AddInstance(new Vector3(-2.0f, 1.0f, -1.0f) + new Vector3(0, 1f, 0), new Vector3(0.1f));

            RenderList.Add(Plane);
            RenderList.Add(Cube);

            CursorVisible = false;
            /*/
             * Overpass = new Model(@"C:\Users\CoolerMaster\source\repos\Project1\Project1\Models\Passoverhead", _lightingShader);
            ConcretePlate = new Model(@"C:\Users\CoolerMaster\source\repos\Project1\Project1\Models\ConcretePlate", _lightingShader);
            Barrier = new Model(@"C:\Users\CoolerMaster\source\repos\Project1\Project1\Models\Barrier", _lightingShader);

            foreach (Vector3 a in _cubePositions)
            {
                Overpass.AddInstance(a);
                ConcretePlate.AddInstance(a + new Vector3(0.0f + 2f, 0.0f, 2.0f));
            }
            Barrier.AddInstance(new Vector3(0f, 0, 4f));
            Barrier.AddInstance(new Vector3(3f, 0, 4f));
            Barrier.AddInstance(new Vector3(6f, 0, 4f));
            Barrier.AddInstance(new Vector3(9f, 0, 4f));
            Barrier.AddInstance(new Vector3(12f, 0, 4f));
            Barrier.AddInstance(new Vector3(15f, 0, 4f));

            RenderList.Add(Overpass);
            RenderList.Add(ConcretePlate);
            RenderList.Add(Barrier);
            /*/
            base.OnLoad(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            
            GL.Viewport(0, 0, Width, Height);

            DSFBO.RenderShadowMap(new Vector3(-2.0f, 1.0f, -1.0f), _ShadowmappingShader, RenderList, Width, Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, DSFBO.TextureID);
            _DebugMap.Use();
            _DebugMap.SetInt("depthMap", 0);
            _DebugMap.SetFloat("near_plane", 0.1f);
            _DebugMap.SetFloat("far_plane", 1.0f);
            //renderQuad();

            

            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("viewPos", _camera.Position);

            //directional light
            _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _lightingShader.SetVector3("dirLight.diffuse", new Vector3(1.4f, 1.4f, 1.4f));
            _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));
            //pointLights
            _lightingShader.SetInt("NumberOfPointLights", 0);
            _lightingShader.SetInt("NumberOfSpotLights", 0);

            for (int i = 0; i < _pointLightPositions.Length; i++)
            {

                    _lightingShader.SetVector3($"pointLights[{i}].position", _pointLightPositions[i]);
                    _lightingShader.SetVector3($"pointLights[{i}].ambient", new Vector3(0.05f, 0.05f, 0.05f));
                    _lightingShader.SetVector3($"pointLights[{i}].diffuse", new Vector3(0.8f, 0.8f, 0.8f));
                    _lightingShader.SetVector3($"pointLights[{i}].specular", new Vector3(1.0f, 1.0f, 1.0f));
                    _lightingShader.SetFloat($"pointLights[{i}].constant", 1.0f);
                    _lightingShader.SetFloat($"pointLights[{i}].linear", 0.09f);
                    _lightingShader.SetFloat($"pointLights[{i}].quadratic", 0.032f);
                
                
            }
            // Spot light
            _lightingShader.SetVector3($"spotLight[{0}].position", _camera.Position);
            _lightingShader.SetVector3($"spotLight[{0}].direction", _camera.Front);
            _lightingShader.SetVector3($"spotLight[{0}].ambient", new Vector3(0.0f, 0.0f, 0.0f));
            _lightingShader.SetVector3($"spotLight[{0}].diffuse", new Vector3(1.0f, 1.0f, 1.0f));
            _lightingShader.SetVector3($"spotLight[{0}].specular", new Vector3(1.0f, 1.0f, 1.0f));
            _lightingShader.SetFloat($"spotLight[{0}].constant", 1.0f);
            _lightingShader.SetFloat($"spotLight[{0}].linear", 0.09f);
            _lightingShader.SetFloat($"spotLight[{0}].quadratic", 0.032f);
            _lightingShader.SetFloat($"spotLight[{0}].cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(12.5f)));
            _lightingShader.SetFloat($"spotLight[{0}].outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(20.0f)));


            foreach (Model render in RenderList)
            {
                render.RenderObject(_lightingShader);
            }
            /*/
            ConcretePlate.RenderObject(_lightingShader);
            Overpass.RenderObject(_lightingShader);
            Barrier.RenderObject(_lightingShader);
            /*/


            SwapBuffers();
            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (!Focused) return;
            if (input.IsKeyDown(Key.Escape)) Exit();

            _camera = _Control.Spector(_camera, e.Time, 5.0f, 1.0f);
            _camera = _Control.Mousecontrol(_camera, 0.1f);
            base.OnUpdateFrame(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused  
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.UpdateProjectionMatrixAspectRatio(Width / (float)Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            base.OnUnload(e);
        }

        private readonly Vector3[] _cubePositions =
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(4.7f, 0.0f, 0.0f),
            new Vector3(9.4f, 0.0f, 0.0f),


        };
        private readonly Vector3[] _pointLightPositions =
        {

            new Vector3(2.0f, 2.0f, 0.0f),

            new Vector3(6.0f, 2.0f, 0.0f),

            new Vector3(10.0f, 2.0f, 0.0f),

            new Vector3(14.0f, 2.0f, 0.0f),

        };

        uint quadVAO = 0;
        uint quadVBO;
        void renderQuad()
        {
            if (quadVAO == 0)
            {
                float[] quadVertices = {
            // positions        // texture Coords
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
             1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
             1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        };
                // setup plane VAO
                GL.GenVertexArrays(1, out quadVAO);
                GL.GenBuffers(1, out quadVBO);
                GL.BindVertexArray(quadVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            }
            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }
    }
}
