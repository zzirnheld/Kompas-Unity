namespace KompasCore.Effects.Identities.ActivationContextNumberIdentities
{
    public class X : ContextualLeafIdentityBase<int>
    {
        public int multiplier = 1;
        public int modifier = 0;
        public int divisor = 1;

        protected override int AbstractItemFrom(ActivationContext contextToConsider)
            => (contextToConsider.x.GetValueOrDefault() * multiplier / divisor) + modifier;
    }
}