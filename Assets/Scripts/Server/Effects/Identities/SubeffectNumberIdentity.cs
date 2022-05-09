using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public interface ISubeffectNumberIdentity : IContextInitializeable
    {
        public int Number { get; }
    }

    public abstract class SubeffectNumberIdentityBase : ContextInitializeableBase, ISubeffectNumberIdentity
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
        public class FromGamestate : SubeffectNumberIdentityBase
        {
            public GamestateNumberIdentity numberIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                numberIdentity.Initialize(restrictionContext);
            }

            protected override int AbstractNumber => numberIdentity.Number;
        }

        public class X : SubeffectNumberIdentityBase
        {
            public int multiplier = 1;
            public int modifier = 0;
            public int divisor = 1;

            protected override int AbstractNumber
                => (RestrictionContext.subeffect.Effect.X * multiplier / divisor) + modifier;
        }
        public class Selector : SubeffectNumberIdentityBase
        {
            public INumberSelector selector;
            public ISubeffectManyNumbersIdentity numbers;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                numbers.Initialize(restrictionContext);
            }

            protected override int AbstractNumber
                => selector.Apply(numbers.Numbers);
        }
    }
}