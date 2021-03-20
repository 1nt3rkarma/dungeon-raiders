using System.Collections;
using UnityEngine;

using TextField = TMPro.TextMeshPro;

public class MainMenuDirector : MonoBehaviour
{
    public Camera mainCamera;

    public UIManagerMainMenu UIManager;

    public CameraPath pathInitial;
    public CameraPath pathToTheDungeon;

    public GatesAnimationHandler gatesAnimationHandler;

    public static bool firstLaunch = true;
    Coroutine cameraPatExecutionRoutine;
    Coroutine CameraPatExecutionRoutine
    {
        get => cameraPatExecutionRoutine;

        set
        {
            if (cameraPatExecutionRoutine != null)
                StopCoroutine(CameraPatExecutionRoutine);
            cameraPatExecutionRoutine = value;
        }
    }

    void Start()
    {
        if (firstLaunch)
        { 
            StartCoroutine(ScenarioInitialRoutine());
            firstLaunch = false;
        }
        else
        {
            SkipInitialScenario();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartCoroutine(ScenarioToTheDungeonRoutine());

    }

    public void SetInitialState()
    {
        UIManager.FadeLogo();
        UIManager.EnableSceneMask();
        mainCamera.transform.position = pathInitial.InitialPosition;
        gatesAnimationHandler.FadeDungeonLogo(0);
    }

    public void SkipInitialScenario()
    {
        UIManager.FadeLogo(1);
        UIManager.EnableSceneMask();
        gatesAnimationHandler.FadeDungeonLogo(1);
        mainCamera.transform.position = pathInitial.FinalPosition;
        mainCamera.transform.rotation = pathInitial.FinalRotation;
        mainCamera.fieldOfView = pathInitial.FinalFOV;

        UIManager.DisableSceneMask(1);
    }

    IEnumerator ScenarioInitialRoutine()
    {
        SetInitialState();

        yield return null;

        UIManager.FadeLogo(1,3);

        yield return new WaitForSeconds(3);

        CameraPatExecutionRoutine = StartCoroutine(ExecuteCameraPathRoutine(pathInitial));

        UIManager.DisableSceneMask(3);

        yield return new WaitForSeconds(3);

        gatesAnimationHandler.FadeDungeonLogo(1,0.5f);
    }

    public void PlayScenarioToTheDungeon()
    {
        StartCoroutine(ScenarioToTheDungeonRoutine());
    }

    IEnumerator ScenarioToTheDungeonRoutine()
    {
        UIManager.FadeLogo(0, 0.5f);

        gatesAnimationHandler.PlayToTheDungeonAnimation();

        CameraPatExecutionRoutine = StartCoroutine(ExecuteCameraPathRoutine(pathToTheDungeon));

        yield return new WaitForSeconds(pathToTheDungeon.totalDuration);
    }

    IEnumerator ExecuteCameraPathRoutine(CameraPath path)
    {
        //DoTweenTransformer cameraMover = new DoTweenTransformer(mainCamera.transform);
        //DoTweenCameraHandler cameraHandler = new DoTweenCameraHandler();
        //cameraHandler.targetCamera = mainCamera;

        //mainCamera.transform.position = path.InitialPosition;

        //for (int i = 1; i < path.nodes.Count; i++)
        //{
        //    if (path.nodes[i].transitFOV)
        //        cameraHandler.TransitFOV(path.nodes[i].FOV, path.nodes[i].duration);
        //    cameraMover.Move(path.nodes[i].transform.position, path.nodes[i].duration, path.nodes[i].curve);
        //    yield return new WaitForSeconds(path.nodes[i].duration);
        //}

        var timer = 0f;

        mainCamera.transform.position = path.InitialPosition;

        while (timer < path.totalDuration)
        {
            yield return null;
            timer += Time.deltaTime;

            var progress = path.progressCurve.Evaluate(timer / path.totalDuration);
            var activeSegment = path.GetSegment(progress);
            var stepPosition = path.GetNormalizedPosition(progress);
            var stepEotation = Quaternion.Euler(activeSegment.GetRotation(progress));

            var newFOV = activeSegment.GetFOV(progress);
            var newRotation = Quaternion.Lerp(mainCamera.transform.rotation, stepEotation, path.rotationSmoothing);
            var newPosition = Vector3.Lerp(mainCamera.transform.position, path.GetNormalizedPosition(progress), path.positionSmoothing);
            mainCamera.transform.position = newPosition;
            mainCamera.transform.rotation = newRotation;
            mainCamera.fieldOfView = newFOV;
        }

        mainCamera.transform.position = path.FinalPosition;
    }

}
