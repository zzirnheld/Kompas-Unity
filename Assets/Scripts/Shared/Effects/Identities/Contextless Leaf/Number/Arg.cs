namespace KompasCore.Effects.Identities.ActivationContextNumberIdentities
{
    public class Arg : ContextlessLeafIdentityBase<int>
    {
        protected override int AbstractItem => InitializationContext.subeffect.Effect.arg;
    }
}