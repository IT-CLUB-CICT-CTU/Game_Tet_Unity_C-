using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class LixiRate
{
    public int value;
    public int weight;
}

public class Lixi : MonoBehaviour
{
    public LixiCountShow show;
    public int value;

    [Header("Text Popup")]
    public GameObject textObj;
    public TextMeshProUGUI textShow;
    public float floatUpDistance = 1.5f;
    public float floatDuration = 0.8f;

    [Header("Lixi Rates")]
    public List<LixiRate> rates = new List<LixiRate>();

    public GameObject mr;
    public BoxCollider col;
    public AudioSource audioSource;

    void Awake()
    {
        value = GetRandomValueByWeight();

        if (textShow != null)
            textShow.text = "+" + value + "K";

        if (textObj != null)
            textObj.SetActive(false);

        if (show == null)
            show = FindObjectOfType<LixiCountShow>();
    }

    int GetRandomValueByWeight()
    {
        int totalWeight = 0;
        foreach (var item in rates)
            totalWeight += item.weight;

        int random = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var item in rates)
        {
            current += item.weight;
            if (random < current)
                return item.value;
        }

        return rates[0].value;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerMove playerMove = other.GetComponent<PlayerMove>();
        if (playerMove.isX2Money)
            value *= 2;

        if (show != null)
            show.UpdateCount(value);

        if (textObj != null)
        {
            textObj.transform.SetParent(null); // tách khỏi Lixi
            textObj.SetActive(true);
            StartCoroutine(FloatAndFade());
        }

        // Ẩn Lixi mesh
        if (mr != null)
            mr.SetActive(false);

        if (col != null)
            col.enabled = false;
        if (audioSource != null)
            audioSource.Play();
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
