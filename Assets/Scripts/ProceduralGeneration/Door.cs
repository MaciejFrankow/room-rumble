using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isClosed = true;
    private bool isFrontierDoor = false;
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.SetActive(isClosed);
    }

    public void OpenDoor() { 
        if (!isFrontierDoor) 
            isClosed = false; 
    }
    public void CloseDoor() => isClosed = true;
    public void SetFrontier(bool value) => isFrontierDoor = value;
}
