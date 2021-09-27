using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ServerAttack : Attack, IServerStackable
    {
        public ServerGame ServerGame { get; }

        public ServerPlayer ServerController { get; }

        private ServerEffectsController EffCtrl => ServerGame.EffectsController;
        private readonly Space attackerInitialSpace;
        private readonly Space defenderInitialSpace;

        public ServerAttack(ServerGame serverGame, ServerPlayer controller, GameCard attacker, GameCard defender)
            : base(controller, attacker, defender)
        {
            this.ServerGame = serverGame != null ? serverGame : throw new System.ArgumentNullException("serverGame", "Server game cannot be null for attack");
            this.ServerController = controller != null ? controller : throw new System.ArgumentNullException("controller", "Attack must have a non-null controller");
            attackerInitialSpace = attacker.Position;
            defenderInitialSpace = defender.Position;
        }

        /// <summary>
        /// Trigger the triggers related to attack declaration.
        /// Should be called before the attack is resolved.
        /// </summary>
        public void Declare()
        {
            ServerController.ServerNotifier.NotifyAttackStarted(attacker, defender, controller);

            var attackerContext = new ActivationContext(card: attacker, stackable: this, triggerer: Controller);
            var defenderContext = new ActivationContext(card: defender, stackable: this, triggerer: Controller);
            EffCtrl.TriggerForCondition(Trigger.Attacks, attackerContext);
            EffCtrl.TriggerForCondition(Trigger.Defends, defenderContext);
            EffCtrl.TriggerForCondition(Trigger.Battles, attackerContext, defenderContext);
        }

        //this is factored out so i can maybe eventually add some indication of whether an attack is still gonna be valid
        private bool StillValidAttack
        {
            get
            {
                return attacker.Location == CardLocation.Field 
                    && defender.Location == CardLocation.Field
                    && attacker.Position == attackerInitialSpace
                    && defender.Position == defenderInitialSpace;
            }
        }

        public Task StartResolution(ActivationContext context)
        {
            //deal the damage
            if (StillValidAttack) DealDamage();
            var attackerContext = new ActivationContext(card: attacker, stackable: this, triggerer: Controller);
            var defenderContext = new ActivationContext(card: defender, stackable: this, triggerer: Controller);
            EffCtrl.TriggerForCondition(Trigger.BattleEnds, attackerContext, defenderContext);
            //then finish the resolution by just returning that completed the task. (don't need to call anything)
            return Task.CompletedTask;
        }

        private void DealDamage()
        {
            //get damage from both, before either takes any damage, in case effects matter on hp
            int attackerDmg = attacker.CombatDamage;
            int defenderDmg = defender.CombatDamage;
            //deal the damage
            defender.TakeDamage(attackerDmg, stackSrc: this);
            attacker.TakeDamage(defenderDmg, stackSrc: this);
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