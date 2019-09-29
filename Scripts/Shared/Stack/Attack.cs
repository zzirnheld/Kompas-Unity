using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : IStackable
{
    public ServerGame serverGame;

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
        this.serverGame = serverGame;
        this.attacker = attacker;
        this.defender = defender;
        serverGame.Trigger(TriggerCondition.Attacks, attacker, null, this, null);
        serverGame.Trigger(TriggerCondition.Defends, defender, null, this, null);
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
        defender.E -= attackerDmg;
        attacker.E -= defenderDmg;
        //check for death
        serverGame.CheckForDeath(attacker, null, this);
        serverGame.CheckForDeath(defender, null, this);
        //notify the players that cards' nesw have changed
        serverGame.serverNotifier.NotifySetNESW(defender);
        serverGame.serverNotifier.NotifySetNESW(attacker);
        //trigger effects based on combat damage
        serverGame.Trigger(TriggerCondition.TakeCombatDamage, defender, null, this, attackerDmg);
        serverGame.Trigger(TriggerCondition.TakeCombatDamage, attacker, null, this, defenderDmg);
        serverGame.Trigger(TriggerCondition.DealCombatDamage, attacker, null, this, attackerDmg);
        serverGame.Trigger(TriggerCondition.DealCombatDamage, defender, null, this, defenderDmg);
    }
}
