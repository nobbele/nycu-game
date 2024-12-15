using UnityEngine;

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public Sprite icon;

    public Item(string id, string name, Sprite icon)
    {
        this.id = id;
        this.name = name;
        this.icon = icon;
    }
}
