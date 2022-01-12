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
        public override Player Owner
        {
            get => ServerOwner;
            protected set => ServerOwner = value as ServerPlayer;
        }

        public ServerEffect[] ServerEffects { get; private set; }
        public override IEnumerable<Effect> Effects => ServerEffects;

        public override bool IsAvatar
        {
            get => false;
            protected set
            {
                throw new System.NotImplementedException($"Tried to set isAvatar actual GameCard {this}");
            }
        }

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
                    case CardLocation.Field:
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
                if(old != value) ServerNotifier.NotifyKnownToEnemy(this, old);
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

        public virtual void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
        {
            base.SetInfo(serializedCard, id);
            ServerEffects = effects;
            int i = 0;
            foreach (var eff in effects) eff.SetInfo(this, game, owner, i++);
            Debug.Log($"Setting card with effects: {string.Join(", ", effects.Select(e => e.ToString()))}");
            ServerGame = game;
            ServerController = ServerOwner = owner;
        }

        public override void ResetCard()
        {
            //notify first so reset values get set to their proper things
            ServerNotifier.NotifyResetCard(this);
            base.ResetCard();
        }

        public override void Vanish()
        {
            ActivationContext context = new ActivationContext(mainCardBefore: this);
            base.Vanish();
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.Vanish, context);
        }

        public override void AddAugment(GameCard augment, IStackable stackSrc = null)
        {
            var attachedContext = new ActivationContext(mainCardBefore: augment, secondaryCardBefore: this, 
                space: Position, stackable: stackSrc, player: Controller);
            var augmentedContext = new ActivationContext(mainCardBefore: this, secondaryCardBefore: augment,
                space: Position, stackable: stackSrc, player: Controller);
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
            var context = new ActivationContext(mainCardBefore: this, secondaryCardBefore: formerlyAugmentedCard,
                stackable: stackSrc, player: stackSrc?.Controller ?? Controller);
            base.Detach(stackSrc);
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.AugmentDetached, context);
        }

        public override void Remove(IStackable stackSrc = null)
        {
            Debug.Log($"Trying to remove {CardName} from {Location}");

            //proc the trigger before actually removing anything
            var player = stackSrc?.Controller ?? Controller;
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: player);

            var cardsThisLeft = Location == CardLocation.Field ?
                Game.boardCtrl.CardsAndAugsWhere(c => c.CardInAOE(this)).ToList() :
                new List<GameCard>();
            var leaveContexts = cardsThisLeft.Select(c => 
                new ActivationContext(mainCardBefore: this, secondaryCardBefore: c, stackable: stackSrc, player: player));

            base.Remove(stackSrc);

            context.CacheCardInfoAfter();
            foreach(var lc in leaveContexts)
            {
                lc.CacheCardInfoAfter();
            }
            EffectsController.TriggerForCondition(Trigger.Remove, context);
            EffectsController.TriggerForCondition(Trigger.LeaveAOE, leaveContexts.ToArray());
            //copy the colleciton  so that you can edit the original
            var augments = Augments.ToArray();
            foreach (var aug in augments) aug.Discard(stackSrc);
        }

        public override void Reveal(IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller);
            base.Reveal(stackSrc);
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.Revealed, context);
            //logic for actually revealing to client has to happen server-side.
            KnownToEnemy = true;
            ServerController.ServerEnemy.ServerNotifier.NotifyRevealCard(this);
        }

        #region stats
        public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: n - N);
            EffectsController.TriggerForCondition(Trigger.NChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            var setContext = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: n);
            EffectsController.TriggerForCondition(Trigger.NSet, setContext);
            base.SetN(n, stackSrc);
            context.CacheCardInfoAfter();
            setContext.CacheCardInfoAfter();

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetE(int e, IStackable stackSrc = null, bool notify = true)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: e - E);
            EffectsController.TriggerForCondition(Trigger.EChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            var setContext = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: e);
            EffectsController.TriggerForCondition(Trigger.ESet, setContext);
            base.SetE(e, stackSrc);
            context.CacheCardInfoAfter();
            setContext.CacheCardInfoAfter();

            if (notify) ServerNotifier.NotifyStats(this);

            //kill if applicable
            DieIfApplicable(stackSrc);
        }
        public void DieIfApplicable(IStackable stackSrc)
        {
            if (E <= 0 && CardType == 'C' && Summoned) Discard(stackSrc);
        }

        public override void SetS(int s, IStackable stackSrc = null, bool notify = true)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: s - S);
            EffectsController.TriggerForCondition(Trigger.SChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            var setContext = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: s);
            EffectsController.TriggerForCondition(Trigger.SSet, setContext);
            base.SetS(s, stackSrc);
            context.CacheCardInfoAfter();
            setContext.CacheCardInfoAfter();

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetW(int w, IStackable stackSrc = null, bool notify = true)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: w - W);
            EffectsController.TriggerForCondition(Trigger.WChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            var setContext = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: w);
            EffectsController.TriggerForCondition(Trigger.WSet, setContext);
            base.SetW(w, stackSrc);
            context.CacheCardInfoAfter();
            setContext.CacheCardInfoAfter();

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetC(int c, IStackable stackSrc = null, bool notify = true)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: c - C);
            EffectsController.TriggerForCondition(Trigger.CChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            var setContext = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: c);
            EffectsController.TriggerForCondition(Trigger.CSet, setContext);
            base.SetC(c, stackSrc);
            context.CacheCardInfoAfter();
            setContext.CacheCardInfoAfter();

            if (notify) ServerNotifier.NotifyStats(this);
        }

        public override void SetA(int a, IStackable stackSrc = null, bool notify = true)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: a - A);
            EffectsController.TriggerForCondition(Trigger.AChange, context);
            EffectsController.TriggerForCondition(Trigger.NESWChange, context);
            var setContext = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller, x: a);
            EffectsController.TriggerForCondition(Trigger.ASet, setContext);
            base.SetA(a, stackSrc);
            context.CacheCardInfoAfter();
            setContext.CacheCardInfoAfter();

            if (notify) ServerNotifier.NotifyStats(this);
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
                var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller);
                context.CacheCardInfoAfter();
                if (negated) EffectsController.TriggerForCondition(Trigger.Negate, context);
            }
            base.SetNegated(negated, stackSrc);
        }

        public override void SetActivated(bool activated, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: this, stackable: stackSrc, player: stackSrc?.Controller);
            if (Activated != activated)
            {
                ServerNotifier.NotifyActivate(this, activated);
                context.CacheCardInfoAfter();
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