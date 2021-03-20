using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAnimator : MonoBehaviour
{
    public Transform modelTransform;
    public MeshRenderer glowRenderer;

    [Header("Idle animation")]
    public bool isFloating;
    public float floatMaxY = 0.75f;
    public float floatMinY = 0.5f;
    public float floatDuration = 1.5f;

    public bool isRotating;
    public float rotationDuration = 3f;

    public bool looping = false;

    [Header("PickUp animation")]
    public float jumpMaxY = 1.7f;
    public float jumpMinY = 0.5f;
    [Range(0,1)][Tooltip("How much of the total duration object is jumping up?")]
    public float jumpProportion = 0.4f;
    public float jumpDuration = 1f;
    public float jumpScale = 0.5f;
    public float followSpeed;
    public AnimationCurve followSpeedCurve;

    private DoTweenTransformer transformRotator;
    private DoTweenTransformer transformMover;
    private DoTweenTransformer transformScaler;
    private DoTweenMaterialColorizer glowFader;
    private Coroutine followUnitRoutine;

    void Awake()
    {
        transformRotator = new DoTweenTransformer(modelTransform);
        transformMover = new DoTweenTransformer(modelTransform);
        transformScaler = new DoTweenTransformer(modelTransform);

        if (glowRenderer)
            glowFader = new DoTweenMaterialColorizer(glowRenderer.material);
    }

    void OnDestroy()
    {
        transformRotator?.Stop();
        transformMover?.Stop();
        transformScaler?.Stop();
        glowFader?.Stop();
    }

    void OnEnable()
    {
        RunIdleAnimation();
    }

    public void RunIdleAnimation()
    {
        transformRotator?.Stop();
        transformMover?.Stop();
        transformScaler?.Stop();

        if (isFloating)
            transformMover?.FloatLocalPosition(floatMinY, floatMaxY, floatDuration);

        if (isRotating)
            transformRotator?.RotateCircle(true, rotationDuration);

        if (looping)
        {
            transformRotator?.EnableLoop();
            transformMover?.EnableLoop();
        }
    }

    public void RunPickUpAnimation(Transform unitTransform)
    {
        glowFader?.Transit(1, 0, jumpDuration / 2);

        if (transformMover != null && transformScaler != null)
        {
            var highestPoint = modelTransform.localPosition;
            highestPoint.y = jumpMaxY;
            var lowestPoint = modelTransform.localPosition;
            lowestPoint.y = jumpMinY;

            SequenceNode<Vector3>[] positions = new SequenceNode<Vector3>[]
            {
            new SequenceNode<Vector3>(){value = highestPoint, duration = jumpDuration * jumpProportion, ease = DG.Tweening.Ease.InOutSine },
            new SequenceNode<Vector3>(){value = lowestPoint, duration = jumpDuration * (1 - jumpProportion), ease = DG.Tweening.Ease.InOutSine }
            };
            transformMover.MoveLocal(positions);

            SequenceNode<float>[] scales = new SequenceNode<float>[]
            {
            new SequenceNode<float>(){value = jumpScale, duration = jumpDuration * jumpProportion, ease = DG.Tweening.Ease.InOutFlash},
            new SequenceNode<float>(){value = 0.05f, duration = jumpDuration * (1 - jumpProportion), ease = DG.Tweening.Ease.InOutFlash}
            };
            transformScaler.Scale(scales);
        }

        if (followUnitRoutine != null)
            StopCoroutine(followUnitRoutine);
        followUnitRoutine = StartCoroutine(FollowUnitRoutine(unitTransform));
    }
    
    IEnumerator FollowUnitRoutine(Transform target)
    {
        var timer = 0f;

        while(timer < jumpDuration)
        {
            //var direction = target.position - transform.position;
            //direction.Normalize();

            //transform.position += direction * followSpeed * Time.deltaTime; //* followSpeedCurve.Evaluate(timer) 
            transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime); //* followSpeedCurve.Evaluate(timer) 
            yield return null;
        }
    }
}
