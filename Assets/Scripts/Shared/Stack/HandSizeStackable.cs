using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.Effects.Restrictions.CardRestrictionElements;

namespace KompasCore.Effects
{
	public abstract class HandSizeStackable : IStackable
	{
		public abstract Player Controller { get; }

		public GameCard Source => Controller.Avatar;

		private IRestriction<GameCardBase> handSizeCardRestriction;
		public IRestriction<GameCardBase> HandSizeCardRestriction
		{
			get
			{
				if (handSizeCardRestriction == null)
				{
					handSizeCardRestriction = new AllOf()
					{
						elements = new IRestriction<GameCardBase>[]
						{
							new Friendly(),
							new Location() { locations = new string[]{ "Hand" } }
						}
					};
					handSizeCardRestriction.Initialize(new EffectInitializationContext(game: Source.Game, source: default, controller: Controller));
				}
				return handSizeCardRestriction;
			}
		}

		public GameCard GetCause(GameCardBase withRespectTo) => Source;
	}
}