namespace KompasServer.Effects
{
    public class ChooseOptionSubeffect : ServerSubeffect
    {
        public string ChoiceBlurb;
        public string[] OptionBlurbs;
        public int[] OptionJumpIndices;

        private void AskForOptionChoice()
        {
            EffectController.ServerNotifier.ChooseEffectOption(this);
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
