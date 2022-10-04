namespace KompasCore.Effects.Identities.GamestatePlayerIdentities
{
    public class FriendlyPlayer : NoActivationContextIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.controller;
    }

    public class EnemyPlayer : NoActivationContextIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.controller.Enemy;
    }
}
