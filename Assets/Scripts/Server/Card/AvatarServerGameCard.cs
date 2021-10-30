using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Cards
{
    public class AvatarServerGameCard : ServerGameCard
    {
        public override int E 
        { 
            get => base.E;
            protected set
            {
                base.E = value;
                LoseIfDead();
            }
        }
        public override bool Summoned => false;
        public override bool IsAvatar => true;

        public override bool Remove(IStackable stackSrc = null)
        {
            Debug.Log($"Trying to remove AVATAR {CardName} from {Location}");
            if (Summoned)
            {
                var corner = Space.AvatarCornerFor(ControllerIndex);
                var unfortunate = Game.boardCtrl.GetCardAt(corner);
                if(unfortunate != null && unfortunate != this) unfortunate.Owner.annihilationCtrl.Annihilate(unfortunate, stackSrc: stackSrc);
                Move(to: corner, normalMove: false, stackSrc: stackSrc);
                return true;
            }
            else return Location == CardLocation.Nowhere;
        }

        public void LoseIfDead()
        {
            if (E <= 0) ServerGame.Lose(ControllerIndex);
        }

    }
}
