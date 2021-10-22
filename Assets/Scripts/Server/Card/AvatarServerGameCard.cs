using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Cards
{
    public class AvatarServerGameCard : ServerGameCard
    {
        public override bool CanRemove => Summoned || Location == CardLocation.Nowhere;

        public override int Shield
        {
            get => base.Shield;
            protected set
            {
                base.Shield = value;
                LoseIfDead();
            }
        }

        public override int E 
        { 
            get => base.E;
            protected set
            {
                base.E = value;
                LoseIfDead();
            }
        }

        private bool summoned = true;
        public override bool Summoned => summoned;
        public override bool IsAvatar => true;

        public override bool Remove(IStackable stackSrc = null)
        {
            Debug.Log($"Trying to remove AVATAR {CardName} from {Location}");
            if (Summoned)
            {
                var corner = Space.AvatarCornerFor(ControllerIndex);
                var unfortunate = Game.boardCtrl.GetCardAt(corner);
                if(unfortunate != null) unfortunate.Owner.annihilationCtrl.Annihilate(unfortunate, stackSrc: stackSrc);
                Move(to: corner, normalMove: false, stackSrc: stackSrc);
                /*
                SetN(N - BaseN, stackSrc: stackSrc);
                SetW(W - BaseW, stackSrc: stackSrc);
                summoned = false;
                ServerNotifier.NotifyIncarnated(this, incarnated: false);
                */
                return true;
            }
            else return Location == CardLocation.Nowhere;
        }

        public override bool Incarnate(IStackable stackSrc = null)
        {
            if (Summoned) return false;
            var playContext = new ActivationContext(card: this, stackable: stackSrc, triggerer: stackSrc?.Controller, space: Position);
            SetN(N + BaseN, stackSrc: stackSrc);
            SetW(W + BaseW, stackSrc: stackSrc);
            summoned = true;
            ServerNotifier.NotifyIncarnated(this, incarnated: true);
            EffectsController.TriggerForCondition(Trigger.Play, playContext);
            //TODO DieIfApplicable(stackSrc);
            return true;
        }

        public void LoseIfDead()
        {
            if (E <= 0 && Shield <= 0) ServerGame.Lose(ControllerIndex);
        }

    }
}
