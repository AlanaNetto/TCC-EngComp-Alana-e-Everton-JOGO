using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Obstacle
{
    public float stepSize;
    public override void ExecuteAction(){
        this.transform.Translate(0,stepSize,0);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collided with car");
        if(col.tag == "Player")
        {
            col.GetComponent<CharacterController>().CarCrash();
        }
    }
}
