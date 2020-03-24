using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviourExtended
{
    public Animator animatorScreen;
    public Animator animatorVisualHint;

    public ResourceBar progressBar;

    public List<RectTransform> screensRects;

    public int[] actionsPerPhase;

    public Phases phase = (Phases) 1;
    public int count;

    public int actionsCountMaxView;
    int actionsCountMax
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < actionsPerPhase.Length; i++)
                sum += actionsPerPhase[i];
            return sum;
        }
    }

    public int actionsCountView;
    int actionsCount
    {
        get
        {
            int sum = 0;
            for (int i = 0; i < actionsPerPhase.Length; i++)
                if (i < (int)phase)
                    sum += actionsPerPhase[i];
                else if (i == (int)phase)
                    sum += count;
            return sum;
        }
    }

    private void Awake()
    {
        SubsribeToGameEvents();
    }

    private void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    private void Update()
    {
        actionsCountMaxView = actionsCountMax;
        actionsCountView = actionsCount;
        progressBar.SetValue((float)actionsCount / actionsCountMax);
    }

    public void Run()
    {
        gameObject.SetActive(true);
        Level.DisableGeneration();
        progressBar.gameObject.SetActive(true);

        Hero.singlton.isListening = false;
        animatorScreen.SetTrigger("next");
    }

    protected override void OnHeroJump(Hero hero)
    {
        if (phase == Phases.Jumping)
            CountAction();
    }

    protected override void OnHeroLeap(Hero hero, LeapDirections direction)
    {
        if (phase == Phases.Leaping)
            CountAction();
    }

    protected override void OnHeroAttack(Hero hero)
    {
        if (phase == Phases.Attacking)
            CountAction();
    }

    protected override void OnSwipe(SwipeDirections direction)
    {
        if (phase == Phases.Movement)
            CountAction();
    }

    protected override void OnSingleTap()
    {
        if (phase == Phases.Movement || phase == Phases.Goodluck)
            CountAction();
    }

    protected override void OnTapHold()
    {
        if (phase == Phases.Movement || phase == Phases.Goodluck)
            CountAction();
    }

    public void OnNewScreen()
    {
        switch (phase)
        {
            case Phases.Movement:
                Hero.singlton.isListening = false;
                break;
            case Phases.Jumping:
                Hero.singlton.isListening = true;
                animatorVisualHint.SetTrigger("tap");
                break;
            case Phases.Leaping:
                Hero.singlton.isListening = true;
                animatorVisualHint.SetTrigger("swipeRL");
                break;
            case Phases.Attacking:
                Hero.singlton.isListening = true;
                animatorVisualHint.SetTrigger("swipeForward");
                break;
            case Phases.Goodluck:
                Hero.singlton.isListening = false;
                break;
            case Phases.FinishTutorial:
                FinishTutorial();
                break;
            default:
                break;
        }
    }

    void CountAction()
    {
        Debug.Log($"Засчитали действие в фазе: {phase}");
        int phaseIndex = (int)phase;

        count++;
        if (count >= actionsPerPhase[phaseIndex])
            SwitchScreen();
    }

    void SwitchScreen()
    {
        count = 0;
        int phaseIndex = (int)phase + 1;
        phase = (Phases)phaseIndex;

        animatorScreen.SetTrigger("next");
    }

    void FinishTutorial()
    {
        Hero.singlton.isListening = true;
        Level.EnableGeneration();
        PlayerPrefs.SetInt("tutorial", 0);
        Player.needTutorial = false;
        Player.stepsSession = 0;
        Player.stepsLevel = 0;
        UI.ShowGameplayUI();
        Destroy(gameObject);
    }

    public enum Phases { Movement = 0, Jumping = 1, Leaping = 2, Attacking = 3, Goodluck = 4, FinishTutorial = 5 }
}

