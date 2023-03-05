namespace KompasCore.Effects.Identities.Spaces
{
    public class ContextSpace : ContextualLeafIdentityBase<Space>
    {
        protected override Space AbstractItemFrom(ActivationContext context)
            => context.space;
    }
}