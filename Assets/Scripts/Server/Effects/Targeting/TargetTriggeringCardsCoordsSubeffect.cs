using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardsCoordsSubeffect : ServerSubeffect
    {
        public bool after = false;

        public override Task<ResolutionInfo> Resolve()
        {
            var cardInfo = after ? Context.AfterCardInfo : Context.BeforeCardInfo;

            if (cardInfo == null) throw new NullCardException(TargetWasNull);
            else if (!cardInfo.Position.Valid) throw new InvalidSpaceException(cardInfo.Position, NoValidSpaceTarget);

            ServerEffect.AddSpace(cardInfo.Position.Copy);
            Debug.Log($"Just added {Space} from {cardInfo}");
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}