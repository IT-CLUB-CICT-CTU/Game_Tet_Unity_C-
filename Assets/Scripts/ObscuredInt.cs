using UnityEngine;

[System.Serializable]
public class ObscuredInt
{
    private int hiddenValue;
    private int key;
    private bool inited = false;

    public void Init(int value)
    {
        key = Random.Range(1000, 9999);
        hiddenValue = value ^ key;
        inited = true;
    }

    public int Get()
    {
        if (!inited)
        {
            Init(0);
        }
        return hiddenValue ^ key;
    }

    public void Set(int value)
    {
        if (!inited)
        {
            Init(value);
            return;
        }

        hiddenValue = value ^ key; // ? KHÔNG ??i key
    }

    public void Add(int value)
    {
        int current = Get();
        Set(current + value);
    }
}
