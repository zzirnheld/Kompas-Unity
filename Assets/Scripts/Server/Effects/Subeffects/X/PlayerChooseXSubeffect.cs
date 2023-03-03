using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class PlayerChooseXSubeffect : ServerSubeffect
    {
        public NumberRestriction XRest;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            XRest.Initialize(DefaultInitializationContext);
        }

        private async Task<int> AskForX() => await ServerPlayer.serverAwaiter.GetPlayerXValue();

        public override async Task<ResolutionInfo> Resolve()
        {
            bool xLegal = false;
            while (!xLegal)
            {
                int x = await AskForX();
                xLegal = SetXIfLegal(x);
            }
            return ResolutionInfo.Next;
        }

        public bool SetXIfLegal(int x)
        {
            if (XRest.IsValidNumber(x))
            {
                ServerEffect.X = x;
                return true;
            }
            return false;
        }
    }
}