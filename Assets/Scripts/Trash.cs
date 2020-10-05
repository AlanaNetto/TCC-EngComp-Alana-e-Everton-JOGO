using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public string trashType;
    public bool collected;
    public bool discarted;
    public bool discartedCorrectly;

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collided");
        if(col.tag == "Player")
        {
            GameController.gameController.ColectTrash(this);
            this.gameObject.SetActive(false);
        }
    }
}

