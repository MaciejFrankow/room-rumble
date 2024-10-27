using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public int roomCount = 10;

    private readonly List<Room> generatedRooms = new();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        Room firstRoom = InstantiateRoom(0);
        firstRoom.transform.position = Vector3.zero;
        generatedRooms.Add(firstRoom);

        int roomsGenerated = 1;
        while (roomsGenerated < roomCount)
        {
            Room newRoom = InstantiateRoom(Random.Range(0, roomPrefabs.Length));

            if (PositionRoom(newRoom))
            {
                generatedRooms.Add(newRoom);
            }

            roomsGenerated++;

        }
    }

    Room InstantiateRoom(int prefabIndex)
    {
        GameObject roomInstance = Instantiate(roomPrefabs[prefabIndex]);
        Room room = roomInstance.GetComponent<Room>();
        room.GenerateRandomRoom(); 
        return room;
    }

    bool PositionRoom(Room newRoom)
    {
        foreach (Room existingRoom in generatedRooms)
        {
            foreach (var existingConnectionPoint in existingRoom.connectionPoints) // Get Random
            {
                if (existingConnectionPoint == null || existingRoom.IsConnectionPointUsed(existingConnectionPoint))
                    continue;

                foreach (var newConnectionPoint in newRoom.connectionPoints) // Get Random
                {
                    if (newConnectionPoint == null || newRoom.IsConnectionPointUsed(newConnectionPoint))
                        continue;

                    if (TryConnectRooms(existingRoom, existingConnectionPoint, newRoom, newConnectionPoint))
                    {
                        return true;
                    }
                }
            }
        }

        Debug.Log($"Overlap detected after positioning {newRoom.name}.");
        //Destroy(newRoom.gameObject);
        return false;
    }


    bool TryConnectRooms(Room room1, Transform pointTransform1, Room room2, Transform pointTransform2)
    {
        Vector3 offset = pointTransform1.gameObject.transform.position - pointTransform2.gameObject.transform.position;

        room2.gameObject.transform.position += offset;

        for (int rotation = 0; rotation <= 3; rotation++)
        {
            room2.gameObject.transform.RotateAround(pointTransform1.position, Vector3.up, 90 * rotation);

            Debug.Log(rotation);

            if (!RoomsOverlap(room2))
            {
                room1.MarkConnectionPointAsUsed(pointTransform1);
                room2.MarkConnectionPointAsUsed(pointTransform2);

                Debug.Log($"Connected {room1.name} and {room2.name} at points {pointTransform1.name} and {pointTransform2.name}");
                return true;
            }
        }
        Debug.Log($"Failed to connect {room1.name} and {room2.name} due to overlap.");
        return false;
    }


    bool RoomsOverlap(Room newRoom)
    {
        Collider[] newRoomColliders = newRoom.GetColliders();

        foreach (Room existingRoom in generatedRooms)
        {
            Collider[] existingRoomColliders = existingRoom.GetColliders();

            foreach (Collider newCollider in newRoomColliders)
            {
                foreach (Collider existingCollider in existingRoomColliders)
                {
                    if (newCollider.bounds.Intersects(existingCollider.bounds))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

