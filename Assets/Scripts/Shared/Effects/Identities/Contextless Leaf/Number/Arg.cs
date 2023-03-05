namespace KompasCore.Effects.Identities.Numbers
{
    public class Arg : ContextlessLeafIdentityBase<int>
    {
        protected override int AbstractItem => InitializationContext.subeffect.Effect.arg;
    }
}