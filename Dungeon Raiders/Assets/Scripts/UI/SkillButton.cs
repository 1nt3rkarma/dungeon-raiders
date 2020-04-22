using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Skill skill;

    public RectTransform rect;
    public float heightDefaut;
    public float widthDefaut;

    public Image icon;
    public Image background;

    public Image iconDisabled;
    public Image backgroundDisabled;

    public Image frame;
    public Color frameNormal;
    public Color frameRecharging;

    public float sizeDecOnPress = 0.1f;

    public bool isPressed = false;

    public void Init(Skill skill, float w, float h)
    {
        this.skill = skill;

        icon.sprite = skill.icon;
        iconDisabled.sprite = skill.iconDisabled;

        icon.enabled = true;
        background.enabled = true;

        iconDisabled.enabled = false;
        backgroundDisabled.enabled = false;

        frame.color = frameNormal;
        frame.fillAmount = 1;

        heightDefaut = h;
        widthDefaut = w;

        ResetSize();
    }

    void Update()
    {
        if (skill.state == SkillStates.idling && skill.cooldown > 0 && skill.cooldownTimer > 0)
        {
            icon.enabled = false;
            background.enabled = false;

            iconDisabled.enabled = true;
            backgroundDisabled.enabled = true;

            frame.color = frameRecharging;

            DecreaseSize();

            frame.fillAmount = (skill.cooldown - skill.cooldownTimer) / skill.cooldown;
        }
        else
        {
            icon.enabled = true;
            background.enabled = true;

            iconDisabled.enabled = false;
            backgroundDisabled.enabled = false;

            frame.color = frameNormal;
            frame.fillAmount = 1;

            if (isPressed)
                DecreaseSize();
            else
                ResetSize();
        }
    }

    public void OnPress()
    {
        skill.Use();
        isPressed = true;
    }

    public void OnRelease()
    {
        skill.Cancel();
        isPressed = false;
    }

    public void DecreaseSize()
    {
        float w = widthDefaut * (1- sizeDecOnPress);
        float h = heightDefaut * (1 - sizeDecOnPress);

        rect.sizeDelta = new Vector2(w, h);
    }

    public void ResetSize()
    {
        float w = widthDefaut;
        float h = heightDefaut;

        rect.sizeDelta = new Vector2(w, h);
    }
}
