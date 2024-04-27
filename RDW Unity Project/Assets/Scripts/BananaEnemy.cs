public class BananaEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        MoveSpeed = 1f; //fast banana
        Health = 20;
        changeDirectionInterval = 1f;

        KnockbackDistance = 4f;

        AttackInterval = 4f;
    }

    public override int Slash(int dmg)
    {
        return base.Slash(dmg * 2);
    }
}