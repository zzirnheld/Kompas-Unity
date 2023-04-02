namespace KompasCore.Effects.Identities.Spaces
{
    public class ContextSpace : TriggerContextualLeafIdentityBase<Space>
    {
        protected override Space AbstractItemFrom(TriggeringEventContext context)
            => context.space;
    }
}