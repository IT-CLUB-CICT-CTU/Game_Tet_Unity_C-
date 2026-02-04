using System.Collections.Generic;
using UnityEngine;

public class CustomManager : MonoBehaviour
{
    public enum ItemType
    {
        Shirt,
        Pants,
        Helmet,
        Accessory
    }

    [System.Serializable]
    public class CustomItem
    {
        public ItemType type;
        public List<GameObject> items;
        [HideInInspector] public int currentIndex;
    }

    public List<CustomItem> customItems;

    void Start()
    {
        foreach (var item in customItems)
        {
            string key = item.type + "Index";
            item.currentIndex = PlayerPrefs.GetInt(key, 0);
            ApplyItem(item, item.currentIndex);
        }
    }

    // ===== BUTTON CALL =====
    public void Next(ItemType type)
    {
        CustomItem item = GetItem(type);
        if (item == null || item.items.Count == 0) return;

        item.currentIndex = (item.currentIndex + 1) % item.items.Count;
        ApplyItem(item, item.currentIndex);
        Save(item);
    }

    public void Prev(ItemType type)
    {
        CustomItem item = GetItem(type);
        if (item == null || item.items.Count == 0) return;

        item.currentIndex = (item.currentIndex - 1 + item.items.Count) % item.items.Count;
        ApplyItem(item, item.currentIndex);
        Save(item);
    }

    // =======================
    void ApplyItem(CustomItem item, int index)
    {
        foreach (var go in item.items)
            go.SetActive(false);

        if (index >= 0 && index < item.items.Count)
            item.items[index].SetActive(true);
    }

    void Save(CustomItem item)
    {
        PlayerPrefs.SetInt(item.type + "Index", item.currentIndex);
    }

    CustomItem GetItem(ItemType type)
    {
        return customItems.Find(x => x.type == type);
    }

    // ===== SHIRT =====
    public void NextShirt() => Next(ItemType.Shirt);
    public void PrevShirt() => Prev(ItemType.Shirt);

    // ===== PANTS =====
    public void NextPants() => Next(ItemType.Pants);
    public void PrevPants() => Prev(ItemType.Pants);

    // ===== HELMET =====
    public void NextHelmet() => Next(ItemType.Helmet);
    public void PrevHelmet() => Prev(ItemType.Helmet);

    // ===== ACCESSORY =====
    public void NextAccessory() => Next(ItemType.Accessory);
    public void PrevAccessory() => Prev(ItemType.Accessory);

}
