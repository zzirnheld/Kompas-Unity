using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IStackable
{
    public ServerGame serverGame;

    public Card Source { get { return attacker; } }

    public CharacterCard attacker;
    public CharacterCard defender;

    /// <summary>
    /// Constructor should be called when the attack is declared
    /// </summary>
    /// <param name="serverGame"></param>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public Attack(ServerGame serverGame, CharacterCard attacker, CharacterCard defender)
    {
        this.serverGame = serverGame ?? throw new System.ArgumentNullException("Cannot have null servergame");
        this.attacker = attacker ?? throw new System.ArgumentNullException("Cannot have null attacker");
        this.defender = defender ?? throw new System.ArgumentNullException("Cannot have null defender");
        serverGame.Trigger(TriggerCondition.Attacks, attacker, this, null);
        serverGame.Trigger(TriggerCondition.Defends, defender, this, null);
        serverGame.Trigger(TriggerCondition.Battles, attacker, this, null);
        serverGame.Trigger(TriggerCondition.Battles, defender, this, null);
    }

    public void StartResolution()
    {
        //deal the damage
        DealDamage();
        //then finish the resolution
        serverGame.FinishStackEntryResolution();
    }
    
    private void DealDamage()
    {
        //get damage from both, before either takes any damage, in case effects matter on hp
        int attackerDmg = attacker.W;
        int defenderDmg = defender.W;
        //deal the damage
        serverGame.SetStats(defender,
            defender.N,
            defender.E - attackerDmg,
            defender.S,
            defender.W);
        serverGame.SetStats(attacker,
            attacker.N,
            attacker.E - defenderDmg,
            attacker.S,
            attacker.W);
        //trigger effects based on combat damage
        serverGame.Trigger(TriggerCondition.TakeCombatDamage, defender, this, attackerDmg);
        serverGame.Trigger(TriggerCondition.TakeCombatDamage, attacker, this, defenderDmg);
        serverGame.Trigger(TriggerCondition.DealCombatDamage, attacker, this, attackerDmg);
        serverGame.Trigger(TriggerCondition.DealCombatDamage, defender, this, defenderDmg);
    }
}
