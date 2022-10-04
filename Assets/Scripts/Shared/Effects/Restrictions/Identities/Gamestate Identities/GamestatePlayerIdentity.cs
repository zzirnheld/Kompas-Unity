namespace KompasCore.Effects.Identities.GamestatePlayerIdentities
{
    public class FriendlyPlayer : NoActivationContextIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.Controller;
    }

    public class EnemyPlayer : NoActivationContextIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.Controller.Enemy;
    }
}
