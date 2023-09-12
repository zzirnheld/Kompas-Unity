using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions
{
	public abstract class PlayerRestrictionElement : RestrictionBase<Player>, IRestriction<GameCardBase>, IRestriction<(Space s, Player p)>
	{
		public bool IsValid(GameCardBase item, IResolutionContext context)
			=> IsValid(item.Controller, context);

		public bool IsValid((Space s, Player p) item, IResolutionContext context)
			=> IsValid(item.p, context);
	}

	namespace PlayerRestrictionElements
	{
		public class AllOf : AllOfBase<Player> { }

		public class Not : PlayerRestrictionElement
		{
			[JsonProperty(Required = Required.Always)]
			public IRestriction<Player> negated;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				negated.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(Player item, IResolutionContext context)
				=> !negated.IsValid(item, context);
		}
	}
}