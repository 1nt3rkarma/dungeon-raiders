using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialUI : MonoBehaviourExtended
{
    public Image tintGameplay;

    public Animator animatorScreen;
    public Animator animatorVisualHint;

    public Text textMain;
    public Text textNext;

    public int phaseIndex = -1;
    public List<TutorialPhase> phases;
    public TutorialPhase phase { get => phases[phaseIndex]; }

    public int actionsCount;
    public int actionsTotal
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < phaseIndex; i++)
                sum += phases[i].actionsCount;
            sum += actionsCount;
            return sum;        
        }
    }

    public int actionsMax
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < phases.Count; i++)
                sum += phases[i].actionsCount;
            return sum;
        }
    }

    public bool isBusy = false;

    private void Awake()
    {
        SubsribeToGameEvents();
    }

    private void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    public void Run()
    {
        gameObject.SetActive(true);

        Level.DisableGeneration();

        SwitchPhase();
    }

    protected override void OnUnitJump(Unit unit)
    {
        if (unit is Hero)
            if (phase.catchAction == PlayerActions.HeroJumps)
                CountAction();
    }

    protected override void OnUnitLeap(Unit unit, LeapDirections direction)
    {
        if (unit is Hero)
            if (phase.catchAction == PlayerActions.HeroLeaps)
                CountAction();
    }

    protected override void OnUnitAttack(Unit unit)
    {
        if (unit is Hero)
            if (phase.catchAction == PlayerActions.HeroAttacks)
                CountAction();
    }

    protected override void OnSwipe(SwipeDirections direction)
    {
        if (phase.catchAction == PlayerActions.AnyInput)
            CountAction();
    }

    protected override void OnStationary()
    {
        if (phase.catchAction == PlayerActions.AnyInput)
            CountAction();
    }

    void CountAction()
    {
        if (!isBusy)
        {
            actionsCount++;
            if (actionsCount >= phases[phaseIndex].actionsCount)
                SwitchPhase();
        }
    }

    void SwitchPhase(int index)
    {
        actionsCount = 0;

        isBusy = true;

        phaseIndex = Mathf.Clamp(index, 0, phases.Count);

        if (phaseIndex < phases.Count)
        {
            textNext.text = phases[phaseIndex].text;
            animatorScreen.SetTrigger("switchScreen");
        }
        else
        {
            StartCoroutine(FinishTutorial());
        }
    }

    void SwitchPhase()
    {
        SwitchPhase(phaseIndex + 1);
    }

    public void OnNewScreen()
    {
        isBusy = false;

        textMain.text = phases[phaseIndex].text;

        if (phase.forceHeroIngnorance)
            Hero.singlton.isListening = false;
        else
            Hero.singlton.isListening = true;

        if (phase.needTint)
            tintGameplay.enabled = true;
        else
            tintGameplay.enabled = false;

        if (phase.hintAnimationTag != "")
            animatorVisualHint.SetTrigger(phase.hintAnimationTag);
        else
            animatorVisualHint.SetTrigger("none");

    }

    IEnumerator FinishTutorial()
    {
        yield return new WaitForEndOfFrame();

        Hero.singlton.isListening = true;
        Level.EnableGeneration();
        PlayerPrefs.SetInt("tutorial", 0);
        Player.needTutorial = false;
        Player.stepsTotal = 0;
        Player.steps = 0;
        UI.ShowGameplayUI();
        Destroy(gameObject);
    }
}

public enum PlayerActions { AnyInput, HeroJumps, HeroLeaps, HeroAttacks, HeroUseSkill }

[System.Serializable]
public class TutorialPhase : object
{
    public string label;
    [TextArea]
    public string text;
    public int actionsCount;
    public bool forceHeroIngnorance;
    public PlayerActions catchAction;

    public string hintAnimationTag;
    public bool needTint;
}
