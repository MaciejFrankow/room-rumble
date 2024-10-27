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
        // This method can be used to handle specific room setup logic if needed,
        // like setting random materials, decorations, or any other features.
        // Since you're not using size, this can be left empty or removed.
    }

    public Collider[] GetColliders()
    {
        return GetComponentsInChildren<Collider>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (connectionPoints != null)
        {
            foreach (var point in connectionPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawSphere(point.position, 0.2f); // Visualize connection points as small spheres
                }
            }
        }
    }
}
