using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleEnemy : EnemyBase
{
    public override int Stab(int dmg)
    {
        return TakeDamage(2*dmg); //double dmg from stabs
    }
}
