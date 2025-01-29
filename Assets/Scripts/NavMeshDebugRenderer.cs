using UnityEngine;
using UnityEngine.AI;

public class NavMeshDebugRenderer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

        Gizmos.color = Color.cyan;
        for (int i = 0; i < triangulatedNavMesh.indices.Length; i += 3)
        {
            Vector3 v0 = triangulatedNavMesh.vertices[triangulatedNavMesh.indices[i]];
            Vector3 v1 = triangulatedNavMesh.vertices[triangulatedNavMesh.indices[i + 1]];
            Vector3 v2 = triangulatedNavMesh.vertices[triangulatedNavMesh.indices[i + 2]];

            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }
}
