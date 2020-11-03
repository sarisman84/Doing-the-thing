using Cinemachine;
using UnityEngine;

namespace Player
{
    public class FPCameraHandler
    {
        public FPCameraHandler()
        {
           AlterCursorState(false);
        }

        public void AlterCursorState(bool value)
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private float MouseRotation(float lookInput, float mouseSensitivity)
        {
            return lookInput * mouseSensitivity * Time.deltaTime;
        }


        private float xRotation;

        public void RotateCameraVertically(Transform camera, Vector2 lookInput, float mouseSensitivity = 100f)
        {
            float mouseY = MouseRotation(lookInput.y, mouseSensitivity);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);


            camera.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        public void RotatePlayerHorizontally(Transform player, Vector2 lookInput, float mouseSensitivity = 100f)
        {
            float mouseX = MouseRotation(lookInput.x, mouseSensitivity);
            player.Rotate(Vector3.up * mouseX);
        }
    }
}