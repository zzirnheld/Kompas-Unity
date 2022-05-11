using KompasCore.Effects.Identities;

namespace KompasServer.Effects
{
    public class SetXByNumberIdentitySubeffect : SetXSubeffect
    {
        public INoActivationContextIdentity<int> numberIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            /*var ctxt = DefaultRestrictionContext;
            UnityEngine.Debug.Log($"Initializing with {ctxt}");
            numberIdentity.Initialize(initializationContext: ctxt);*/
            numberIdentity.Initialize(initializationContext: DefaultRestrictionContext);
        }

        public override int BaseCount => numberIdentity.Item;
    }
}