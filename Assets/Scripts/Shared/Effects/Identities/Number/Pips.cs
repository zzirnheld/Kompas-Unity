namespace KompasCore.Effects.Identities.Numbers
{
    public class Pips : ContextualParentIdentityBase<int>
    {
        public IIdentity<Player> player;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            player.Initialize(initializationContext);
        }

        protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => player.From(context, secondaryContext).Pips;
    }
}