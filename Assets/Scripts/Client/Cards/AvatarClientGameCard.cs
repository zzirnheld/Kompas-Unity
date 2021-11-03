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
    }
}
