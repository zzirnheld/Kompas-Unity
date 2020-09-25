using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.GameCore;
using KompasServer.Networking;
using System.Collections.Generic;

namespace KompasServer.Cards
{
    public class ServerGameCard : GameCard
    {
        public ServerGame ServerGame { get; private set; }
        public override Game Game => ServerGame;

        public ServerEffectsController EffectsController => ServerGame.EffectsController;
        public ServerNotifier ServerNotifier => ServerController.ServerNotifier;

        private ServerPlayer serverController;
        public ServerPlayer ServerController
        {
            get => serverController;
            set
            {
                serverController = value;
                cardCtrl.SetRotation();
                ServerNotifier.NotifyChangeController(this, ServerController);
            }
        }
        public override Player Controller
        {
            get => ServerController;
            set => ServerController = value as ServerPlayer;
        }

        public ServerPlayer ServerOwner { get; private set; }
        public override Player Owner => ServerController;

        public ServerEffect[] ServerEffects { get; private set; }
        public override IEnumerable<Effect> Effects => ServerEffects;

        public override bool IsAvatar => false;

        public override CardLocation Location
        {
            get => base.Location;
            set
            {
                if (Location == CardLocation.Hand && value != CardLocation.Hand && !KnownToEnemy)
                    ServerController.ServerEnemy.ServerNotifier.NotifyDecrementHand();
                base.Location = value;
                switch (Location)
                {
                    case CardLocation.Discard:
                    case CardLocation.Field:
                    case CardLocation.Annihilation:
                        knownToEnemy = true;
                        break;
                    case CardLocation.Deck:
                        knownToEnemy = false;
                        break;
                    default:
                        UnityEngine.Debug.Log($"Card {CardName} being moved to {Location}. " +
                            $"Not setting knownToEnemy, because enemy knowledge doesn't change");
                        break;
                }
            }
        }

        private bool knownToEnemy = false;
        public override bool KnownToEnemy => knownToEnemy;

        public virtual void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
        {
            base.SetInfo(serializedCard, id);
            ServerGame = game;
            ServerController = ServerOwner = owner;
            ServerEffects = effects;
        }

        public override void AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: augment, stackable: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
            EffectsController.TriggerForCondition(Trigger.AugmentAttached, context);
            ServerNotifier.NotifyAttach(augment, BoardX, BoardY);
            base.AddAugment(augment, stackSrc);
        }

        protected override void Detach(IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
            EffectsController.TriggerForCondition(Trigger.AugmentDetached, context);
            base.Detach(stackSrc);
        }

        public override bool Remove(IStackable stackSrc = null)
        {
            if (!CanRemove) return false;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
            EffectsController.TriggerForCondition(Trigger.Remove, context);
            base.Remove(stackSrc);
            //copy the enumeration so that you can edit the original
            var augments = new List<GameCard>(Augments);
            foreach (var aug in augments) aug.Discard();
            return true;
        }

        public override void ResetForTurn(Player turnPlayer)
        {
            base.ResetForTurn(turnPlayer);

            if (CardType == 'S' && SpellSubtype == VanishingSubtype)
            {
                if (TurnsOnBoard > Arg) Discard();
            }
        }

        #region stats
        public override void SetN(int n, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: N - n);
            EffectsController.TriggerForCondition(Trigger.NChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetN(n, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetE(int e, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: E - e);
            EffectsController.TriggerForCondition(Trigger.EChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetE(e, stackSrc);
            ServerNotifier.NotifyStats(this);

            //kill if applicable
            if (E <= 0 && CardType == 'C') Discard(stackSrc);
        }

        public override void SetS(int s, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: S - s);
            EffectsController.TriggerForCondition(Trigger.SChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetS(s, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetW(int w, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: W - w);
            EffectsController.TriggerForCondition(Trigger.WChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetW(w, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetC(int c, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: C - c);
            EffectsController.TriggerForCondition(Trigger.CChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetC(c, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetA(int a, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: A - a);
            EffectsController.TriggerForCondition(Trigger.AChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetA(a, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetNegated(bool negated, IStackable stackSrc = null)
        {
            if (Negated != negated)
            {
                ServerNotifier.NotifySetNegated(this, negated);
                var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller);
                if (negated) EffectsController.TriggerForCondition(Trigger.Negate, context);
            }
            base.SetNegated(negated, stackSrc);
        }

        public override void SetActivated(bool activated, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller);
            if (Activated != activated)
            {
                ServerNotifier.NotifyActivate(this, activated);
                if (activated) EffectsController.TriggerForCondition(Trigger.Activate, context);
                else EffectsController.TriggerForCondition(Trigger.Deactivate, context);
            }
            base.SetActivated(activated, stackSrc);
        }
        #endregion stats
    }
}