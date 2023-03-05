namespace KompasCore.Effects.Identities.Stackables
{
    public class StackableCause : ContextualLeafIdentityBase<IStackable>
    {
        protected override IStackable AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.stackableCause;
    }
}