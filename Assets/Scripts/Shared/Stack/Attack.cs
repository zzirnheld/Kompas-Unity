using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : IStackable
{
    public readonly Player controller;
    public readonly GameCard attacker;
    public readonly GameCard defender;

    public GameCard Source => attacker;
    public Player Controller => controller;

    /// <summary>
    /// Constructor should be called when the attack is declared
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public Attack(Player controller, GameCard attacker, GameCard defender)
    {
        controller = controller ?? throw new System.ArgumentNullException("Cannot have null controller of attack");
        this.attacker = attacker ?? throw new System.ArgumentNullException("Cannot have null attacker");
        this.defender = defender ?? throw new System.ArgumentNullException("Cannot have null defender");
    }
}
