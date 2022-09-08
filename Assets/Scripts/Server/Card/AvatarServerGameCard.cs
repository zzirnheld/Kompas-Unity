using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using KompasServer.Effects;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Cards
{
    public class AvatarServerGameCard : ServerGameCard
    {
        public override bool Summoned => false;
        public override bool IsAvatar => true;

        public override bool Remove(IStackable stackSrc = null)
        {
            if (Location == CardLocation.Nowhere) return true;
            var corner = Space.AvatarCornerFor(ControllerIndex);
            var unfortunate = Game.BoardController.GetCardAt(corner);
            if (unfortunate != null && unfortunate != this && !unfortunate.IsAvatar)
                unfortunate.Owner.annihilationCtrl.Annihilate(unfortunate, stackSrc: stackSrc);
            Move(to: corner, normalMove: false, stackSrc: stackSrc);
            return false;
        }

        public override void SetE(int e, IStackable stackSrc, bool onlyStatBeingSet = true)
        {
            base.SetE(e, stackSrc, onlyStatBeingSet);
            LoseIfDead();
        }

        public void LoseIfDead()
        {
            if (E <= 0) ServerGame.Lose(ControllerIndex);
        }

    }
}
