using KompasServer.Effects.Identities;

namespace KompasServer.Effects
{
    public class SetXByNumberIdentitySubeffect : SetXSubeffect
    {
        public SubeffectNumberIdentity numberIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            numberIdentity.Initialize(restrictionContext: RestrictionContext);
        }

        public override int BaseCount => numberIdentity.Number();
    }
}