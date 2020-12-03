using System.Linq;

namespace KompasServer.Effects
{
    public class AddRestSubeffect : CardTargetSubeffect
    {
        public override bool Resolve()
        {
            Effect.rest.AddRange(ServerGame.Cards.Where(c => cardRestriction.Evaluate(c)));
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}