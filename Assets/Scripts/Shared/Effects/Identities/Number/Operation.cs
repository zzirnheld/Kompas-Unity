using System.Linq;

namespace KompasCore.Effects.Identities.Numbers
{
    public class Operation : ContextualParentIdentityBase<int>
    {
        public IIdentity<int>[] numbers;
        public INumberOperation operation;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            foreach(var identity in numbers)
            {
                identity.Initialize(initializationContext);
            }
        }

        protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            var numberValues = numbers.Select(n => n.From(context, secondaryContext)).ToArray();
            return operation.Perform(numberValues);
        }
    }
}