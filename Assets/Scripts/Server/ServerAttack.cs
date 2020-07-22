using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;

namespace KompasServer.Effects
{
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
            var attackerContext = new ActivationContext(card: attacker, stackable: this, triggerer: Controller);
            var defenderContext = new ActivationContext(card: defender, stackable: this, triggerer: Controller);
            EffCtrl.TriggerForCondition(Trigger.Attacks, attackerContext);
            EffCtrl.TriggerForCondition(Trigger.Defends, defenderContext);
            EffCtrl.TriggerForCondition(Trigger.Battles, attackerContext, defenderContext);
        }

        private bool StillValidAttack
        {
            get
            {
                return attacker.Location == CardLocation.Field &&
                    defender.Location == CardLocation.Field;
            }
        }

        public void StartResolution(ActivationContext context)
        {
            //deal the damage
            if (StillValidAttack) DealDamage();
            var attackerContext = new ActivationContext(card: attacker, stackable: this, triggerer: Controller);
            var defenderContext = new ActivationContext(card: defender, stackable: this, triggerer: Controller);
            EffCtrl.TriggerForCondition(Trigger.BattleEnds, attackerContext, defenderContext);
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
            var attackerDealContext = new ActivationContext(card: attacker, stackable: this, triggerer: Controller, x: attackerDmg);
            var defenderDealContext = new ActivationContext(card: defender, stackable: this, triggerer: Controller, x: defenderDmg);
            var attackerTakeContext = new ActivationContext(card: attacker, stackable: this, triggerer: Controller, x: defenderDmg);
            var defenderTakeContext = new ActivationContext(card: defender, stackable: this, triggerer: Controller, x: attackerDmg);
            //trigger effects based on combat damage
            EffCtrl.TriggerForCondition(Trigger.TakeCombatDamage, attackerTakeContext, defenderTakeContext);
            EffCtrl.TriggerForCondition(Trigger.DealCombatDamage, attackerDealContext, defenderDealContext);
        }
    }
}