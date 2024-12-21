using UnityEngine;

[System.Serializable]
public class Item
{
    public string id;
    public string name;
    public Sprite icon;
    public string info;

    public Item(string id, string name, Sprite icon, string info)
    {
        this.id = id;
        this.name = name;
        this.icon = icon;
        this.info = info;
    }

    // TODO: remove this constructor
    public Item(string id, string name, Sprite icon)
    {
        this.id = id;
        this.name = name;
        this.icon = icon;
    }
}
