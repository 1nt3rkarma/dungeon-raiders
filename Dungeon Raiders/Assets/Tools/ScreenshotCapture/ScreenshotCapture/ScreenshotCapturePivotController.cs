using UnityEngine;

using Utils = ScreenshotCapture.ScreenshotCaptureUtils;

namespace ScreenshotCapture
{
    public class ScreenshotCapturePivotController : MonoBehaviour
    {
        public Vector3 defaultEuler;
        public Vector3 minEuler;
        public Vector3 maxEuler;
        public AxisRestriction restrictions;
        public float sensitivity = 0.5f;
        public MouseButtons controllButton = MouseButtons.Left;

        public float Z;

        public bool isCapturing;

        private void Awake()
        {
            Z = defaultEuler.z;
        }

        private void Update()

        {
            if (Input.GetMouseButtonDown((int)controllButton)
                && Utils.GetUIElement(Input.mousePosition) == null)
                isCapturing = true;

            if (isCapturing)
                UpdateRotation();

            if (Input.GetMouseButtonUp((int)controllButton))
                isCapturing = false;
        }

        private void UpdateRotation()
        {
            var scrollX = Input.GetAxis("Mouse X") * sensitivity * Mathf.Deg2Rad;
            var scrollY = Input.GetAxis("Mouse Y") * sensitivity * Mathf.Deg2Rad;

            transform.Rotate(Vector3.up, -scrollX);
            transform.Rotate(Vector3.right, scrollY);

            var euler = transform.eulerAngles;

            if (euler.x > 180)
                euler.x -= 360;
            if (!restrictions.x)
                euler.x = Mathf.Clamp(euler.x, minEuler.x, maxEuler.x);
            else
                euler.x = defaultEuler.x;

            if (euler.y > 180)
                euler.y -= 360;
            if (!restrictions.y)
                euler.y = Mathf.Clamp(euler.y, minEuler.y, maxEuler.y);
            else
                euler.y = defaultEuler.y;

            if (!restrictions.z)
                euler.z = Z;
            else
                euler.z = defaultEuler.z;

            transform.eulerAngles = euler;
        }

        public void SetZ(float z)
        {
            Z = Mathf.Clamp(z, minEuler.z, maxEuler.z);
            UpdateRotation();
        }

        public void ResetRotation()
        {
            SetZ(defaultEuler.z);
            transform.localEulerAngles = defaultEuler;
        }
    }

    [System.Serializable]
    public class AxisRestriction : object
    {
        public bool x;
        public bool y;
        public bool z;
    }

    public enum MouseButtons { Left, Right, Wheel }
}