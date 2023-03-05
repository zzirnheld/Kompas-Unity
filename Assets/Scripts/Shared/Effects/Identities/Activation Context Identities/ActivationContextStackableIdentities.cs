namespace KompasCore.Effects.Identities
{
    namespace ActivationContextStackableIdentities
    {
        public class StackableCause : ContextualLeafIdentityBase<IStackable>
        {
            protected override IStackable AbstractItemFrom(ActivationContext contextToConsider)
                => contextToConsider.stackableCause;
        }
    }
}