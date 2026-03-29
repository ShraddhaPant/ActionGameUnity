
using UnityEngine;

public class PlayerHealth_2 : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log("Health: " + health);
    }
}