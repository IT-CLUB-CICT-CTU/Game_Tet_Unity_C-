using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public float time = 3f;
    public Image fill;
    public bool isLoaded = false;

    void Update()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Load());
        }
    }

    IEnumerator Load()
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            fill.fillAmount = t / time; 

            yield return null; 
        }

        fill.fillAmount = 1f;
        isLoaded = true;
        Debug.Log("Load Done");
        this.gameObject.SetActive(false);
    }
}
