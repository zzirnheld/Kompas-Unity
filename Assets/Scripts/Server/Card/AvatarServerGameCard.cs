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

        //TODO make this return whether the Avatar is summoned yet
        public override bool Summoned => false;
        public override bool IsAvatar => true;
        //public override int CombatDamage => Summoned ? base.CombatDamage : 0;

        public override bool Remove(IStackable stackSrc = null)
        {   
            if (Summoned) return base.Remove(stackSrc);
            else return Location == CardLocation.Nowhere;
        }

        public override void SetInfo(SerializableCard serializedCard, ServerGame game, ServerPlayer owner, ServerEffect[] effects, int id)
        {
            base.SetInfo(serializedCard, game, owner, effects, id);
        }
    }
}
