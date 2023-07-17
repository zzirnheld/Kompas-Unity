using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects.Subeffects
{
	public class SetXByGamestate: SetX
	{
		public const string EffectUsesThisTurn = "Effect Uses This Turn";
		public const string NumberOfTargets = "Number of Targets";

		public string whatToCount;

		public CardValue cardValue;

		public IRestriction<GameCardBase> throughRestriction;
		public IRestriction<GameCardBase> cardRestriction;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);

			throughRestriction?.Initialize(DefaultInitializationContext);
			cardRestriction?.Initialize(DefaultInitializationContext);
			cardValue?.Initialize(DefaultInitializationContext);
		}

		public override int BaseCount
		{
			get
			{
				return whatToCount switch
				{
					EffectUsesThisTurn => Effect.TimesUsedThisTurn,
					NumberOfTargets => Effect.CardTargets.Count(),

					_ => throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect"),
				};
			}
		}
	}
}