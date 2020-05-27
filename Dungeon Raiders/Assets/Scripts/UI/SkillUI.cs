using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public RectTransform rect;

    public SkillButton buttonPref;
    public SkillButton mainButton;

    void Start()
    {
        Init(Hero.singlton.skills, Hero.singlton);
    }

    void Init(List<Skill> skills, Unit unit)
    {
        foreach (var skill in skills)
            if (skill.drawUIButton)
                mainButton = CreateButton(skill, unit);
    }

    SkillButton CreateButton(Skill skill, Unit unit)
    {
        SkillButton button = Instantiate(buttonPref, rect);

        var w = buttonPref.rect.sizeDelta.x;
        var h = buttonPref.rect.sizeDelta.x;

        button.Init(unit, skill, w, h);
        return button;
    }
}
