using System.Threading.Tasks;

namespace KompasServer.Effects
{
    /// <summary>
    /// Represents an entire keyword part of an effect,
    /// like Mech Pilot (or, hopefully soon, Invoke)
    /// </summary>
    public class KeywordSubeffect : ServerSubeffect
    {
        public string keyword;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            var subeffects = Game.cardRepo.InstantiateServerPartialKeyword(keyword);
            ServerEffect.InsertSubeffects(subeffIndex + 1, subeffects);
            //The subeffects will then be initialized by the calling Effect
        }

        //This effect doesn't do anything when it resolves.
        //TODO maybe later it should stash any existing targets/rest, or clear it out?
        //I think it'd be more useful to have access to what targets the partial keyword did use,
        //for things like "Mech Pilot, and"
        public override Task<ResolutionInfo> Resolve() => Task.FromResult(ResolutionInfo.Next);
    }
}