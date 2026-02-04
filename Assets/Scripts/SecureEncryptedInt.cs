using System;
using UnityEngine;

[System.Serializable]
public class SecureEncryptedInt
{
    // 🔒 dữ liệu rác
    [SerializeField] private string encryptedValue;

    // 🔑 khoá
    [SerializeField] private int salt;

    // 🧪 chống tráo
    [SerializeField] private int checksum;

    private bool inited;

    // ================= INIT =================
    public void Init(int value)
    {
        salt = GenerateSalt();
        encryptedValue = Encrypt(value, salt);
        checksum = GenerateChecksum(value, salt);
        inited = true;
    }

    // ================= GET =================
    public int Get()
    {
        if (!inited)
            Init(0);

        int value = Decrypt(encryptedValue, salt);

        // 🚨 PHÁT HIỆN TRÁO
        if (checksum != GenerateChecksum(value, salt))
        {
            Debug.LogError("🚨 SCORE CHEAT DETECTED!");
            Reset();
            return 0;
        }

        return value;
    }

    // ================= SET =================
    public void Set(int value)
    {
        if (!inited)
        {
            Init(value);
            return;
        }

        // 🔄 đổi salt mỗi lần set
        salt = GenerateSalt();
        encryptedValue = Encrypt(value, salt);
        checksum = GenerateChecksum(value, salt);
    }

    // ================= ADD =================
    public void Add(int value)
    {
        int current = Get();
        Set(current + value);
    }

    // ================= RESET =================
    public void Reset()
    {
        Init(0);
    }

    // ================= CRYPTO =================
    private string Encrypt(int value, int salt)
    {
        int mixed = value ^ salt;
        byte[] bytes = BitConverter.GetBytes(mixed);
        return Convert.ToBase64String(bytes);
    }

    private int Decrypt(string encrypted, int salt)
    {
        byte[] bytes = Convert.FromBase64String(encrypted);
        int mixed = BitConverter.ToInt32(bytes, 0);
        return mixed ^ salt;
    }

    // ================= UTILS =================
    private int GenerateSalt()
    {
        return UnityEngine.Random.Range(1000, 9999);
    }

    private int GenerateChecksum(int value, int salt)
    {
        // checksum không lộ value
        return (value + 17) * (salt ^ 31);
    }
}
