using UnityEngine;

public class RotateCharInMainMenu : MonoBehaviour
{
    public Transform target;
    public float rotateSpeed = 0.3f;

    private bool isDragging;
    private Vector2 lastPos;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        MouseRotate();
#else
        TouchRotate();
#endif
    }

    void MouseRotate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            float deltaX = currentPos.x - lastPos.x;

            target.Rotate(Vector3.up, -deltaX * rotateSpeed, Space.World);

            lastPos = currentPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void TouchRotate()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.position.x - lastPos.x;

                target.Rotate(Vector3.up, -deltaX * rotateSpeed, Space.World);

                lastPos = touch.position;
            }
        }
    }
}
