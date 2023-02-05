using System;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public Vector2Int pos;
    public int maxHealth;
    public int health;
    public Tuple<ResourceType, int> cost;
    protected RectTransform healthBar;
    protected float maxHealthWidth;

    // Test
    private RectTransform hb_mid;
    private RectTransform hb_left;
    private RectTransform hb_right;

    protected CanvasController canvasController;
    protected GameController gameController;
    protected SpriteAnimator animator;
    public void Awake()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void Start()
    {
        animator = gameObject.GetComponent<SpriteAnimator>();
        healthBar = gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        maxHealthWidth = healthBar.sizeDelta.x;
    }

    public abstract void onClicked();
    public virtual void receiveDamage(int damage)
    {
        health -= damage;
        healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * maxHealthWidth, healthBar.sizeDelta.y);
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
    public virtual void counterAttack(Unit unit)
    {
    }
}
