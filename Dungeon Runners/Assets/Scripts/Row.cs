using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public bool isPopedUp = false;

    public List<Block> blocks;
    public List<MeshRenderer> renderers;

    void Start()
    {
        if (!isPopedUp)
            PopUp();
    }

    public void SetPosition(int z)
    {
        transform.localPosition = new Vector3(0, 0, z);
    }

    public void Fade()
    {
        StartCoroutine(AnimationRoutine(RowAnimationModes.FadeDown));
    }

    public void PopUp()
    {
        StartCoroutine(AnimationRoutine(RowAnimationModes.PopUp));
    }

    void SetBlocksPosition(float y)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            var newPosition = blocks[i].transform.localPosition;
            newPosition.y = y;
            blocks[i].transform.localPosition = newPosition;
        }
    }

    void SetBlocksAlpha(float alpha)
    {
        Mathf.Clamp(alpha, 0, 1);

        for (int i = 0; i < renderers.Count; i++)
        {
            var newColor = renderers[i].material.color;
            newColor.a = alpha;
            renderers[i].material.color = newColor;
        }
    }

    IEnumerator AnimationRoutine(RowAnimationModes mode)
    {
        float y;
        float limit;
        float alpha;

        // Заданем направление движения и прироста/убыли альфа-канала
        // Стартовую позицию и альфа-канал блоков
        if (mode == RowAnimationModes.PopUp)
        {
            y = -1;
            limit = 0;
            alpha = 0;

            SetBlocksPosition(-1);
            SetBlocksAlpha(0);

            // Плавно двигаем их ВВЕРХ и делаем непрозрачными
            while (y < limit)
            {
                y += Time.deltaTime;
                alpha += Time.deltaTime;

                SetBlocksPosition(y);
                SetBlocksAlpha(alpha);

                yield return null;
            }

            // Выравниваем позицию и альфа канал
            SetBlocksPosition(limit);
            SetBlocksAlpha(1);
        }
        else
        {
            y = 0;
            limit = -1;
            alpha = 1;

            SetBlocksPosition(0);
            SetBlocksAlpha(1);

            // Плавно двигаем их ВНИЗ и делаем прозрачными
            while (y > limit)
            {
                y -= Time.deltaTime;
                alpha -= Time.deltaTime;

                SetBlocksPosition(y);
                SetBlocksAlpha(alpha);

                yield return null;
            }

            // Выравниваем позицию и альфа канал
            SetBlocksPosition(limit);
            SetBlocksAlpha(0);
        }
    }

    public enum RowAnimationModes { PopUp, FadeDown}
}
