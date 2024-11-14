using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType { Square, Rectangle, LShaped };
    public RoomType roomType;

    public Transform[] connectionPoints;
    public List<Transform> usedConnectionPoints = new();

    public bool IsConnectionPointUsed(Transform transform) => usedConnectionPoints.Contains(transform);

    public void MarkConnectionPointAsUsed(Transform transform)
    {
        if (!usedConnectionPoints.Contains(transform))
            usedConnectionPoints.Add(transform);
    }

    public void GenerateRandomRoom()
    {
    }

    public Collider[] GetColliders()
    {
        return GetComponentsInChildren<Collider>();
    }

}
