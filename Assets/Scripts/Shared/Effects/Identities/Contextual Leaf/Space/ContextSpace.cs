namespace KompasCore.Effects.Identities.ActivationContextSpaceIdentities
{
    public class ContextSpace : ContextualLeafIdentityBase<Space>
    {
        protected override Space AbstractItemFrom(ActivationContext context)
            => context.space;
    }
}