using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.GameCore;
using KompasServer.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KompasServer.Cards
{
    public class ServerGameCard : GameCard
    {
        public ServerGame ServerGame { get; private set; }
        public override Game Game => ServerGame;

        public ServerEffectsController EffectsController => ServerGame?.EffectsController;
        public ServerNotifier ServerNotifier => ServerController?.ServerNotifier;

        private ServerPlayer serverController;
        public ServerPlayer ServerController
        {
            get => serverController;
            set
            {
                serverController = value;
                cardCtrl.SetRotation();
                ServerNotifier.NotifyChangeController(this, ServerController);
                foreach (var eff in Effects) eff.Controller = value;
            }
        }
        public override Player Controller
        {
            get => ServerController;
            set => ServerController = value as ServerPlayer;
        }

        public ServerPlayer ServerOwner { get; private set; }
        public override Player Owner
        {
            get => ServerOwner;
            protected set => ServerOwner = value as ServerPlayer;
        }

        public ServerEffect[] ServerEffects { get; private set; }
        public override IEnumerable<Effect> Effects => ServerEffects;

        public override bool IsAvatar => false;

        public override CardLocation Location
        {
            get => base.Location;
            protected set
            {
                if (Location == CardLocation.Hand && value != CardLocation.Hand && !KnownToEnemy)
                    ServerController.ServerEnemy.ServerNotifier.NotifyDecrementHand();

                if (Location != value) ResetCard();

                base.Location = value;
                switch (Location)
                {
                    case CardLocation.Discard:
                    case CardLocation.Board:
                    case CardLocation.Annihilation:
                        KnownToEnemy = true;
                        break;
                    case CardLocation.Deck:
                        KnownToEnemy = false;
                        break;
                        //Otherwise, KnownToEnemy doesn't change, if it's been added to the hand
                        //discard->rehand is public, but deck->rehand is private, for example
                }
            }
        }

        private bool knownToEnemy = false;
        public override bool KnownToEnemy
        {
            get => knownToEnemy;
            set
            {
                bool old = knownToEnemy;
                knownToEnemy = value;
                //update clients if changed
                if (old != value) ServerNotifier.NotifyKnownToEnemy(this, old);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            foreach (var eff in Effects)
            {
                sb.Append(eff.ToString());
                sb.Append(", ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Resets any of the card's values that might be different from their originals.
        /// Should be called when cards move out the discard, or into the hand, deck, or annihilation
        /// </summary>
        public void ResetCard()
        {
            if (InitialCardValues == null)
            {
                Debug.Log("Tried to reset card whose info was never set! This should only be the case at game start");
                return;
            }

            SetCardInfo(InitialCardValues, ID);

            SetTurnsOnBoard(0);
            SetSpacesMoved(0);
            SetAttacksThisTurn(0);

            if (Effects != null) foreach (var eff in Effects) eff.Reset();
            //instead of setting negations or activations to 0, so that it updates the client correctly
            while (Negated) SetNegated(false);
            while (Activated) SetActivated(false);
        }

        public void SetInitialCardInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
        {
            SetCardInfo(serializedCard, id);
            ServerEffects = effects;
            int i = 0;
            foreach (var eff in effects) eff.SetInfo(this, game, owner, i++);
            ServerGame = game;
            ServerOwner = owner;
            ServerController = owner;
        }

        public override void Vanish()
        {
            ActivationContext context = new ActivationContext(game: ServerGame, mainCardBefore: this);
            base.Vanish();
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.Vanish, context);
        }

        public override void AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            var attachedContext = new ActivationContext(game: ServerGame, mainCardBefore: augment, secondaryCardBefore: this,
                space: Position, stackableCause: stackSrc, player: stackSrc?.Controller ?? Controller);
            var augmentedContext = new ActivationContext(game: ServerGame, mainCardBefore: this, secondaryCardBefore: augment,
                space: Position, stackableCause: stackSrc, player: stackSrc?.Controller ?? Controller);
            bool wasKnown = augment.KnownToEnemy;
            base.AddAugment(augment, stackSrc);
            attachedContext.CacheCardInfoAfter();
            augmentedContext.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.AugmentAttached, attachedContext);
            EffectsController.TriggerForCondition(Trigger.Augmented, augmentedContext);
            ServerGame.ServerPlayers[augment.ControllerIndex].ServerNotifier.NotifyAttach(augment, Position, wasKnown);
        }

        protected override void Detach(IStackable stackSrc = null)
        {
            var formerlyAugmentedCard = AugmentedCard;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, secondaryCardBefore: formerlyAugmentedCard,
                stackableCause: stackSrc, player: stackSrc?.Controller ?? Controller);
            base.Detach(stackSrc);
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.AugmentDetached, context);
        }

        public override bool Remove(IStackable stackSrc = null)
        {
            //Debug.Log($"Trying to remove {CardName} from {Location}");

            //proc the trigger before actually removing anything
            var player = stackSrc?.Controller ?? Controller;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: player);

            var cardsThisLeft = Location == CardLocation.Board ?
                Game.boardCtrl.CardsAndAugsWhere(c => c != null && c.IsCardInMyAOE(this)).ToList() :
                new List<GameCard>();
            var leaveContexts = cardsThisLeft.Select(c =>
                new ActivationContext(game: ServerGame, mainCardBefore: this, secondaryCardBefore: c, stackableCause: stackSrc, player: player));

            var ret = base.Remove(stackSrc);

            context.CacheCardInfoAfter();
            foreach (var lc in leaveContexts)
            {
                lc.CacheCardInfoAfter();
            }
            EffectsController.TriggerForCondition(Trigger.Remove, context);
            EffectsController.TriggerForCondition(Trigger.LeaveAOE, leaveContexts.ToArray());
            //copy the colleciton  so that you can edit the original
            var augments = Augments.ToArray();
            foreach (var aug in augments) aug.Discard(stackSrc);
            return ret;
        }

        public override void Reveal(IStackable stackSrc = null)
        {
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller);
            base.Reveal(stackSrc);
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.Revealed, context);
            //logic for actually revealing to client has to happen server-side.
            KnownToEnemy = true;
            ServerController.ServerEnemy.ServerNotifier.NotifyRevealCard(this);
        }

        #region stats
        public override void SetN(int n, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            if (n == N) return;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: n - N);
            base.SetN(n, stackSrc);
            context.CacheCardInfoAfter();
            EffectsController?.TriggerForCondition(Trigger.NChange, context);

            if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
        }

        public override void SetE(int e, IStackable stackSrc = null, bool onlyStatBeingSet = true)
        {
            if (e == E) return;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: e - E);
            base.SetE(e, stackSrc);
            context.CacheCardInfoAfter();
            EffectsController?.TriggerForCondition(Trigger.EChange, context);

            if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);

            //kill if applicable
            DieIfApplicable(stackSrc);
        }
        public void DieIfApplicable(IStackable stackSrc)
        {
            if (E <= 0 && CardType == 'C' && Summoned) Discard(stackSrc);
        }

        public override void SetS(int s, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            if (s == S) return;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: s - S);
            base.SetS(s, stackSrc);
            context.CacheCardInfoAfter();
            EffectsController?.TriggerForCondition(Trigger.SChange, context);

            if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
        }

        public override void SetW(int w, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            if (w == W) return;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: w - W);
            base.SetW(w, stackSrc);
            context.CacheCardInfoAfter();
            EffectsController?.TriggerForCondition(Trigger.WChange, context);

            if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
        }

        public override void SetC(int c, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            if (c == C) return;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: c - C);
            base.SetC(c, stackSrc);
            context.CacheCardInfoAfter();
            EffectsController?.TriggerForCondition(Trigger.CChange, context);

            if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
        }

        public override void SetA(int a, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            if (a == A) return;
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: a - A);
            base.SetA(a, stackSrc);
            context.CacheCardInfoAfter();
            EffectsController?.TriggerForCondition(Trigger.AChange, context);

            if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
        }

        public override void TakeDamage(int dmg, IStackable stackSrc = null)
        {
            int netDmg = dmg;
            base.TakeDamage(netDmg, stackSrc);
        }

        public override void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
        {
            base.SetCharStats(n, e, s, w, stackSrc);
            ServerNotifier.NotifyStats(this);
        }

        public override void SetStats(CardStats stats, IStackable stackSrc = null)
        {
            base.SetStats(stats, stackSrc);
            ServerNotifier?.NotifyStats(this);
        }

        public override void SetNegated(bool negated, IStackable stackSrc = null)
        {
            if (Negated != negated)
            {
                //Notify of value being set to, even if it won't actually change whether the card is negated or not
                //so that the client can know how many negations a card has
                ServerNotifier.NotifySetNegated(this, negated);

                var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller);
                context.CacheCardInfoAfter();
                if (negated) EffectsController.TriggerForCondition(Trigger.Negate, context);
            }
            base.SetNegated(negated, stackSrc);
        }

        public override void SetActivated(bool activated, IStackable stackSrc = null)
        {
            var context = new ActivationContext(game: ServerGame, mainCardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller);
            if (Activated != activated)
            {
                //Notify of value being set to, even if it won't actually change whether the card is activated or not,
                //so that the client can know how many activations a card has
                ServerNotifier.NotifyActivate(this, activated);

                context.CacheCardInfoAfter();
                if (activated) EffectsController.TriggerForCondition(Trigger.Activate, context);
                else EffectsController.TriggerForCondition(Trigger.Deactivate, context);
            }
            base.SetActivated(activated, stackSrc);
        }

        public override void SetSpacesMoved(int spacesMoved)
        {
            bool changed = SpacesMoved != spacesMoved;
            base.SetSpacesMoved(spacesMoved);
            if (changed) ServerNotifier?.NotifySpacesMoved(this);
        }

        public override void SetAttacksThisTurn(int attacksThisTurn)
        {
            bool changed = AttacksThisTurn != attacksThisTurn;
            base.SetAttacksThisTurn(attacksThisTurn);
            if (changed) ServerNotifier?.NotifyAttacksThisTurn(this);
        }
        #endregion stats
    }
}