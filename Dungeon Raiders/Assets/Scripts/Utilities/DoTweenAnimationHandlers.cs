using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DoTweenAnimationHandler
{
    protected Tween animation;
    protected Sequence sequence;

    public bool Running => animation != null || sequence != null;

    /// <summary>
    /// Останавливает анимацию и убивает tweener
    /// </summary>
    /// <param name="complete">Нужно ли завершить анимацию?</param>
    public void Stop(bool complete = false)
    {
        animation?.Kill(complete);

        sequence?.Kill(complete);
    }

    public void SetEase(Ease ease)
    {
        animation?.SetEase(ease);
    }

    public void EnableLoop()
    {
        if (sequence == null)
            animation?.SetLoops(-1);
        else
            sequence?.SetLoops(-1);
    }

    public void DisableLoop()
    {
        animation?.SetLoops(1);

        sequence?.SetLoops(1);
    }
}

public class DoTweenMaterialColorizer : DoTweenAnimationHandler
{
    public Material targetMaterial;
    public float defaultAlpha => defaultColor.a;
    public Color defaultColor;

    public DoTweenMaterialColorizer(Material targetMaterial)
    {
        this.targetMaterial = targetMaterial;
        defaultColor = targetMaterial.color;
    }
    private Tween GetTweener(float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        Color targetColor = targetMaterial.color;
        targetColor.a = toAlpha;

        var tweener = DOTween.To(
            () => targetMaterial.color,
            x => targetMaterial.color = x,
            targetColor,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }
    private Tween GetTweener(Color toColor, float duration, Ease ease = Ease.Linear, bool affectAlpha = false)
    {
        Color targetColor = toColor;
        if (!affectAlpha)
            targetColor.a = targetMaterial.color.a;

        var tweener = DOTween.To(
            () => targetMaterial.color,
            x => targetMaterial.color = x,
            targetColor,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void SetAlpha(float a)
    {
        Color color = targetMaterial.color;
        color.a = a;
        targetMaterial.color = color;
    }
    public void SetColor(Color color, bool affectAlpha = false)
    {
        if (affectAlpha)
            targetMaterial.color = color;
        else
            targetMaterial.color = new Color(color.r, color.g, color.b, targetMaterial.color.a);
    }

    public void Transit(float fromAlpha, float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        Stop();

        SetAlpha(fromAlpha);

        animation = GetTweener(toAlpha, duration, ease);
    }
    public void Transit(Color fromColor, Color toColor, float duration, Ease ease = Ease.Linear, bool affectAlpha = false)
    {
        Stop();

        SetColor(fromColor, affectAlpha);

        animation = GetTweener(toColor, duration, ease, affectAlpha);
    }

    public void Blink(float fromAlpha, float toAlpha, float durationIn, float durationOut)
    {
        Stop();

        SetAlpha(fromAlpha);

        sequence = DOTween.Sequence();
        sequence.Append(GetTweener(toAlpha, durationIn, Ease.Flash));
        sequence.Append(GetTweener(fromAlpha, durationOut, Ease.Flash));
    }
    public void Blink(Color fromColor, Color toColor, float durationIn, float durationOut, Ease ease = Ease.Flash, bool affectAlpha = false)
    {
        Stop();

        SetColor(fromColor, affectAlpha);

        sequence = DOTween.Sequence();
        sequence.Append(GetTweener(toColor, durationIn, ease, affectAlpha));
        sequence.Append(GetTweener(fromColor, durationOut, ease, affectAlpha));
    }
}

public class DoTweenGraphicColorizer : DoTweenAnimationHandler
{
    private Graphic targetGraphic;
    public Graphic TargetGraphic
    {
        get => targetGraphic;

        set
        {
            targetGraphic = value;
            defaultColor = targetGraphic.color;
        }
    }

    public float defaultAlpha => defaultColor.a;
    public Color defaultColor;

    public DoTweenGraphicColorizer(Graphic targetGraphic)
    {
        TargetGraphic = targetGraphic;
    }
    private Tween GetTweener(float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        Color targetColor = targetGraphic.color;
        targetColor.a = toAlpha;

        var tweener = DOTween.To(
            () => targetGraphic.color,
            x => targetGraphic.color = x,
            targetColor,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }
    private Tween GetTweener(Color toColor, float duration, Ease ease = Ease.Linear, bool affectAlpha = false)
    {
        Color targetColor = toColor;
        if (!affectAlpha)
            targetColor.a = targetGraphic.color.a;

        var tweener = DOTween.To(
            () => targetGraphic.color,
            x => targetGraphic.color = x,
            targetColor,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void SetAlpha(float a)
    {
        Color color = targetGraphic.color;
        color.a = a;
        targetGraphic.color = color;
    }
    public void SetColor(Color color, bool affectAlpha = false)
    {
        if (affectAlpha)
            targetGraphic.color = color;
        else
            targetGraphic.color = new Color(color.r, color.g, color.b, targetGraphic.color.a);
    }

    public void Transit (float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        Stop();

        animation = GetTweener(toAlpha, duration, ease);
    }
    public void Transit(float fromAlpha, float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        SetAlpha(fromAlpha);

        Transit(toAlpha, duration, ease);
    }
    public void Transit(Color fromColor, Color toColor, float duration, Ease ease = Ease.Linear, bool affectAlpha = false)
    {
        Stop();

        SetColor(fromColor, affectAlpha);

        animation = GetTweener(toColor, duration, ease, affectAlpha);
    }

    public void Blink(float fromAlpha, float toAlpha, float durationIn, float durationOut)
    {
        Stop();

        SetAlpha(fromAlpha);

        sequence = DOTween.Sequence();
        sequence.Append(GetTweener(toAlpha, durationIn, Ease.Flash));
        sequence.Append(GetTweener(fromAlpha, durationOut, Ease.Flash));
    }
    public void Blink(Color fromColor, Color toColor, float durationIn, float durationOut, Ease ease = Ease.Flash, bool affectAlpha = false)
    {
        Stop();

        SetColor(fromColor, affectAlpha);

        sequence = DOTween.Sequence();
        sequence.Append(GetTweener(toColor, durationIn, ease, affectAlpha));
        sequence.Append(GetTweener(fromColor, durationOut, ease, affectAlpha));
    }
}

public class DoTweenCanvasFader : DoTweenAnimationHandler
{
    public CanvasGroup targetCanvasGroup;
    public float defaultAlpha;

    public DoTweenCanvasFader(CanvasGroup targetCanvasGroup)
    {
        this.targetCanvasGroup = targetCanvasGroup;
        defaultAlpha = targetCanvasGroup.alpha;
    }
    private Tween GetTweener(float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        var tweener = DOTween.To(
            () => targetCanvasGroup.alpha,
            x => targetCanvasGroup.alpha = x,
            toAlpha,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void SetAlpha(float a)
    {
        targetCanvasGroup.alpha = a;
    }

    public void Transit(float fromAlpha, float toAlpha, float duration, Ease ease = Ease.Linear)
    {
        Stop();

        SetAlpha(fromAlpha);

        animation = GetTweener(toAlpha, duration, ease);
    }

    public void Blink(float fromAlpha, float toAlpha, float durationIn, float durationOut)
    {
        Stop();

        SetAlpha(fromAlpha);

        sequence = DOTween.Sequence();
        sequence.Append(GetTweener(toAlpha, durationIn, Ease.Flash));
        sequence.Append(GetTweener(fromAlpha, durationOut, Ease.Flash));
    }
}

public class DoTweenImageFiller : DoTweenAnimationHandler
{
    public Image targetImage;

    public DoTweenImageFiller(Image targetImage)
    {
        this.targetImage = targetImage;
    }
    private Tween GetTweener(float toAmount, float duration, Ease ease = Ease.Linear)
    {
        var tweener = DOTween.To(
            () => targetImage.fillAmount,
            x => targetImage.fillAmount = x,
            toAmount,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void Fill(float fromAmount, float toAmount, float duration)
    {
        Stop();

        targetImage.fillAmount = fromAmount;

        animation = GetTweener(toAmount, duration);
    }
}

public class DoTweenTransformer : DoTweenAnimationHandler
{
    public Transform targetTransform;
    public Vector3 originalScale;

    public DoTweenTransformer(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
        originalScale = targetTransform.localScale;
        //originalScale = targetTransform.localScale;
        //originalScale = targetTransform.localScale;
    }

    private Tween GetScaleTweener(Vector3 toSize, float duration, Ease ease = Ease.Linear)
    {
        var tweener = DOTween.To(
            () => targetTransform.localScale,
            x => targetTransform.localScale = x,
            toSize,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }
    private Tween GetScaleTweener(float toScale, float duration, Ease ease = Ease.Linear)
    {
        var toSize = new Vector3(toScale, toScale, toScale);
        return GetScaleTweener(toSize, duration, ease);
    }
    private Tween GetEulerTweener(float degrees, bool clockwise, float duration, Ease ease = Ease.Linear)
    {
        var targetEuler = targetTransform.localEulerAngles;
        if (clockwise)
            targetEuler.y += degrees;
        else
            targetEuler.y -= degrees;
        var targetQuaternion = Quaternion.Euler(targetEuler);

        //Debug.Log($"TWEENING LOCAL EULER FROM {targetTransform.localEulerAngles} TO {targetEuler}");
        //var tweener = targetTransform.DORotate(targetEuler, duration);
        var tweener = DOTween.To(
            () => targetTransform.localEulerAngles,
            x => targetTransform.localEulerAngles = x,
            targetEuler,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }
    private Tween GetLocalPositionTweener(Vector3 toPosition, float duration, Ease ease = Ease.Linear)
    {
        var tweener = DOTween.To(
            () => targetTransform.localPosition,
            x => targetTransform.localPosition = x,
            toPosition,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }
    private Tween GetWorldPositionTweener(Vector3 toPosition, float duration, Ease ease = Ease.Linear)
    {
        var tweener = DOTween.To(
            () => targetTransform.position,
            x => targetTransform.position = x,
            toPosition,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void Reset()
    {
        Stop();
        targetTransform.localScale = originalScale;
    }
    public void SetScale(Vector3 size)
    {
        targetTransform.localScale = size;
    }
    public void SetScale(float scale)
    {
        var size = new Vector3(scale, scale, scale);
        SetScale(size);
    }

    public void Move(Vector3 toPosition, float duration, Ease ease = Ease.Linear)
    {
        Stop();
        animation = GetWorldPositionTweener(toPosition, duration, ease);
    }
    public void Move(SequenceNode<Vector3>[] positions)
    {
        Stop();

        sequence = DOTween.Sequence();
        foreach (var p in positions)
            sequence.Append(GetWorldPositionTweener(p.value, p.duration, p.ease));
    }

    public void MoveLocal(Vector3 toPosition, float duration, Ease ease = Ease.Linear)
    {
        Stop();
        animation = GetLocalPositionTweener(toPosition, duration, ease);
    }
    public void MoveLocal(float toHeight, float duration, Ease ease = Ease.Linear)
    {
        var toPosition = targetTransform.localPosition;
        toPosition.y = toHeight;

        MoveLocal(toPosition, duration, ease);
    }
    public void MoveLocal(SequenceNode<Vector3>[] positions)
    {
        Stop();

        sequence = DOTween.Sequence();
        foreach (var p in positions)
            sequence.Append(GetLocalPositionTweener(p.value, p.duration, p.ease));
    }
    public void FloatLocalPosition(float formHeight, float toHeight, float duration)
    {
        Stop();

        var fromPosition = targetTransform.localPosition;
        fromPosition.y = formHeight;

        var toPosition = targetTransform.localPosition;
        toPosition.y = toHeight;

        targetTransform.localPosition = fromPosition;

        //animation = GetLocalPositionTweener(toPosition, duration, Ease.InOutCirc);
        sequence = DOTween.Sequence();
        sequence.Append(GetLocalPositionTweener(toPosition, duration / 2, Ease.InOutSine));
        sequence.Append(GetLocalPositionTweener(fromPosition, duration / 2, Ease.InOutSine));
    }

    public void Rotate(float degrees, bool clockwise, float duration, Ease ease = Ease.Linear)
    {
        Stop();

        animation = GetEulerTweener(degrees, clockwise, duration, ease);
    }
    public void RotateCircle(bool clockwise, float duration, Ease ease = Ease.Linear)
    {
        Rotate(360, clockwise, duration, ease);
    }

    public void Scale(Vector3 fromSize, Vector3 toSize, float duration, Ease ease = Ease.Linear)
    {
        Stop();

        SetScale(fromSize);

        animation = GetScaleTweener(toSize, duration, ease);
    }
    public void Scale(float fromScale, float toScale, float duration, Ease ease = Ease.Linear)
    {
        var fromSize = new Vector3(fromScale, fromScale, fromScale);
        var toSize = new Vector3(toScale, toScale, toScale);

        Scale(fromSize, toSize, duration, ease);
    }
    public void ScaleInOut(float fromScale, float toScale, float durationIn, float durationOut)
    {
        Stop();

        SetScale(fromScale);

        sequence = DOTween.Sequence();
        sequence.Append(GetScaleTweener(toScale, durationIn, Ease.Flash));
        sequence.Append(GetScaleTweener(fromScale, durationOut, Ease.Flash));
    }
    public void Scale(SequenceNode<float>[] scales)
    {
        Stop();

        sequence = DOTween.Sequence();
        foreach (var s in scales)
            sequence.Append(GetScaleTweener(s.value, s.duration, s.ease));
    }
}

public class DoTweenLevelSpeedHandler : DoTweenAnimationHandler
{
    public Level levelInstance;

    public DoTweenLevelSpeedHandler(Level levelInstance)
    {
        this.levelInstance = levelInstance;
    }

    private Tween GetTweener(float targetSpeed, float duration, Ease ease)
    {
        var tweener = DOTween.To(
            () => levelInstance.moveSpeed,
            x => levelInstance.moveSpeed = x,
            targetSpeed,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void SpeedMove(float targetSpeed, float duration, Ease ease = Ease.Linear)
    {
        Stop();

        animation = GetTweener(targetSpeed, duration, ease);
    }
}

public class DoTweenCameraHandler : DoTweenAnimationHandler
{
    public Camera targetCamera;

    private Tween GetFOVTweener(float targetFOV, float duration, Ease ease)
    {
        var tweener = DOTween.To(
            () => targetCamera.fieldOfView,
            x => targetCamera.fieldOfView = x,
            targetFOV,
            duration);

        tweener.SetEase(ease);

        return tweener;
    }

    public void TransitFOV(float targetFOV, float duration, Ease ease = Ease.Linear)
    {
        Stop();
        animation = GetFOVTweener(targetFOV, duration, ease);
    }
}

public struct SequenceNode<T>
{
    public T value;
    public float duration;
    public Ease ease;
}