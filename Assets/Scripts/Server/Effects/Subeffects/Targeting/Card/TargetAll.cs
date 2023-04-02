using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAllSubeffect : CardTarget
    {
        public override bool IsImpossible() => !Game.Cards.Any(c => cardRestriction.IsValidCard(c, ResolutionContext));

        public override Task<ResolutionInfo> Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, ResolutionContext)).ToArray();
            //check what targets there are now, before you add them, to not mess with NotAlreadyTarget restriction
            //because Linq executes lazily, it would otherwise add the targets, then re-execute the query and not find any
            foreach (var t in targets) Effect.AddTarget(t);

            if (targets.Any()) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }

}