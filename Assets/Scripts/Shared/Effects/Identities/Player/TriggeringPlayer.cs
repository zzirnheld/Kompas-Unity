namespace KompasCore.Effects.Identities.Players
{

    public class TriggeringPlayer : TriggerContextualLeafIdentityBase<Player>
    {
        protected override Player AbstractItemFrom(TriggeringEventContext contextToConsider)
            => contextToConsider.player;
    }
}
