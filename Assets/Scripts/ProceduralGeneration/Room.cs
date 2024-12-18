using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Door[] Doors;

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void OpenAllPossibleDoors()
    {
        foreach (Door door in Doors) {
            door.OpenDoor();
        }
    }

    public Door GetDoorAtPosition(Transform doorTransform) => Doors.FirstOrDefault(e => e.transform.position.x == doorTransform.transform.position.x && e.transform.position.z == doorTransform.transform.position.z);
}
