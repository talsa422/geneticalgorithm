using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour, Killable
{
    GameManager gmRef;
    void Start()
    {
        Invoke("KillMessage", 150f);
    }

    public void KillMessage()
    {
        GmRef.TransformList.Remove(this.transform);
        Destroy(this.gameObject);
    }

    public bool IsEnemy()
    {
        return false;
    }

    public GameManager GmRef { get => gmRef; set => gmRef = value; } 
}
