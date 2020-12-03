using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class PlayerChooseXSubeffect : ServerSubeffect
    {
        public XRestriction XRest;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            XRest.Initialize(Source);
        }

        private void AskForX()
        {
            EffectController.ServerNotifier.GetXForEffect();
        }

        public override bool Resolve()
        {
            AskForX();
            return false;
        }

        public void SetXIfLegal(int x)
        {
            if (XRest.Evaluate(x))
            {
                ServerEffect.X = x;
                ServerEffect.ResolveNextSubeffect();
            }
            else AskForX();
        }
    }
}