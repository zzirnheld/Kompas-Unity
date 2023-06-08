using System.Collections.Generic;
using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions
{
	public abstract class PlayerRestrictionElement : RestrictionBase<Player>, IRestriction<GameCardBase>
	{
		public bool IsValid(GameCardBase item, IResolutionContext context)
			=> IsValid(item.Controller, context);
	}

	namespace PlayerRestrictionElements
	{
		public class AllOf : AllOfBase<Player> { }

		public class Not : PlayerRestrictionElement
		{
			public IRestriction<Player> negated;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				negated.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(Player item, IResolutionContext context)
				=> !negated.IsValid(item, context);
		}

		public class ActivationRestriction : AllOf
		{
			public string[] locations = { CardLocation.Board.StringVersion() };

			protected override IEnumerable<IRestriction<Player>> DefaultElements
			{
				get
				{
					yield return new GamestateRestrictionElements.FriendlyTurn();
					yield return new TriggerRestrictionElements.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new CardRestrictionElements.AllOf()
						{
							elements = new IRestriction<GameCardBase>[] {
								new CardRestrictionElements.Location() { locations = this.locations },
								new CardRestrictionElements.Not() { negated = new CardRestrictionElements.Negated() }
							}
						}
					};
					yield return new GamestateRestrictionElements.NothingHappening();
					yield return new GamestateRestrictionElements.Not() { negated = new GamestateRestrictionElements.EffectAlreadyTriggered() };
					yield return new PlayersMatch() { player = new Identities.Players.FriendlyPlayer() };
				}
			}
		}
	}
}