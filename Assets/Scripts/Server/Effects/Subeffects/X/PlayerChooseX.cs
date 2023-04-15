using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class PlayerChooseX : ServerSubeffect
    {
        public NumberRestriction XRest;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            XRest.Initialize(DefaultInitializationContext);
        }

        private async Task<int> AskForX() => await ServerPlayer.awaiter.GetPlayerXValue();

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
            if (XRest.IsValid(x, ResolutionContext))
            {
                ServerEffect.X = x;
                return true;
            }
            return false;
        }
    }
}