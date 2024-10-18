using System;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    public int Damage = 0;
    public int Speed = 0;
    public int Stamina = 0;
}
public class Player : MonoBehaviour
{

    public int Experience;
    public int Level;

    public int Health = 100;

    public int AttributePoints;

    public PlayerAttributes Attributes;

    public int XpRequired => 20 * Level + 100;

    void Update()
    {
        if (Experience >= XpRequired)
            LevelUp();

        if (Health <= 0)
            GameOver();

        if (Input.GetKeyDown(KeyCode.U))
            Experience += 10;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"Health -{damage} = ({Health})");

        if (Health <= 0)
        {
            GameOver();
        }
    }

    void LevelUp()
    {
        Experience = 0;
        Level += 1;
        AttributePoints += 1;
        print($"Level Up: {Level - 1} -> {Level}");
        print($"+1 Attribute Point");
        // TODO Update UI
    }

    void GameOver()
    {
        print("Game over");
        // TODO
    }
}
