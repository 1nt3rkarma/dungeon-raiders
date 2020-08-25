using System.Collections.Generic;
using UnityEngine;

namespace ScreenshotCapture
{
    public class ScreenshotCaptureBrowser : MonoBehaviour
    {
        public Transform groupTransform;
        public List<Transform> items;
        private int lastIndex = -1;

        private void Awake()
        {
            items = new List<Transform>();
            foreach (Transform item in groupTransform)
            {
                items.Add(item);
                item.gameObject.SetActive(false);
            }

            Select(0);
        }

        public Transform SelectNext()
        {
            int index;
            if (lastIndex == items.Count - 1)
                index = 0;
            else
                index = lastIndex + 1;
            return Select(index);
        }

        public Transform SelectPrevious()
        {
            int index;
            if (lastIndex == 0)
                index = items.Count - 1;
            else
                index = lastIndex - 1;
            return Select(index);
        }

        public Transform Select(int index)
        {
            if (lastIndex != -1)
                items[lastIndex].gameObject.SetActive(false);

            items[index].gameObject.SetActive(true);
            lastIndex = index;
            return items[index];
        }

        public List<string> GetList()
        {
            var list = new List<string>();
            foreach (var item in items)
                list.Add(item.name);
            return list;
        }
    }
}
