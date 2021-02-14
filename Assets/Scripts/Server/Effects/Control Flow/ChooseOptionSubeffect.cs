using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ChooseOptionSubeffect : ServerSubeffect
    {
        public string ChoiceBlurb;
        public string[] OptionBlurbs;
        public int[] OptionJumpIndices;

        private async Task<int> AskForOptionChoice()
            => await ServerPlayer.serverAwaiter.GetEffectOption(Source.CardName, ChoiceBlurb, OptionBlurbs);

        public override async Task<ResolutionInfo> Resolve()
        {
            int choice = -1;
            while(choice < 0 || choice >= OptionJumpIndices.Length)
            {
                choice = await AskForOptionChoice();
            }

            return ResolutionInfo.Index(OptionJumpIndices[choice]);
        }
    }
}
