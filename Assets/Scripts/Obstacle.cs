using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public virtual void ExecuteAction(){}

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collided");
        if(col.tag == "Player")
        {
            // GameController.gameController.CollidedWithObstacle(this);
        }
    }
}
