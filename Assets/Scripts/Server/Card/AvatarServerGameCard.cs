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

        public override void Remove(IStackable stackSrc = null)
        {
            if (Location == CardLocation.Nowhere) return;
            var corner = Space.AvatarCornerFor(ControllerIndex);
            var unfortunate = Game.boardCtrl.GetCardAt(corner);
            if (unfortunate != null && unfortunate != this && !unfortunate.IsAvatar)
                unfortunate.Owner.annihilationCtrl.Annihilate(unfortunate, stackSrc: stackSrc);
            Move(to: corner, normalMove: false, stackSrc: stackSrc);
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
