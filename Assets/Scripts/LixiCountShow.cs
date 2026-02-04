using UnityEngine;
using TMPro;

public class LixiCountShow : MonoBehaviour
{
    public GameManager gameManager;
    public SecureInt count = new SecureInt();
    public TextMeshProUGUI textShow;

    void Awake()
    {
        count.Init(0);
    }

    void Start()
    {
        Show();

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
            gameManager.lixiCountShow = this;
    }

    public void UpdateCount(int amount)
    {
        count.Add(amount);
        Show();
    }

    void Show()
    {
        textShow.text = count.Get().ToString() + "K";
    }

    public int GetCount()
    {
        return count.Get();
    }
}
