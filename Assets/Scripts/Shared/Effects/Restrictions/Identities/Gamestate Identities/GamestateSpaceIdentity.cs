namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextSpaceIdentity : IContextInitializeable
    {
        public Space Space { get; }
    }

    /// <summary>
    /// Uniquely identifies a space.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateSpaceIdentity : ContextInitializeableBase,
        IActivationContextSpaceIdentity, INoActivationContextSpaceIdentity
    {
        protected abstract Space AbstractSpace { get; }

        public Space Space
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractSpace;
            }
        }

        public Space SpaceFrom(ActivationContext context) => Space;
    }

    namespace GamestateSpaceIdentities
    {
        public class PositionOf : GamestateSpaceIdentity
        {
            public INoActivationContextCardIdentity card;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                card.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => card.Card.Position;
        }
    }
}