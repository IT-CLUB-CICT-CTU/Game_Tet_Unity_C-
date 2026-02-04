using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CatItem : MonoBehaviour
{
    public PlayerMove playerMove;
    public GameObject textObj, item;
    public TextMeshProUGUI textShow;
    public float floatUpDistance = 1.5f;
    public float floatDuration = 0.8f;

    void Update()
    {
        if (playerMove == null)
        {
            playerMove = FindObjectOfType<PlayerMove>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMove.X2Money();
            if (textObj != null)
            {
                textObj.transform.SetParent(null); // tách khỏi Lixi
                textObj.SetActive(true);
                item.SetActive(false);
                StartCoroutine(FloatAndFade());
            }
        }
    }

    IEnumerator FloatAndFade()
    {
        Vector3 startPos = textObj.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatUpDistance;

        Color startColor = textShow.color;
        Color endColor = startColor;
        endColor.a = 0;

        float time = 0;

        while (time < floatDuration)
        {
            float t = time / floatDuration;
            textObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            textShow.color = Color.Lerp(startColor, endColor, t);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(textObj);
        Destroy(gameObject);
    }
}
