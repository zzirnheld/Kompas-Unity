namespace KompasCore.Effects.Identities.GamestatePlayerIdentities
{
    public class FriendlyPlayer : ContextlessLeafIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.Controller;
    }
}
