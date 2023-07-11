using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects
{
	namespace Restrictions
	{
		public interface IPlayRestriction : IRestriction<(Space s, Player p)>
		{
			public static IPlayRestriction CreateDefault() => new PlayRestrictionElements.PlayRestriction();

			public bool IsRecommendedNormalPlay((Space s, Player p) item);
			public bool IsRecommendedPlay((Space s, Player p) item, IResolutionContext context);
		}

		namespace PlayRestrictionElements
		{
			public class AnyOf : AnyOfBase<(Space s, Player p)> { }

			public class StandardPlayRestriction : RestrictionBase<(Space s, Player p)>
			{
				protected override bool IsValidLogic((Space s, Player p) item, IResolutionContext context)
					=> InitializationContext.game.IsValidStandardPlaySpace(item.s, item.p);
			}

			public class PlayRestriction : DualRestrictionBase<(Space s, Player p)>, IPlayRestriction
			{
				public bool playAsAugment = false;
				public string[] augmentOnSubtypes;

				public bool requireStandardAdjacency = true;

				public IRestriction<(Space s, Player p)>[] recommendations = { };

				public override void Initialize(EffectInitializationContext initializationContext)
				{
					base.Initialize(initializationContext);
					foreach (var r in recommendations) r.Initialize(initializationContext);
				}

				private static IRestriction<(Space s, Player p)> OnOrAdjacentToFriendly() => new AnyOf()
					{
						restrictions = new IRestriction<(Space s, Player p)>[] {
							new CardRestrictionElements.Friendly(),
							new SpaceRestrictionElements.AdjacentTo()
							{
								cardRestriction = new CardRestrictionElements.Friendly()
							}
						}
					};

				protected override IEnumerable<IRestriction<(Space s, Player p)>> DefaultRestrictions
				{
					get
					{
						yield return new SpaceRestrictionElements.SpellRule();
						yield return new GamestateRestrictionElements.NoUniqueCopyExists();

						if (playAsAugment)
						{
							if (augmentOnSubtypes != null) yield return new CardRestrictionElements.Subtypes() { subtypes = augmentOnSubtypes };

							//On or adjacent to a friendly
							if (requireStandardAdjacency) yield return OnOrAdjacentToFriendly();
						}
						else
						{
							yield return new SpaceRestrictionElements.Empty();

							if (requireStandardAdjacency) yield return new StandardPlayRestriction();
						}
					}
				}

				protected override IEnumerable<IRestriction<(Space s, Player p)>> DefaultNormalRestrictions
				{
					get
					{
						yield return new GamestateRestrictionElements.NothingHappening();

						//Can afford to play
						yield return new TriggerRestrictionElements.NumberFitsRestriction()
						{
							number = new Identities.Numbers.FromCardValue()
							{
								card = new Identities.Cards.ThisCardNow(),
								cardValue = new CardValue() { value = CardValue.Cost }
							}
						};

						//Currently controls the card in hand
						yield return new PlayerRestrictionElements.PlayersMatch()
						{
							player = new Identities.Players.ControllerOf()
						};
						yield return new TriggerRestrictionElements.CardFitsRestriction()
						{
							card = new Identities.Cards.ThisCardNow(),
							cardRestriction = new CardRestrictionElements.Location(CardLocation.Hand)
						};
					}
				}

				public bool IsRecommendedNormalPlay((Space s, Player p) item)
					=> IsRecommendedPlay(item, ResolutionContext.PlayerTrigger(null, InitializationContext.game));

				public bool IsRecommendedPlay((Space s, Player p) item, IResolutionContext context)
					=> IsValid(item, context)
					&& recommendations.All(r => r.IsValid(item, context));
			}
		}
	}
}