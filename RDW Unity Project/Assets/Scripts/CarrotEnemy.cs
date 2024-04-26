public class CarrotEnemy : EnemyBase
{
    protected override void Start()
    {
        base.Start();

        MoveSpeed = 1.5f; //fast carrots
        Health = 15;
        changeDirectionInterval = 1f;

        KnockbackDistance = 5f;

        AttackInterval = 3f;
    }

    public override int Slash(int dmg)
    {
        return base.Slash(dmg * 2);
    }

    public override int Stab(int dmg)
    {
        return base.Stab(dmg / 2); //half dmg from stabs
    }
}