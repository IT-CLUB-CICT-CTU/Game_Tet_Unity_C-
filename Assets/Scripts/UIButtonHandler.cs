using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class UIButtonHandler : MonoBehaviour
{
    private GameManager gameManager;
    public Button button;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        button = GetComponent<Button>();

        if (button != null && gameManager != null)
        {
            button.onClick.AddListener(InvokeByName);
        }
    }

    void InvokeByName()
    {
        string methodName = gameObject.name;

        MethodInfo method = gameManager.GetType().GetMethod(
            methodName,
            BindingFlags.Public | BindingFlags.Instance
        );

        if (method != null)
        {
            method.Invoke(gameManager, null);
        }
    }
}
