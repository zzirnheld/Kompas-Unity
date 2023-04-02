using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
    public interface IResolutionContext
    {
        public static IResolutionContext Dummy(TriggeringEventContext triggeringEventContext)
            => new DummyResolutionContext(triggeringEventContext);

        /// <summary>
        /// Information describing the event that triggered this effect to occur, if any such event happened. (If it's player-triggered, this is null.) 
        /// </summary>
        public TriggeringEventContext TriggerContext { get; }

        public int StartIndex { get; }
        public List<GameCard> CardTargets { get; }
        public GameCard DelayedCardTarget { get; }
        public List<Space> SpaceTargets { get; }
        public Space DelayedSpaceTarget { get; }
        public List<IStackable> StackableTargets { get; }
        public IStackable DelayedStackableTarget { get; }


        /// <summary>
        /// Used for places that need a resolution context (like triggers calling any other identity), but to enforce never having 
        /// </summary>
        private class DummyResolutionContext : IResolutionContext
        {
            private const string NotImplementedMessage = "Dummy resolution context should never have resolution information checked. Use the secondary (aka stashed) resolution context instead.";
            public TriggeringEventContext TriggerContext { get; }

            public int StartIndex => throw new System.NotImplementedException(NotImplementedMessage);
            public List<GameCard> CardTargets => throw new System.NotImplementedException(NotImplementedMessage);
            public GameCard DelayedCardTarget => throw new System.NotImplementedException(NotImplementedMessage);
            public List<Space> SpaceTargets => throw new System.NotImplementedException(NotImplementedMessage);
            public Space DelayedSpaceTarget => throw new System.NotImplementedException(NotImplementedMessage);
            public List<IStackable> StackableTargets => throw new System.NotImplementedException(NotImplementedMessage);
            public IStackable DelayedStackableTarget => throw new System.NotImplementedException(NotImplementedMessage);

            public DummyResolutionContext(TriggeringEventContext triggerContext)
            {
                TriggerContext = triggerContext;
            }
        }
    }

    public class ResolutionContext : IResolutionContext
    {
        public TriggeringEventContext TriggerContext { get; }

        // Used for resuming delayed effects
        public int StartIndex { get; }
        public List<GameCard> CardTargets { get; }
        public GameCard DelayedCardTarget { get; }
        public List<Space> SpaceTargets { get; }
        public Space DelayedSpaceTarget { get; }
        public List<IStackable> StackableTargets { get; }
        public IStackable DelayedStackableTarget { get; }

        public int X { get; set; }

        public static ResolutionContext PlayerTrigger => new ResolutionContext(default);

        public ResolutionContext(TriggeringEventContext triggerContext)
        : this(triggerContext, 0,
            Enumerable.Empty<GameCard>(), default,
            Enumerable.Empty<Space>(), default,
            Enumerable.Empty<IStackable>(), default)
        { }

        public ResolutionContext(TriggeringEventContext triggerContext,
            int startIndex,
            IEnumerable<GameCard> cardTargets, GameCard delayedCardTarget,
            IEnumerable<Space> spaceTargets, Space delayedSpaceTarget,
            IEnumerable<IStackable> stackableTargets, IStackable delayedStackableTarget)
        {
            TriggerContext = triggerContext;
            StartIndex = startIndex;

            CardTargets = Clone(cardTargets);
            DelayedCardTarget = delayedCardTarget;

            SpaceTargets = Clone(spaceTargets);
            DelayedSpaceTarget = delayedSpaceTarget;

            StackableTargets = Clone(stackableTargets);
            DelayedStackableTarget = delayedStackableTarget;

            X = TriggerContext.x ?? 0;
        }

        private List<T> Clone<T>(IEnumerable<T> list)
        {
            if (list == null) return new List<T>();
            else return new List<T>(list);
        }

        public ResolutionContext Copy => new ResolutionContext(TriggerContext, StartIndex,
            CardTargets, DelayedCardTarget,
            SpaceTargets, DelayedSpaceTarget,
            StackableTargets, DelayedStackableTarget);

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(base.ToString());
            sb.Append(TriggerContext?.ToString());

            if (CardTargets != null) sb.Append($"Targets: {string.Join(", ", CardTargets)}, ");
            if (StartIndex != 0) sb.Append($"Starting at {StartIndex}");

            return sb.ToString();
        }
    }

    public class TriggeringEventContext
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

        private readonly string cachedToString;

        private TriggeringEventContext(Game game,
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
            if (stackableEvent != null) sb.Append($"Stackable Event: {stackableEvent}, ");
            if (stackableCause != null) sb.Append($"Stackable Cause: {stackableCause}, ");
            if (player != null) sb.Append($"Triggerer: {player.index}, ");
            if (x != null) sb.Append($"X: {x}, ");
            if (space != null) sb.Append($"Space: {space}, ");

            cachedToString = sb.ToString();
        }

        public TriggeringEventContext(Game game,
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
                   cardCause: GameCardInfo.CardInfoOf(eventCauseOverride ?? stackableCause?.GetCause(mainCardBefore)),
                   stackableCause: stackableCause,
                   stackableEvent: stackableEvent,
                   player: player,
                   x: x,
                   space: space?.Copy)
        { }

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

        public override string ToString() => cachedToString;
    }
}