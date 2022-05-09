using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectNumberIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract int AbstractNumber { get; }

        public int Number
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractNumber;
            }
        }
    }

    namespace SubeffectNumberIdentities
    {
        public class FromGamestate : SubeffectNumberIdentity
        {
            public GamestateNumberIdentity numberIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                numberIdentity.Initialize(restrictionContext);
            }

            protected override int AbstractNumber => numberIdentity.Number;
        }

        public class X : SubeffectNumberIdentity
        {
            public int multiplier = 1;
            public int modifier = 0;
            public int divisor = 1;

            protected override int AbstractNumber
                => (RestrictionContext.subeffect.Effect.X * multiplier / divisor) + modifier;
        }
    }
}