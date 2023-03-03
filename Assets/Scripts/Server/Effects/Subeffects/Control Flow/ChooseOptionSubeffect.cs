using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class ChooseOptionSubeffect : ServerSubeffect
    {
        public string choiceBlurb;
        public string[] optionBlurbs;
        public bool hasDefault = true;
        public bool showX = false;

        private async Task<int> AskForOptionChoice()
            => await ServerPlayer.serverAwaiter
                .GetEffectOption(cardName: Source.CardName,
                                 choiceBlurb: choiceBlurb,
                                 optionBlurbs: optionBlurbs,
                                 hasDefault: hasDefault,
                                 showX: showX,
                                 x: Effect.X);

        public override async Task<ResolutionInfo> Resolve()
        {
            int choice = -1;
            while (choice < 0 || choice >= jumpIndices.Length)
            {
                choice = await AskForOptionChoice();
            }

            return ResolutionInfo.Index(jumpIndices[choice]);
        }
    }
}
