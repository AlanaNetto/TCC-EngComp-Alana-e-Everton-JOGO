using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecicleBin : MonoBehaviour
{
    public string trashTypeAccepted;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            GameController.gameController.DiscardTrash(trashTypeAccepted);
        }
    }
}
