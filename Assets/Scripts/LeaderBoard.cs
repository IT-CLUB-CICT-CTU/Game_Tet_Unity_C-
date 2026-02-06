using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour
{
    public Transform content;
    public GameObject itemPrefab;
    public int limit = 10;

    public TMP_Text myRank;
    public TMP_Text myName;
    public TMP_Text myScore;

    void OnEnable()
    {
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        // Đợi FirebaseManager tồn tại
        while (FirebaseManager.Instance == null)
            yield return null;

        // Đợi Firebase sign-in xong
        while (!FirebaseManager.Instance.IsReady)
            yield return null;

        Load();
    }

    void Load()
    {
        FirebaseManager.Instance.LoadLeaderBoard(
            limit,
            OnTopLoaded,
            OnMyLoaded
        );
    }

    void OnTopLoaded(List<RankData> list)
    {
        if (content == null || itemPrefab == null)
            return;

        foreach (Transform c in content)
            Destroy(c.gameObject);

        for (int i = 0; i < list.Count; i++)
        {
            GameObject item = Instantiate(itemPrefab, content);
            TMP_Text[] t = item.GetComponentsInChildren<TMP_Text>();

            if (t.Length >= 3)
            {
                t[0].text = (i + 1).ToString();
                t[1].text = list[i].name;
                t[2].text = list[i].score.ToString();
            }
        }
    }

    void OnMyLoaded(MyRankInfo info)
    {
        if (myRank == null || myName == null || myScore == null)
            return;

        myRank.text = info.rank.ToString();
        myName.text = info.name;
        myScore.text = info.score.ToString();
    }
}
