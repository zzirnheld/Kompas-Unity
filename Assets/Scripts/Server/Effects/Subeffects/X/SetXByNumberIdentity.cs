using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Subeffects
{
    public class SetXByNumberIdentity: SetX
    {
        public IActivationContextIdentity<int> numberIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            /*var ctxt = DefaultRestrictionContext;
            UnityEngine.Debug.Log($"Initializing with {ctxt}");
            numberIdentity.Initialize(initializationContext: ctxt);*/
            numberIdentity.Initialize(initializationContext: DefaultInitializationContext);
        }

        public override int BaseCount => numberIdentity.From(CurrentContext, CurrentContext);
    }
}