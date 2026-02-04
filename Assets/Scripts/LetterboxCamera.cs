using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    public float targetAspect = 9f / 16f;
    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }

    void Update()
    {
        ApplyLetterbox();
    }

    void ApplyLetterbox()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f) 
        {
            float height = scaleHeight;
            cam.rect = new Rect(0f, (1f - height) / 2f, 1f, height);
        }
        else
        {
            float width = 1f / scaleHeight;
            cam.rect = new Rect((1f - width) / 2f, 0f, width, 1f);
        }
    }
}
