using UnityEngine;

[System.Serializable]
public class SecureInt
{
    [SerializeField] private int projected;
    [SerializeField] private int key;
    [SerializeField] private int checksum;

    private bool initialized = false;

    public void Init(int value)
    {
        if (initialized) return;

        key = Random.Range(137, 999);
        Set(value);
        initialized = true;
    }

    int CalculateChecksum(int realValue)
    {
        unchecked
        {
            return realValue * 31 ^ key;
        }
    }

    public int Get()
    {
        int real = projected + key;

        // 🔥 detect memory tampering
        if (checksum != CalculateChecksum(real))
        {
            Debug.LogWarning("SECURE INT TAMPER DETECTED");
            Reset();
            return 0;
        }

        return real;
    }

    public void Set(int value)
    {
        projected = value - key;
        checksum = CalculateChecksum(value);
    }

    public void Add(int value)
    {
        Set(Get() + value);
    }

    public void ReKey()
    {
        int real = Get();
        if (real < 0) real = 0;

        key = Random.Range(137, 999);
        Set(real);
    }


    void Reset()
    {
        key = Random.Range(137, 999);
        projected = 0;
        checksum = CalculateChecksum(0);
    }
}
