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
        //public override int CombatDamage => Summoned ? base.CombatDamage : 0;

        public override bool Remove(IStackable stackSrc = null)
        {
            Debug.Log($"Trying to remove AVATAR {CardName} from {Location}");
            if (Summoned)
            {
                Move(to: Space.AvatarCornerFor(ControllerIndex), normalMove: false, stackSrc: stackSrc);
                SetN(N - BaseN, stackSrc: stackSrc);
                SetW(W - BaseW, stackSrc: stackSrc);
                summoned = false;
                ServerNotifier.NotifyIncarnated(this, incarnated: false);
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
            return true;
        }
    }
}
