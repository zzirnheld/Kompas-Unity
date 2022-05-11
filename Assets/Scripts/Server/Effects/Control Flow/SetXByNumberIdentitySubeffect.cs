using KompasCore.Effects.Identities;

namespace KompasServer.Effects
{
    public class SetXByNumberIdentitySubeffect : SetXSubeffect
    {
        public INoActivationContextNumberIdentity numberIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            numberIdentity.Initialize(initializationContext: DefaultRestrictionContext);
        }

        public override int BaseCount => numberIdentity.Number;
    }
}