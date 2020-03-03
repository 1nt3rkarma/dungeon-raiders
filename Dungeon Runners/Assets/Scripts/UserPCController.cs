using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPCController : MonoBehaviour
{
    public Player player;

    void Start()
    {
        
    }

    void Update()
    {
        var direction = 0;
        if (Input.GetKeyDown(KeyCode.A))
            direction -= 1;
        if (Input.GetKeyDown(KeyCode.D))
            direction += 1;


        if (direction != 0)
            player.OrderLeapHero((MoveDirections)direction);
    }
}
