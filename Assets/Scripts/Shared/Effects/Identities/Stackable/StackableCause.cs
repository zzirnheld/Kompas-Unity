namespace KompasCore.Effects.Identities.Stackables
{
    public class StackableCause : TriggerContextualLeafIdentityBase<IStackable>
    {
        protected override IStackable AbstractItemFrom(TriggeringEventContext contextToConsider)
            => contextToConsider.stackableCause;
    }
}