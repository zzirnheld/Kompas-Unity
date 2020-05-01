using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAttack : Attack, IServerStackable
{
    public ServerGame serverGame;

    public ServerPlayer ServerController { get; }

    public ServerAttack(ServerGame serverGame, ServerPlayer controller, CharacterCard attacker, CharacterCard defender) 
        : base(controller, attacker, defender)
    {
        this.serverGame = serverGame ?? throw new System.ArgumentNullException("Server game cannot be null for attack");
        this.ServerController = controller ?? throw new System.ArgumentNullException("Attack must have a non-null controller");
        serverGame.Trigger(TriggerCondition.Attacks, attacker, this, null, controller);
        serverGame.Trigger(TriggerCondition.Defends, defender, this, null, controller);
        serverGame.Trigger(TriggerCondition.Battles, attacker, this, null, controller);
        serverGame.Trigger(TriggerCondition.Battles, defender, this, null, controller);
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
        serverGame.Trigger(TriggerCondition.TakeCombatDamage, defender, this, attackerDmg, ServerController);
        serverGame.Trigger(TriggerCondition.TakeCombatDamage, attacker, this, defenderDmg, ServerController);
        serverGame.Trigger(TriggerCondition.DealCombatDamage, attacker, this, attackerDmg, ServerController);
        serverGame.Trigger(TriggerCondition.DealCombatDamage, defender, this, defenderDmg, ServerController);
    }
}
