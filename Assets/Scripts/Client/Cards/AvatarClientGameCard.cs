using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;

namespace KompasClient.Cards
{
    public class AvatarClientGameCard : ClientGameCard
    {
        public override bool Summoned => false;
        public override bool IsAvatar => true;

        public AvatarClientGameCard(SerializableCard serializedCard, ClientPlayer owner, ClientEffect[] effects, int id, ClientCardController clientCardController)
            : base(serializedCard, id, owner, effects, clientCardController)
        {

        }
    }
}
