using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public RectTransform rect;

    public SkillButton buttonPref;
    public SkillButton button1;
    public SkillButton button2;

    //void Start()
    //{
    //    Init(Hero.singlton.skills, Hero.singlton);
    //}

    //void Init(List<Skill> skills, Unit unit)
    //{
    //    foreach (var skill in skills)
    //        if (skill.drawUIButton)
    //            mainButton = CreateButton(skill, unit);
    //}

    SkillButton CreateButton(Skill skill, Unit unit)
    {
        SkillButton button = Instantiate(buttonPref, rect);

        button.Init(unit, skill);
        return button;
    }
}
