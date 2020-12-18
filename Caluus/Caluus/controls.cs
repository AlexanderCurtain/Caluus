using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Project1
{
    public class controlmethod 
    {
        private Vector2 _lastPos;
        private bool _firstMove = true;

        public Camera Spector(Camera _camera, double DeltaTime, float cameraSpeed, float sensitivity)
        {
            var input = Keyboard.GetState();


            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)DeltaTime; // Forward
            }

            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)DeltaTime; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * cameraSpeed * (float)DeltaTime; //Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * cameraSpeed * (float)DeltaTime; //Right
            }
            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)DeltaTime; // Up
            }
            if (input.IsKeyDown(Key.LShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)DeltaTime; // Down
            }

            return _camera;
        }     
        
        public Camera Mousecontrol(Camera _camera, float sensitivity)
        {

            var mouse = Mouse.GetState();

            if (_firstMove) // this bool variable is initially set to true
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // reversed since y-coordinates range from bottom to top
                if (_camera.Pitch > 89.0f)
                {
                    _camera.Pitch = 89.0f;
                }
                else if (_camera.Pitch < -89.0f)
                {
                    _camera.Pitch = -89.0f;
                }
            }
            return _camera;
        }
    

    }
}
