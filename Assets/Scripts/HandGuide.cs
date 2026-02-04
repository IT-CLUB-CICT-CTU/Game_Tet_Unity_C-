using UnityEngine;

public class HandGuide : MonoBehaviour
{
    public float scaleAmount = 0.15f; // độ zoom (0.1–0.2 là đẹp)
    public float speed = 2f;

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float s = 1f + Mathf.Sin(Time.unscaledTime * speed) * scaleAmount;
        transform.localScale = startScale * s;
    }
}
