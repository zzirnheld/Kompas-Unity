using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
	public class TargetTriggeringCardsSpace : ServerSubeffect
	{
		public bool after = false;

		public override Task<ResolutionInfo> Resolve()
		{
			var cardInfo = after
				? ResolutionContext.TriggerContext.MainCardInfoAfter
				: ResolutionContext.TriggerContext.mainCardInfoBefore;

			if (cardInfo == null) throw new NullCardException(TargetWasNull);
			else if (!cardInfo.Position.IsValid) throw new InvalidSpaceException(cardInfo.Position, NoValidSpaceTarget);

			ServerEffect.AddSpace(cardInfo.Position.Copy);
			Debug.Log($"Just added {SpaceTarget} from {cardInfo}");
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}