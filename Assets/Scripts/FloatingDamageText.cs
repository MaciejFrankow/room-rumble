using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeOutTime = 1f;
    private TextMeshPro textMesh;
    private Color startColor;

    public void Initialize(float damage, Vector3 position)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = damage.ToString("F0");
        transform.position = position + Vector3.up * 0.5f;


        startColor = textMesh.color;
        Destroy(gameObject, fadeOutTime);
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        startColor.a -= Time.deltaTime / fadeOutTime;
        textMesh.color = startColor;

        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }

    }
}
