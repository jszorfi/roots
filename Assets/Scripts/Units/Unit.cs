using System;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public Vector2Int pos;
    public int maxHealth;
    public int health;
    public Tuple<ResourceType, int> cost;
    private RectTransform healthBar;

    protected CanvasController canvasController;
    protected SpriteAnimator animator;
    public void Start()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        animator = gameObject.GetComponent<SpriteAnimator>();
        healthBar = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
    }

    public abstract void onClicked();
    public void receiveDamage(int damage)
    {
        health -= damage;
        healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * healthBar.sizeDelta.x, healthBar.sizeDelta.y);
        if (health <= 0)
        {
            canvasController.mapController.Die(this);
            if (animator != null)
            {
                animator.SetAnimationByName("Die", delegate { Destroy(gameObject); });
            }
            else 
            {
                Destroy(gameObject);
            }
        }
    }
    public abstract void refresh();
}
