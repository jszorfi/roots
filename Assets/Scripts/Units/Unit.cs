using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public int maxHealth;
    public int health;

    protected CanvasController canvasController;
    private void Start()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
    }
    public abstract void onClicked();
    public void receiveDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {

        }
    }

    public void heal(int healing)
    {
        health += healing;
        if (health > maxHealth)
            health = maxHealth;
    }
}