using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;

namespace KompasClient.Cards
{
    public class AvatarClientGameCard : ClientGameCard
    {
        public override bool Summoned => false;
        public override bool IsAvatar => true;
        public override int CombatDamage => Summoned ? base.CombatDamage : 0;

        public override bool Remove(IStackable stackSrc = null)
        {
            // Debug.LogWarning("Remove called for Avatar - doing nothing");
            if (Summoned) return base.Remove(stackSrc);
            else return Location == CardLocation.Nowhere;
        }

        public override void SetInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
            => base.SetInfo(serializedCard, game, owner, effects, id);
    }
}
