using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;
    public bool showFps = false;

    void Update()
    {
        if (!showFps) return;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";

        if (fps > 60)
            fpsText.color = Color.green;
        else if (fps > 30)
            fpsText.color = Color.yellow;
        else
            fpsText.color = Color.red;
    }
}
