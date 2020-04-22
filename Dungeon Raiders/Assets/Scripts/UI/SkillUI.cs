using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public RectTransform rect;

    public SkillButton buttonPref;

    void Start()
    {
        Init(Hero.singlton.skills);
    }

    void Init(List<Skill> skills)
    {
        foreach (var skill in skills)
            if (skill.drawUIButton)
                CreateButton(skill);
    }

    SkillButton CreateButton(Skill skill)
    {
        SkillButton button = Instantiate(buttonPref, rect);

        var w = buttonPref.rect.sizeDelta.x;
        var h = buttonPref.rect.sizeDelta.x;

        button.Init(skill, w, h);
        return button;
    }
}
