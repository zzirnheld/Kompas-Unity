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

        public override void SetE(int e, IStackable stackSrc = null, bool notify = true)
        {
            base.SetE(e, stackSrc, notify);
            if (E <= 0) ServerGame.Lose(ControllerIndex);
        }

        private bool summoned = false;
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
                SetN(N - BaseN, stackSrc: stackSrc);
                SetW(W - BaseW, stackSrc: stackSrc);
                summoned = false;
                ServerNotifier.NotifyIncarnated(this, incarnated: false);
                return false;
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
            return true;
        }
    }
}
