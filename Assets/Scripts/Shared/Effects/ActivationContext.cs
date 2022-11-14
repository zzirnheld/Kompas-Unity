using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    public class ActivationContext
    {
        public readonly Game game;

        // Information about the relevant triggering situation
        /// <summary>
        /// Information about the primary card involved in the triggering event,
        /// stashed before the triggering event
        /// </summary>
        public readonly GameCardInfo mainCardInfoBefore;

        /// <summary>
        /// Information about the secondary card involved in the triggering event,
        /// stashed before the triggering event.
        /// The secondary card is often something like "the other card in the attack"
        /// </summary>
        public readonly GameCardInfo secondaryCardInfoBefore;

        /// <summary>
        /// The card that caused the triggering event.<br/>
        /// - If an event happened because of an effect, this is the effect's source.<br/>
        /// - If an event is to do with how an attack proceeds, 
        ///  like the fight starting or ending, or combat damage being dealt,
        ///  the attacker is considered to have caused the attack.<br/>
        /// - If an event happened because of an attack, 
        ///  excluding any triggers directly related to the attack itself,
        ///  this is the other card involved in the attack.
        ///  (Think a character dying during a fight. That was caused by the other card.)
        /// </summary>
        public readonly GameCardInfo cardCauseBefore;

        /// <summary>
        /// The object on the stack that caused this event to occur.
        /// For example, if an effect caused an attack to start, this would be the effect.
        /// </summary>
        public readonly IStackable stackableCause;

        /// <summary>
        /// The object on the stack that this trigger describes an event related to.
        /// For example, if this is an "Attack" event, the stackableEvent is that attack.
        /// </summary>
        public readonly IStackable stackableEvent;

        public readonly Player player;
        public readonly int? x;
        public readonly Space space;


        /// <summary>
        /// The information for the main triggering card,
        /// stashed immediately after the triggering event occurred.
        /// </summary>
        public GameCardInfo MainCardInfoAfter { get; private set; }

        /// <summary>
        /// The information for the secondary triggering card,
        /// stashed immediately after the triggering event occurred.
        /// The secondary card could be the defender in an "Attack" trigger, etc.
        /// </summary>
        public GameCardInfo SecondaryCardInfoAfter { get; private set; }

        /// <summary>
        /// The information for the card that caused the event,
        /// stashed immediately after the triggering event occurred.
        /// </summary>
        public GameCardInfo CauseCardInfoAfter { get; private set; }

        private readonly string asString;

        // Used for resuming delayed effects
        public int StartIndex { get; private set; }
        public List<GameCard> CardTargets { get; private set; }
        public GameCard DelayedCardTarget { get; private set; }
        public List<Space> SpaceTargets { get; private set; }
        public Space DelayedSpaceTarget { get; private set; }
        public List<IStackable> StackableTargets { get; private set; }
        public IStackable DelayedStackableTarget { get; private set; }

        public ActivationContext Copy
        {
            get
            {
                var copy = new ActivationContext(game: game,
                    mainCardInfoBefore: mainCardInfoBefore, 
                    secondaryCardInfoBefore: secondaryCardInfoBefore, 
                    cardCause: cardCauseBefore, 
                    stackableCause: stackableCause,
                    stackableEvent: stackableEvent,
                    player: player, 
                    x: x, 
                    space: space);
                copy.SetResumeInfo(CardTargets, SpaceTargets, StackableTargets,
                    DelayedCardTarget, DelayedSpaceTarget, DelayedStackableTarget,
                    StartIndex);
                return copy;
            }
        }

        private ActivationContext(Game game,
                                  GameCardInfo mainCardInfoBefore,
                                  GameCardInfo secondaryCardInfoBefore,
                                  GameCardInfo cardCause,
                                  IStackable stackableCause,
                                  IStackable stackableEvent,
                                  Player player,
                                  int? x,
                                  Space space)
        {
            this.game = game;
            this.mainCardInfoBefore = mainCardInfoBefore;
            this.secondaryCardInfoBefore = secondaryCardInfoBefore;
            this.cardCauseBefore = cardCause;
            this.stackableCause = stackableCause;
            this.stackableEvent = stackableEvent;
            this.player = player;
            this.x = x;
            this.space = space;

            var sb = new System.Text.StringBuilder();

            if (mainCardInfoBefore != null) sb.Append($"Card: {mainCardInfoBefore.CardName}, ");
            if (secondaryCardInfoBefore != null) sb.Append($"Secondary Card: {secondaryCardInfoBefore.CardName}, ");
            if (cardCause != null) sb.Append($"Card cause: {cardCause.CardName}, ");
            if (stackableCause != null) sb.Append($"Stackable Cause: {stackableCause}, ");
            if (player != null) sb.Append($"Triggerer: {player.index}, ");
            if (x != null) sb.Append($"X: {x}, ");
            if (space != null) sb.Append($"Space: {space}, ");
            if (CardTargets != null) sb.Append($"Targets: {string.Join(", ", CardTargets)}, ");
            if (StartIndex != 0) sb.Append($"Starting at {StartIndex}");

            asString = sb.ToString();
        }

        public ActivationContext(Game game,
                                 GameCard mainCardBefore = null,
                                 GameCard secondaryCardBefore = null,
                                 GameCard eventCauseOverride = null,
                                 IStackable stackableCause = null,
                                 IStackable stackableEvent = null,
                                 Player player = null,
                                 int? x = null,
                                 Space space = null)
            : this(game: game,
                   mainCardInfoBefore: GameCardInfo.CardInfoOf(mainCardBefore),
                   secondaryCardInfoBefore: GameCardInfo.CardInfoOf(secondaryCardBefore),
                   //Set the event cause either as the override if one is provided,
                   //or as the stackable's cause if not.
                   cardCause: GameCardInfo.CardInfoOf(eventCauseOverride ?? stackableCause?.GetCause(mainCardBefore?.Card)),
                   stackableCause: stackableCause,
                   stackableEvent: stackableEvent,
                   player: player,
                   x: x,
                   space: space?.Copy)
        { }

        /// <summary>
        /// Set any information relevant to resuming an effect's resolution
        /// </summary>
        /// <param name="startIndex">The index at which to start resolving the effect (again)</param>
        /// <param name="targets">The targets to resume with, if any</param>
        /// <param name="spaces">The spaces to resume with, if any</param>
        public void SetResumeInfo(IEnumerable<GameCard> targets, IEnumerable<Space> spaces, IEnumerable<IStackable> stackables,
            GameCard delayedCardTarget, Space delayedSpaceTarget, IStackable delayedStackableTarget,
            int? startIndex = null)
        {
            CardTargets = targets == null ? new List<GameCard>() : new List<GameCard>(targets);
            SpaceTargets = spaces == null ? new List<Space>() : new List<Space>(spaces);
            StackableTargets = stackables == null ? new List<IStackable>() : new List<IStackable>(stackables);
            DelayedCardTarget = delayedCardTarget;
            DelayedSpaceTarget = delayedSpaceTarget;
            DelayedStackableTarget = delayedStackableTarget;
            if (startIndex.HasValue) StartIndex = startIndex.Value;
        }

        /// <summary>
        /// Caches the state of the card(s) relevant to the effect immediately after the triggering event occurred
        /// </summary>
        /// <param name="mainCard">The main card whose information to cache</param>
        /// <param name="secondaryCard">The secondary card whose information to cache, if any 
        /// (like the attacker on a defends proc)</param>
        public void CacheCardInfoAfter()
        {
            if (MainCardInfoAfter != null)
                throw new System.ArgumentException("Already initialized MainCardInfoAfter on this context");
            else
                MainCardInfoAfter = GameCardInfo.CardInfoOf(mainCardInfoBefore.Card);

            if (secondaryCardInfoBefore != null)
            {
                if (SecondaryCardInfoAfter != null)
                    throw new System.ArgumentException("Already initialized SecondaryCardInfoAfter on this context");
                else
                    SecondaryCardInfoAfter = GameCardInfo.CardInfoOf(secondaryCardInfoBefore.Card);
            }

            if (cardCauseBefore != null)
            {
                if (CauseCardInfoAfter != null)
                    throw new System.ArgumentException("Already initialized CauseCardInfoAfter on this context");
                else
                    CauseCardInfoAfter = GameCardInfo.CardInfoOf(cardCauseBefore.Card);
            }
        }

        public override string ToString()
        {
            return asString;
        }
    }
}