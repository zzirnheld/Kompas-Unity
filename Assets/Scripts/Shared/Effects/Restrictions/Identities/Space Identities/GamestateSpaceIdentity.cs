namespace KompasCore.Effects.Identities
{
    public abstract class GamestateSpaceIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract Space SpaceLogic();

        public Space Space() => initialized ? SpaceLogic()
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class PositionOfGameSpaceIdentity : GamestateSpaceIdentity
    {
        public GamestateCardIdentity cardIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            cardIdentity.Initialize(restrictionContext);
        }

        protected override Space SpaceLogic() => cardIdentity.CardFrom(RestrictionContext.game).Position;
    }
}