namespace KompasCore.Effects.Identities.Numbers
{
    public class X : TriggerContextualLeafIdentityBase<int>
    {
        public int multiplier = 1;
        public int modifier = 0;
        public int divisor = 1;

        protected override int AbstractItemFrom(TriggeringEventContext contextToConsider)
            => (contextToConsider.x.GetValueOrDefault() * multiplier / divisor) + modifier;
    }
}