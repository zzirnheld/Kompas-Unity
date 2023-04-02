using KompasCore.Cards;
using System.Collections.Generic;

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
}