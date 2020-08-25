using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScreenshotCapture
{
    public static class ScreenshotCaptureUtils
    {
        public static GameObject GetUIElement(Vector2 atScreenPosition)
        {
            var pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = atScreenPosition;

            var hitObjects = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, hitObjects);

            if (hitObjects.Count > 0)
                return hitObjects[0].gameObject;
            else
                return null;
        }
    }
}
