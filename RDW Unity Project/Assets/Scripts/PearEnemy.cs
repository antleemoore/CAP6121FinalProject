public class PearEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        
        Health = 20;
        changeDirectionInterval = 3f;

        KnockbackDistance = 1f;

        AttackInterval = 2f;
    }

    public override int Stab(int dmg)
    {
        return TakeDamage(dmg * 2);
    }

    public override int Slash(int dmg)
    {
        return base.Slash(dmg / 2);
    }
}