using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leap : Skill
{
    public string leapLeftTag = "leapL";
    public string leapRightTag = "leapR";

    [HideInInspector]
    public LeapDirections direction;

    void Update()
    {
        OnUpdate();
    }

    protected override void BeginScenario()
    {
        base.BeginScenario();

        if (direction == LeapDirections.Left)
            RequestAnimation(leapLeftTag);
        if (direction == LeapDirections.Right)
            RequestAnimation(leapRightTag);
    }

    protected override void CastStarted()
    {
        caster.isLeaping = true;
        StartCoroutine(TransitionRoutine(direction));
        GameEvent.InvokeUnitLeap(caster, direction);
    }

    IEnumerator TransitionRoutine(LeapDirections direction)
    {
        var speed = 1 / caster.leapDuration;

        var targetBlock = Level.GetBlock(caster.row, caster.line + (int)direction);

        var x = targetBlock.transform.position.x;
        var y = caster.transform.position.y;
        var z = caster.transform.position.z;

        var targetPosition = new Vector3(x, y, z);

        if (direction == LeapDirections.Left)
            while (caster.transform.position.x > targetPosition.x)
            {
                caster.transform.position -= caster.transform.right * speed * Time.deltaTime;
                if (caster.transform.position.x <= targetPosition.x)
                    break;
                yield return null;
            }
        else if (direction == LeapDirections.Right)
            while (caster.transform.position.x < targetPosition.x)
            {
                caster.transform.position += caster.transform.right * speed * Time.deltaTime;
                if (caster.transform.position.x >= targetPosition.x)
                    break;
                yield return null;
            }
        caster.transform.position = targetPosition;
    }

    public override void Interrupt()
    {
        base.Interrupt();
        caster.isLeaping = false;
    }
}

public enum LeapDirections { None = 0, Right = 1, Left = -1 }