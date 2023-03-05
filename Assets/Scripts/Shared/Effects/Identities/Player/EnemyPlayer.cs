namespace KompasCore.Effects.Identities.Players
{

    public class EnemyPlayer : ContextlessLeafIdentityBase<Player>
    {
        protected override Player AbstractItem => InitializationContext.Controller.Enemy;
    }
}
