public class Carrot : Character
{
    public int extraAttackDamage;

    public override void onClicked()
    {
        canvasController.displayCarrotSkills();
    }

    public void baseAttack(Unit target)
    {
        target.receiveDamage(damage * damageMultiplier);
    }

    public void extraAttack(Unit target)
    {
        target.receiveDamage(extraAttackDamage * damageMultiplier);
    }
}
