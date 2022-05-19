using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextIdentity<ReturnType> : IContextInitializeable
    {
        public ReturnType Item { get; }
    }

    /// <summary>
    /// Uniquely identifies a space.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateSpaceIdentity : ContextInitializeableBase,
        IActivationContextIdentity<Space>, INoActivationContextIdentity<Space>
    {
        protected abstract Space AbstractSpace { get; }

        public Space Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractSpace;
            }
        }

        public Space From(ActivationContext context, ActivationContext secondaryContext) => Item;
    }

    namespace GamestateSpaceIdentities
    {
        public class PositionOf : GamestateSpaceIdentity
        {
            public INoActivationContextIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(InitializationContext);
                card.Initialize(InitializationContext);
            }

            protected override Space AbstractSpace => card.Item.Position;
        }
    }
}