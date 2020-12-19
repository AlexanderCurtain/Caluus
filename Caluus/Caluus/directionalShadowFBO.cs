using System;
using System.Reflection.Emit;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;

namespace Project1
{
    class DirectionShadowFBO
    {
        public int ID { get; private set; }
        public int TextureID { get; private set; }
        public FramebufferErrorCode FramebufferStatus { get; private set; }

        public float near_plane = 1.0f, far_plane = 20.0f;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public DirectionShadowFBO(int width, int height)
        {
            //gen depthbuffer as texture
            TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.Repeat);

            //gen frambuffer
            ID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            //attach depth
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureID, 0);

            //since we dont have a color buffer, we cant draw do it (we dont need to)
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            //check status and store it
            FramebufferStatus = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

            //unbind everything
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            WriteFBOstatus();

            Width = width;
            Height = height;
        }
        public void RenderShadowMap(Vector3 lightpos, Shader _Shader, List<Model> ObjectsRendering, int WindowWidth, int WindowHeight)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            

            foreach (Model OBJ in ObjectsRendering)
            {
                OBJ.RenderToShadowMap(_Shader);
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, WindowWidth, WindowHeight);
        }

        private void WriteFBOstatus()
        {
            
            switch (GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Console.WriteLine("FBO: The framebuffer is complete and valid for rendering.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Console.WriteLine("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Console.WriteLine("FBO: There are no attachments.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Console.WriteLine("FBO: Attachments are of different size. All attachments must have the same width and height.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Console.WriteLine("FBO: The color attachments have different format. All color attachments must have the same format.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Console.WriteLine("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Console.WriteLine("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Console.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("FBO: Status unknown. (yes, this is really bad.)");
                        break;
                    }
            }
        }


    }
}
