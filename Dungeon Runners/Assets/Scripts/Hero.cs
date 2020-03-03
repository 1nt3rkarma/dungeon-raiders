using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public int row { get => block.GetRowIndex(); }
    public int rowView;
    public int line { get => block.GetLineIndex(); }
    public int lineView;

    public Block block;
    public Player player;

    public float shiftDuration = 0.25f;
    public bool isMoving = true;

    public Animator animator;
    public HeroAnimationHandler animHandler;

    public Coroutine leapRoutine;

    void Start()
    {
        block = Level.GetNearestBlock(transform.position);
        rowView = row;
    }

    void Update()
    {
        lineView = line;
    }

    public void Leap(MoveDirections direction)
    {
        if (leapRoutine == null)
        {
            if (direction == MoveDirections.Left && line == 0)
                return;
            if (direction == MoveDirections.Right && line == 2)
                return;

            leapRoutine = StartCoroutine(LeapRoutine(direction));
        }
    }

    IEnumerator LeapRoutine(MoveDirections direction)
    {
        var speed = 1 / shiftDuration;

        //animator.speed = speed;
        if (direction == MoveDirections.Left)
            animator.SetTrigger("leapL");
        if (direction == MoveDirections.Right)
            animator.SetTrigger("leapR");

        int sign;
        if (direction == MoveDirections.Left)
            sign = -1;
        else
            sign = 1;

        var targetPosition = transform.position + sign * transform.right;
        var distance = Mathf.Abs(transform.position.x - targetPosition.x);

        // Ждем событие анимации - прыжок
        while (!animHandler.eventLeap)
            yield return null;
        animHandler.eventLeap = false;

        while (distance > 0.1)
        {
            transform.position += sign * transform.right * speed * Time.deltaTime;
            distance = Mathf.Abs(transform.position.x - targetPosition.x);
            yield return null;
        }

        //animator.speed = 1;
        transform.position = targetPosition;
        leapRoutine = null;
    }

    void OnDrawGizmosSelected()
    {
        if (block != null)
        {
            Gizmos.color = Color.yellow;
            var center = block.transform.position + new Vector3(0.5f, 0, 0.5f);
            var size = Vector3.one / 2;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
