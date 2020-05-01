using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IStackable
{
    public Card Source { get { return attacker; } }
    public Player Controller { get; private set; }

    public CharacterCard attacker;
    public CharacterCard defender;

    /// <summary>
    /// Constructor should be called when the attack is declared
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public Attack(Player controller, CharacterCard attacker, CharacterCard defender)
    {
        this.Controller = controller ?? throw new System.ArgumentNullException("Cannot have null controller of attack");
        this.attacker = attacker ?? throw new System.ArgumentNullException("Cannot have null attacker");
        this.defender = defender ?? throw new System.ArgumentNullException("Cannot have null defender");
    }
}
