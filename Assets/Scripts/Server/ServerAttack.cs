using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAttack : Attack, IServerStackable
{
    public ServerGame ServerGame { get; }

    public ServerPlayer ServerController { get; }

    private ServerEffectsController EffCtrl => ServerGame.EffectsController;

    public ServerAttack(ServerGame serverGame, ServerPlayer controller, GameCard attacker, GameCard defender) 
        : base(controller, attacker, defender)
    {
        this.ServerGame = serverGame ?? throw new System.ArgumentNullException("Server game cannot be null for attack");
        this.ServerController = controller ?? throw new System.ArgumentNullException("Attack must have a non-null controller");
    }

    /// <summary>
    /// Trigger the triggers related to attack declaration.
    /// Should be called before the attack is resolved.
    /// </summary>
    public void Declare()
    {
        EffCtrl.Trigger(TriggerCondition.Attacks, cardTriggerer: attacker, stackTrigger: this, triggerer: ServerController);
        EffCtrl.Trigger(TriggerCondition.Defends, cardTriggerer: defender, stackTrigger: this, triggerer: ServerController);
        EffCtrl.Trigger(TriggerCondition.Battles, cardTriggerer: attacker, stackTrigger: this, triggerer: ServerController);
        EffCtrl.Trigger(TriggerCondition.Battles, cardTriggerer: defender, stackTrigger: this, triggerer: ServerController);
    }

    private bool StillValidAttack
    {
        get
        {
            return attacker.Location == CardLocation.Field &&
                defender.Location == CardLocation.Field;
        }
    }

    public void StartResolution(int startIndex = 0)
    {
        //deal the damage
        if(StillValidAttack) DealDamage();
        //then finish the resolution
        EffCtrl.FinishStackEntryResolution();
    }

    private void DealDamage()
    {
        //get damage from both, before either takes any damage, in case effects matter on hp
        int attackerDmg = attacker.CombatDamage;
        int defenderDmg = defender.CombatDamage;
        //deal the damage
        defender.SetE(defender.E - attackerDmg);
        attacker.SetE(attacker.E - defenderDmg);
        //trigger effects based on combat damage
        EffCtrl.Trigger(TriggerCondition.TakeCombatDamage, cardTriggerer: defender, stackTrigger: this, triggerer: ServerController, x: attackerDmg);
        EffCtrl.Trigger(TriggerCondition.TakeCombatDamage, cardTriggerer: attacker, stackTrigger: this, triggerer: ServerController, x: defenderDmg);
        EffCtrl.Trigger(TriggerCondition.DealCombatDamage, cardTriggerer: attacker, stackTrigger: this, triggerer: ServerController, x: attackerDmg);
        EffCtrl.Trigger(TriggerCondition.DealCombatDamage, cardTriggerer: defender, stackTrigger: this, triggerer: ServerController, x: defenderDmg);
    }
}
