using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isClosed = false;
    private bool isFrontierDoor = false;
    void Start()
    {
        UpdateDoorState();
    }

    private void UpdateDoorState()
    {
        gameObject.SetActive(isClosed);
    }

    void Update()
    {
        //gameObject.SetActive(isClosed);
    }

    public void OpenDoor()
    {
        if (!isFrontierDoor && isClosed) 
        {
            isClosed = false;
            UpdateDoorState();
        }
    }

    public void CloseDoor()
    {
        if (!isClosed)  
        {
            isClosed = true;
            UpdateDoorState();
        }
    }
    public void SetFrontier(bool value) => isFrontierDoor = value;
}
