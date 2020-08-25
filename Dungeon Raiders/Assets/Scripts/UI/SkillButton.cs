using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Skill skill;
    public Unit caster;

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

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        icon.sprite = skill.icon;
        iconDisabled.sprite = skill.iconDisabled;

        icon.enabled = true;
        background.enabled = true;

        iconDisabled.enabled = false;
        backgroundDisabled.enabled = false;

        frame.color = frameNormal;
        frame.fillAmount = 1;

        heightDefaut = rect.sizeDelta.x;
        widthDefaut = rect.sizeDelta.x;

        ResetSize();
    }

    public void Init(Unit caster, Skill skill)
    {
        this.skill = skill;
        this.caster = caster;

        icon.sprite = skill.icon;
        iconDisabled.sprite = skill.iconDisabled;

        icon.enabled = true;
        background.enabled = true;

        iconDisabled.enabled = false;
        backgroundDisabled.enabled = false;

        frame.color = frameNormal;
        frame.fillAmount = 1;

        ResetSize();
    }

    void Update()
    {

        if (isPressed && skill.state == SkillStates.idling)
            caster.Use(skill);

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
        caster.Use(skill);
        isPressed = true;
    }

    public void OnRelease()
    {
        caster.Cancel(skill);
        isPressed = false;
    }

    public void DecreaseSize()
    {
        rect.localScale = new Vector3(1 - sizeDecOnPress, 1 - sizeDecOnPress, 1);
    }

    public void ResetSize()
    {
        rect.localScale = new Vector3(1, 1, 1);
    }
}
