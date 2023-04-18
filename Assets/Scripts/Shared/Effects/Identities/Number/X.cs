namespace KompasCore.Effects.Identities.Numbers
{
    public class TriggerX : TriggerContextualLeafIdentityBase<int>
    {
        public int multiplier = 1;
        public int modifier = 0;
        public int divisor = 1;

        protected override int AbstractItemFrom(TriggeringEventContext contextToConsider)
            => (contextToConsider.x.GetValueOrDefault() * multiplier / divisor) + modifier;
    }

    public class EffectX : EffectContextualLeafIdentityBase<int>
    {
        public int multiplier = 1;
        public int modifier = 0;
        public int divisor = 1;

        protected override int AbstractItemFrom(IResolutionContext contextToConsider)
            => (contextToConsider.X * multiplier / divisor) + modifier;
    }
}