using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.GameCore;
using KompasServer.Networking;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                foreach (var eff in Effects.Where(e => e != null)) eff.Controller = value;
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
                    /* default:
                        Debug.Log($"Card {CardName} being moved to {Location}. " +
                            $"Not setting knownToEnemy, because enemy knowledge doesn't change");
                        break; */
                }
            }
        }

        private bool knownToEnemy = false;
        public override bool KnownToEnemy => knownToEnemy;

        public virtual void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
        {
            base.SetInfo(serializedCard, id);
            ServerEffects = effects;
            ServerGame = game;
            ServerController = ServerOwner = owner;
        }

        public override void ResetCard()
        {
            //notify first so reset values get set to their proper things
            ServerNotifier.NotifyResetCard(this);
            base.ResetCard();
        }

        public override bool AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            if (augment == null) return false;

            var context = new ActivationContext(card: augment, space: Position, stackable: stackSrc, triggerer: Controller);
            EffectsController.TriggerForCondition(Trigger.AugmentAttached, context);

            var augmentedContext = new ActivationContext(card: this, space: Position, stackable: stackSrc, triggerer: Controller);
            EffectsController.TriggerForCondition(Trigger.Augmented, augmentedContext);

            ServerGame.ServerPlayers[augment.ControllerIndex].ServerNotifier.NotifyAttach(augment, BoardX, BoardY);
            return base.AddAugment(augment, stackSrc);
        }

        protected override bool Detach(IStackable stackSrc = null)
        {
            if (AugmentedCard == null) return false;

            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
            EffectsController.TriggerForCondition(Trigger.AugmentDetached, context);
            return base.Detach(stackSrc);
        }

        public override bool Remove(IStackable stackSrc = null)
        {
            //if you can't remove, don't even proc the trigger
            if (!CanRemove) return false;

            //proc the trigger before actually removing anything
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller ?? Controller);
            EffectsController.TriggerForCondition(Trigger.Remove, context);

            //remove for realsies
            base.Remove(stackSrc);

            //copy the colleciton  so that you can edit the original
            var augments = Augments.ToArray();
            foreach (var aug in augments) aug.Discard();
            return true;
        }

        #region stats
        public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
        {
            if (CardType != 'C') return;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: n - N);
            EffectsController.TriggerForCondition(Trigger.NChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetN(n, stackSrc);

            if(notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetE(int e, IStackable stackSrc = null, bool notify = true)
        {
            if (CardType != 'C') return;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: e - E);
            EffectsController.TriggerForCondition(Trigger.EChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetE(e, stackSrc);

            if (notify) ServerNotifier.NotifyStats(this);

            //kill if applicable
            if (E <= 0 && CardType == 'C') Discard(stackSrc);
        }

        public override void SetS(int s, IStackable stackSrc = null, bool notify = true)
        {
            if (CardType != 'C') return;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: s - S);
            EffectsController.TriggerForCondition(Trigger.SChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetS(s, stackSrc);

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetW(int w, IStackable stackSrc = null, bool notify = true)
        {
            if (CardType != 'C') return;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: w - W);
            EffectsController.TriggerForCondition(Trigger.WChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetW(w, stackSrc);

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetC(int c, IStackable stackSrc = null, bool notify = true)
        {
            if (CardType != 'S') return;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: c - C);
            EffectsController.TriggerForCondition(Trigger.CChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetC(c, stackSrc);

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetA(int a, IStackable stackSrc = null, bool notify = true)
        {
            if (CardType != 'A') return;
            var context = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, x: a - A);
            EffectsController.TriggerForCondition(Trigger.AChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            base.SetA(a, stackSrc);

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
        {
            base.SetCharStats(n, e, s, w, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetStats((int n, int e, int s, int w, int c, int a) stats, IStackable stackSrc = null)
        {
            base.SetStats(stats, stackSrc);
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

        public override void SetSpacesMoved(int spacesMoved, bool fromReset = false)
        {
            base.SetSpacesMoved(spacesMoved, fromReset);
            if (ServerController != null && !fromReset) ServerController.ServerNotifier.NotifySpacesMoved(this);
        }

        public override void SetAttacksThisTurn(int attacksThisTurn, bool fromReset = false)
        {
            base.SetAttacksThisTurn(attacksThisTurn, fromReset);
            if (ServerController != null && !fromReset) ServerController.ServerNotifier.NotifyAttacksThisTurn(this);
            // Debug.Log($"Setting attacks this turn for {CardName} to {attacksThisTurn}");
        }
        #endregion stats
    }
}