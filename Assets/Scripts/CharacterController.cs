using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float stepSize;
    public bool canMakeActions = true;

    public void CarCrash(){
        canMakeActions = false;
    }

    public void Walk(){
        this.transform.Translate(stepSize,0,0);
    }

    public void Turn90(){
        this.transform.Rotate(0,0,90.0f);
    }

}
