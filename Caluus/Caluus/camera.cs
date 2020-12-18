using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Project1
{
    public class Camera
    {
        public Vector3 Position;
        public Vector3 cameraTarget;
        public Vector3 cameraDirection;
        public Vector3 Front = -Vector3.UnitZ;
        public Vector3 Up = Vector3.UnitY;
        public Vector3 Right = Vector3.UnitX;
        public float Yaw = 0;
        public float Pitch = 0;
        public float FOV = 45.0f;
        public float Aspect_Ratio = 1.6f;
        public float zNear = 0.1f;
        public float zFar = 100.0f;



        Matrix4 _view;

        // This represents how the vertices will be projected. It's hard to explain through comments,
        // so check out the web version for a good demonstration of what this does.
        Matrix4 _projection;

        public Camera(Vector3 ViewMatrix, float AspectRatio, float FOVdeg)
        {
            _view = Matrix4.CreateTranslation(ViewMatrix);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOVdeg),AspectRatio, 0.1f, 1000.0f);
            Aspect_Ratio = AspectRatio;
            FOV = FOVdeg;
        }

        public Matrix4 GetViewMatrix()
        {
            Front.X = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(Yaw));
            Front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(Pitch));
            Front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(Yaw));

            _view = Matrix4.CreateTranslation(Position);
            _view = Matrix4.LookAt(Position, Position + Front, Up);
            return _view;
        }
        public Matrix4 GetProjectionMatrix()
        {
            return _projection;
        }
        public void UpdateProjectionMatrix(float AspectRatio, float FOVdeg, float _zNear, float _zFar)
        {
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOVdeg), AspectRatio, _zNear, _zFar);
        }
        public void UpdateProjectionMatrixFOV(float FOVdeg)
        {
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOVdeg), Aspect_Ratio, zNear, zFar);
        }
        public void UpdateProjectionMatrixAspectRatio(float Aspectratio)
        {
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), Aspectratio, zNear, zFar);
        }

    }
}
