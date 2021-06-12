using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.Cards
{
    public class AvatarClientGameCard : ClientGameCard
    {
        private bool summoned = false;
        public override bool Summoned => summoned;
        public override bool IsAvatar => true;
        public override int CombatDamage => Summoned ? base.CombatDamage : 0;

        public override bool Remove(IStackable stackSrc = null)
        {
            // Debug.LogWarning("Remove called for Avatar - doing nothing");
            if (Summoned)
            {
                summoned = false;
                return true;
            }
            else return Location == CardLocation.Nowhere;
        }

        public override bool Incarnate(IStackable stackSrc = null)
        {
            if (Summoned) return false;
            summoned = true;
            return true;
        }
    }
}
