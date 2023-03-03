using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class CountXLoopSubeffect : LoopSubeffect
    {
        //if true, increments x each iteration. if false, decrements
        public bool increment = true;

        protected override bool ShouldContinueLoop
        {
            get
            {
                //count the number of times this happens
                if (increment) ServerEffect.X++;
                else ServerEffect.X--;

                //let the effect know that if there are no more targets, then call this for loop exit
                ServerEffect.OnImpossible = this;

                //always return true, if another iteration is chosen not to happen exit loop will be called
                return true;
            }
        }

        //Specifically for this type of loop, if another target isn't found, that's when we exit the loop
        public override Task<ResolutionInfo> OnImpossible(string why) => ExitLoop();
    }
}