using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ScreenshotCapture
{
    public class ScreenshotCaptureController : MonoBehaviour
    {
        public ScreenshotCapturePivotController pivotController;
        public ScreenshotCaptureCameraController cameraController;
        public ScreenshotCaptureBrowser browser;

        public GameObject UIObject;

        [Space]
        [Header("Панель сохранения файла")]
        public Button buttonSave;
        public Button buttonSaveList;
        public InputField inputPath;
        public InputField inputFile;
        //public Toggle toggleTransparent;

        [Space]
        [Header("Панель настроек вида")]
        public Button buttonResetRotation;
        public Button buttonResetZoom;
        public Text labelZRotation;
        public Slider sliderZRotation;
        public Text labelFOV;
        public Slider sliderFOV;

        [Space]
        [Header("Панель браузера")]
        public Button buttonSelectNext;
        public Button buttonSelectPrev;
        public Dropdown dropdownList;

        [Space]
        [Header("Панель освещения")]
        public Light sideLight;
        public float minIntensity = 0;
        public float maxIntensity = 5;
        public Text labelSideLight;
        public Slider sliderSideLight;

        public string file = "test";
        public string path = @"D:\Projects\screenshots";

        private void Start()
        {
            sliderFOV.minValue = cameraController.minFOV;
            sliderFOV.maxValue = cameraController.maxFOV;
            sliderFOV.SetValueWithoutNotify(cameraController.mainCamera.fieldOfView);
            sliderFOV.onValueChanged.AddListener(OnFOVSliderChange);
            labelFOV.text = $"FOV: {Math.Round(cameraController.mainCamera.fieldOfView, 0)}";

            sliderZRotation.minValue = pivotController.minEuler.z;
            sliderZRotation.maxValue = pivotController.maxEuler.z;
            sliderZRotation.SetValueWithoutNotify(pivotController.Z);
            sliderZRotation.onValueChanged.AddListener(OnRotationSliderChange);
            labelZRotation.text = $"Z Rotation: {Math.Round(pivotController.Z, 1)}";

            sliderSideLight.minValue = minIntensity;
            sliderSideLight.maxValue = maxIntensity;
            sliderSideLight.SetValueWithoutNotify(sideLight.intensity);
            sliderSideLight.onValueChanged.AddListener(OnLightSliderChange);
            labelSideLight.text = $"Side light: {Math.Round(sideLight.intensity, 1)}";

            inputPath.text = path;
            inputPath.onValueChanged.AddListener(OnInputPathChanged);

            inputFile.text = browser.items[0].name;
            inputFile.onValueChanged.AddListener(OnInputFileChanged);

            buttonResetRotation.onClick.AddListener(OnResetRotationClicked);
            buttonResetZoom.onClick.AddListener(OnResetZoomClicked);
            buttonSave.onClick.AddListener(OnSaveClicked);
            buttonSaveList.onClick.AddListener(OnSaveListClicked);

            buttonSelectNext.onClick.AddListener(OnSelectNextClicked);
            buttonSelectPrev.onClick.AddListener(OnSelectPrevClicked);
            dropdownList.ClearOptions();
            dropdownList.AddOptions(browser.GetList());
            dropdownList.value = 0;
            dropdownList.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDestroy()
        {
            sliderZRotation.onValueChanged.RemoveAllListeners();
        }

        private IEnumerator CaptureRoutine()
        {
            UIObject.SetActive(false);

            yield return new WaitForEndOfFrame();
            TransparencyCapture.captureScreenshot(path + @"\" + file + ".png");

            yield return new WaitForEndOfFrame();
            UIObject.SetActive(true);
        }

        private IEnumerator CaptureAllRoutine()
        {
            buttonSave.interactable = false;
            buttonSaveList.interactable = false;
            buttonSelectNext.interactable = false;
            buttonSelectPrev.interactable = false;
            dropdownList.interactable = false;

            int count = browser.items.Count;
            for (int i = 0; i < count; i++)
            {
                yield return CaptureRoutine();
                OnSelectNextClicked();
            }

            buttonSave.interactable = true;
            buttonSaveList.interactable = true;
            buttonSelectNext.interactable = true;
            buttonSelectPrev.interactable = true;
            dropdownList.interactable = true;
        }

        private void OnDropdownChanged(int index)
        {
            var item = browser.Select(index);
            inputFile.text = item.name;
        }

        private void OnSelectNextClicked()
        {
            var item = browser.SelectNext();
            var index = browser.items.IndexOf(item);

            dropdownList.SetValueWithoutNotify(index);
            inputFile.text = item.name;
        }

        private void OnSelectPrevClicked()
        {
            var item = browser.SelectPrevious();
            var index = browser.items.IndexOf(item);

            dropdownList.SetValueWithoutNotify(index);
            inputFile.text = item.name;
        }

        private void OnFOVSliderChange(float newValue)
        {
            cameraController.SetFOV(newValue);
            labelFOV.text = $"FOV: {Math.Round(newValue, 0)}";
        }

        private void OnRotationSliderChange(float newValue)
        {
            pivotController.SetZ(newValue);
            labelZRotation.text = $"Z Rotation: {Math.Round(newValue, 1)}";
        }

        private void OnLightSliderChange(float newValue)
        {
            sideLight.intensity = newValue;
            labelSideLight.text = $"Side light: {Math.Round(sideLight.intensity, 1)}";
        }

        private void OnResetRotationClicked()
        {
            pivotController.ResetRotation();
            sliderZRotation.value = pivotController.Z;
        }

        private void OnResetZoomClicked()
        {
            cameraController.ResetZoom();
            sliderFOV.value = cameraController.mainCamera.fieldOfView;
        }

        private void OnSaveClicked()
        {
            StartCoroutine(CaptureRoutine());
        }

        private void OnSaveListClicked()
        {
            StartCoroutine(CaptureAllRoutine());
        }

        private void OnInputPathChanged(string text)
        {
            path = text;
        }

        private void OnInputFileChanged(string text)
        {
            file = text;
        }
    }
}
