using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectNumberIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract int NumberLogic();

        public int Number() => initialized ? NumberLogic()
            : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class GamestateSubeffectNumberIdentity : SubeffectNumberIdentity
    {
        public GamestateNumberIdentity numberIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            numberIdentity.Initialize(restrictionContext);
        }

        protected override int NumberLogic() => numberIdentity.Number();
    }
}