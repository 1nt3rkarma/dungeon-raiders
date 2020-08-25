using UnityEngine;

using Utils = ScreenshotCapture.ScreenshotCaptureUtils;

namespace ScreenshotCapture
{
    public class ScreenshotCaptureCameraController : MonoBehaviour
    {
        public Camera mainCamera;

        public Vector3 maxPosition = new Vector3(2, 2, 2);
        public Vector3 minPosition = new Vector3(-2, -2, -6);
        public float defaultFOV = 60;
        public float minFOV = 30;
        public float maxFOV = 120;

        public KeyCode navigationUp = KeyCode.W;
        public KeyCode navigationDown = KeyCode.S;
        public KeyCode navigationRight = KeyCode.D;
        public KeyCode navigationLeft = KeyCode.A;
        public KeyCode navigationReset = KeyCode.R;

        public float sensitivity = 0.5f;
        public float speed = 0.5f;

        private void Awake()
        {
            mainCamera.fieldOfView = defaultFOV;
        }

        void Update()
        {
            var scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
                if (Utils.GetUIElement(Input.mousePosition) == null)
                    UpdateZoom(Input.mouseScrollDelta.y);

            UpdatePosition();

            if (Input.GetKeyDown(navigationReset))
                ResetPosition();
        }

        void UpdatePosition()
        {
            var direction = new Vector3();

            if (Input.GetKey(navigationUp) || Input.GetKeyDown(navigationUp))
                direction += Vector3.up;
            if (Input.GetKey(navigationDown) || Input.GetKeyDown(navigationDown))
                direction -= Vector3.up;
            if (Input.GetKey(navigationRight) || Input.GetKeyDown(navigationRight))
                direction += Vector3.right;
            if (Input.GetKey(navigationLeft) || Input.GetKeyDown(navigationLeft))
                direction -= Vector3.right;

            var position = mainCamera.transform.localPosition + direction * speed;

            position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
            position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);

            mainCamera.transform.localPosition = position;
        }

        void UpdateZoom(float scroll)
        {
            var pos = mainCamera.transform.localPosition;
            pos.z = Mathf.Clamp(mainCamera.transform.localPosition.z + scroll * sensitivity,
                                minPosition.z, maxPosition.z);
            mainCamera.transform.localPosition = pos;
        }

        public void ResetZoom()
        {
            var position = mainCamera.transform.localPosition;
            position.z = 0;
            mainCamera.transform.localPosition = position;

            mainCamera.fieldOfView = defaultFOV;
        }

        public void ResetPosition()
        {
            var position = mainCamera.transform.localPosition;
            position.x = 0;
            position.y = 0;
            mainCamera.transform.localPosition = position;
        }

        public void SetFOV(float value)
        {
            mainCamera.fieldOfView = Mathf.Clamp(value, minFOV, maxFOV);
        }

        void OnDrawGizmos()
        {
            if (!mainCamera)
                return;

            Gizmos.color = Color.blue;
            var zoomMaxPoint = transform.position;
            zoomMaxPoint.z += maxPosition.z;
            var zoomMinPoint = transform.position;
            zoomMinPoint.z += minPosition.z;
            var size = new Vector3(0.2f, 0.2f, 0.2f);
            Gizmos.DrawLine(zoomMinPoint, zoomMaxPoint);
            Gizmos.DrawWireSphere(zoomMaxPoint, 0.1f);
            Gizmos.DrawWireSphere(zoomMinPoint, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(mainCamera.transform.position, size);
        }
    }
}
