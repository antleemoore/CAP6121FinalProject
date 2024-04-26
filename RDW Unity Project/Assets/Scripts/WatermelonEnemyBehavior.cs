using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class WatermelonEnemyBehavior : EnemyBase
{
    protected override void Update()
    {
        base.Update();

        MoveSpeed = 0f;
    }
}
