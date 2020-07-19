using UnityEngine;

namespace KompasServer.Effects
{
    public class LoopSubeffect : ServerSubeffect
    {
        public int jumpTo;
        public bool canDecline = false;

        protected virtual void OnLoopExit()
        {
            //make the "no other targets" button disappear
            if (canDecline)
            {
                EffectController.ServerNotifier.DisableDecliningTarget();
                EffectController.ServerNotifier.AcceptTarget(); // otherwise it keeps them in the now-irrelevant target mode
            }
        }

        protected virtual bool ShouldContinueLoop => true;

        public override bool Resolve()
        {
            //loop again if necessary
            Debug.Log($"im in ur loop of type {GetType()}, the one that jumps to {jumpTo}");
            if (ShouldContinueLoop)
            {
                //tell the client to enable the button to exit the loop
                if (canDecline)
                {
                    EffectController.ServerNotifier.EnableDecliningTarget();
                    ServerEffect.OnImpossible = this;
                }
                return ServerEffect.ResolveSubeffect(jumpTo);
            }
            else return ExitLoop();
        }

        /// <summary>
        /// Cancels the loop (because the player declined another target, or because there are no more valid targets)
        /// </summary>
        public bool ExitLoop()
        {
            //let parent know the loop is over
            if (ServerEffect.OnImpossible == this) ServerEffect.OnImpossible = null;

            //do anything necessary to clean up the loop
            OnLoopExit();

            //then skip to after the loop
            return ServerEffect.ResolveSubeffect(SubeffIndex + 1);
        }

        public override bool OnImpossible()
        {
            if (canDecline) return ExitLoop();
            else return base.OnImpossible();
        }
    }
}