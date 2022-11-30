namespace KompasCore.Effects.Identities
{
    namespace ActivationContextStackableIdentities
    {
        public class StackableCause : ActivationContextIdentityBase<IStackable>
        {
            protected override IStackable AbstractItemFrom(ActivationContext contextToConsider)
                => contextToConsider.stackableCause;
        }
    }
}