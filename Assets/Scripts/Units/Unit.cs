using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public Vector2Int pos;
    public int maxHealth;
    public int health;

    protected CanvasController canvasController;
    protected SpriteAnimator animator;
    private void Start()
    {
        canvasController = GameObject.Find("Canvas").GetComponent<CanvasController>();
        animator = gameObject.GetComponent<SpriteAnimator>();
    }

    public abstract void onClicked();
    public void receiveDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            animator.SetAnimationByName("Die");
        }
    }
}
