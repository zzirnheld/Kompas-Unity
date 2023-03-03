namespace KompasServer.Effects.Subeffect
{
    public class TTimesSubeffect : LoopSubeffect
    {
        public int T;
        private int count = 0;

        protected override void OnLoopExit()
        {
            base.OnLoopExit();
            count = 0;
        }

        protected override bool ShouldContinueLoop
        {
            get
            {
                count++;
                return count < T;
            }
        }
    }
}