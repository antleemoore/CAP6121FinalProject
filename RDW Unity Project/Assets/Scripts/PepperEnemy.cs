public class PepperEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        MoveSpeed = 0.3f; //slow pepper
        Health = 30;
        changeDirectionInterval = 3f;

        KnockbackDistance = 2f;

        AttackInterval = 7f;
    }

    public override int Stab(int dmg)
    {
        return TakeDamage(dmg * 2);
    }
}