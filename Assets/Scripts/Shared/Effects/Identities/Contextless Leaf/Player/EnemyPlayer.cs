namespace KompasCore.Effects.Identities.GamestatePlayerIdentities
{

    public class EnemyPlayer : ContextlessLeafIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.Controller.Enemy;
    }
}
