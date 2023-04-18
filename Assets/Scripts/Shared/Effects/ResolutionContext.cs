using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
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

        public static ResolutionContext PlayerTrigger(Effect effect, Game game)
            => new ResolutionContext(new TriggeringEventContext(game: game, stackableEvent: effect));

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

            X = TriggerContext?.x ?? 0;
        }

        private List<T> Clone<T>(IEnumerable<T> list)
        {
            if (list == null) return new List<T>();
            else return new List<T>(list);
        }

        public IResolutionContext Copy => new ResolutionContext(TriggerContext, StartIndex,
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
}