using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayer : Player
{
    public ClientPlayer ClientEnemy;

    public override Player Enemy => ClientEnemy;
}
