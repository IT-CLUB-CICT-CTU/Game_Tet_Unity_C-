using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenButton : MonoBehaviour
{
    public List<GameObject> objectsToDisable = new List<GameObject>();
    public List<GameObject> objectsToEnable = new List<GameObject>();
    public bool isPause = false;

    public void Open()
    {
        if (objectsToDisable.Count != 0 )
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
        if (objectsToEnable.Count != 0)
            foreach (GameObject obj in objectsToEnable)
            {
            obj.SetActive(true);
            }
        if (objectsToDisable.Count == 0 && objectsToEnable.Count == 0)
        {
            return;
        }

        if (isPause) return;
        Time.timeScale = 0f;
    }

    public void Closed()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(false);
        }
        Time.timeScale = 1f;
    }
}
