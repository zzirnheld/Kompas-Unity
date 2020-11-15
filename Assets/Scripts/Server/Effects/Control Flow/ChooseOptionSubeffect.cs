namespace KompasServer.Effects
{
    public class ChooseOptionSubeffect : ServerSubeffect
    {
        public string ChoiceBlurb;
        public string[] OptionBlurbs;
        public int[] OptionJumpIndices;

        private void AskForOptionChoice()
        {
            ServerPlayer.ServerNotifier.ChooseEffectOption(Source.CardName, ChoiceBlurb, OptionBlurbs);
        }

        public override bool Resolve()
        {
            AskForOptionChoice();
            return false;
        }

        public void ChooseOption(int optionIndex)
        {
            if (optionIndex >= OptionJumpIndices.Length)
            {
                //ask again
                AskForOptionChoice();
                return;
            }

            ServerEffect.ResolveSubeffect(OptionJumpIndices[optionIndex]);
        }
    }
}
