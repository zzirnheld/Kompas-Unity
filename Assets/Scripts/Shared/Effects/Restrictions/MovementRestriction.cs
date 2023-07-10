using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
	public interface IMovementRestriction : IRestriction<Space>
	{
		public bool WouldBeValidNormalMoveInOpenGamestate(Space space);
	} //TODO add the get movement cost thing here

	public static class MovementRestrictionFactory
	{
		public static IMovementRestriction CreateDefault()
			=> new SpaceRestrictionElements.MovementRestriction();
	}

	namespace SpaceRestrictionElements
	{
		public class MovementRestriction : RestrictionBase<Space>, IMovementRestriction
		{
			/// <summary>
			/// A piece of a movement restriction.
			/// Basically, a movement restriction is made up of two groups of restrictions -
			/// one that's checked for a normal move (i.e. player-initiated during an open gamestate),
			/// and one that's checked when the card moves by effect
			/// </summary>
			private class ComponentMovementRestriction : AllOf
			{
				private static IEnumerable<IRestriction<Space>> DefaultMovementRestrictions
				{
					get
					{
						//Card must be in play
						yield return new TriggerRestrictionElements.CardFitsRestriction()
						{
							card = new Identities.Cards.ThisCardNow(),
							cardRestriction = new CardRestrictionElements.Location(CardLocation.Board)
						};

						//TODO re-add swapping probably. will req DefaultEffectElements
						yield return new SpaceRestrictionElements.Empty();

						//Can't "move" to the space the card is in now
						yield return new SpaceRestrictionElements.Different()
						{
							from = new Identities.Cards.ThisCardNow()
						};

						yield return new SpellRule();
					}
				}

				private readonly IReadOnlyList<IRestriction<Space>> restrictions;

				/// <summary>
                /// Constructs a new piece of an overall MovementRestriction
                /// </summary>
                /// <param name="sharedRestrictions">Restrictions that are shared among all elements of the MovementRestriction.
                /// If this is null, the DefaultMovementRestrictions are used instead.</param>
                /// <param name="specificRestrictions">Restrictions that are specific to this particular ComponentMovementRestriction</param>
				public ComponentMovementRestriction(IEnumerable<IRestriction<Space>> sharedRestrictions,
					params IEnumerable<IRestriction<Space>>[] specificRestrictions)
				{
					var interimRestrictions = sharedRestrictions ?? DefaultMovementRestrictions;
					foreach(var additionalRestrictions in specificRestrictions)
					{
						interimRestrictions = interimRestrictions.Concat(additionalRestrictions);
					}
					this.restrictions = interimRestrictions.ToArray();
				}

				//In this case, "Default" is checked when Initialize is called. since this gets constructed before it's initialized, we're good.
				protected override IEnumerable<IRestriction<Space>> DefaultElements => restrictions;
			}

			//TODO: move to shared spot with PlayRestriction?
			/// <summary>
			/// Spell rule: you can't place a spell where it would block a path, through spaces that don't contain a spell, between the avatars
			/// So to be valid, a card has to either not be a spell, or it has to be a valid place to put a spell.
			/// </summary>
			private class SpellRule : SpaceRestrictionElement
			{
				protected override bool IsValidLogic(Space item, IResolutionContext context)
					=> InitializationContext.source.CardType != 'S'
					|| InitializationContext.game.BoardController.ValidSpellSpaceFor(InitializationContext.source, item);
			}

			private ComponentMovementRestriction normal;
			private ComponentMovementRestriction effect;

			public SpaceRestrictionElement[] normalAndEffect = null;
			public SpaceRestrictionElement[] normalOnly = new SpaceRestrictionElement[] { };
			public SpaceRestrictionElement[] effectOnly = new SpaceRestrictionElement[] { };

			public bool moveThroughCards = false; //TODO check this flag when determining how much "movement" the move should cost.
												  //ideally implement some sort of "get move cost to" function here, which can be replaced by an Identity as applicable

			/// <summary>
			/// Restrictions that, by default, apply to a player moving a card normally (but not by effect)
			/// </summary>
			private IEnumerable<IRestriction<Space>> DefaultNormalElements
			{
				get
				{
					//Only characters can move, normally
					yield return new TriggerRestrictionElements.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new CardRestrictionElements.Character()
					};
					yield return new SpaceRestrictionElements.CompareDistance()
					{
						//If you can move through cards, you just care about the taxicab distance.
						//Most cards have to move through an empty path
						shortestEmptyPath = !moveThroughCards,
						distanceTo = new Identities.Cards.ThisCardNow(),
						comparison = new Relationships.NumberRelationships.LessThanEqual(),
						number = new Identities.Numbers.FromCardValue()
						{
							cardValue = new CardValue() { value = CardValue.SpacesCanMove },
							card = new Identities.Cards.ThisCardNow()
						}
					};
					yield return new TriggerRestrictionElements.NothingHappening();
					yield return new TriggerRestrictionElements.FriendlyTurn();
				}
			}

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				normal = new ComponentMovementRestriction(normalAndEffect, DefaultNormalElements, normalOnly);
				effect = new ComponentMovementRestriction(normalAndEffect, effectOnly);
			}

			protected override bool IsValidLogic(Space item, IResolutionContext context)
				=> context.TriggerContext.stackableCause == null
					? normal.IsValid(item, context)
					: effect.IsValid(item, context);

			public bool WouldBeValidNormalMoveInOpenGamestate(Space item)
				=> normal.IsValidIgnoring(item, ResolutionContext.PlayerTrigger(null, InitializationContext.game),
					restriction => restriction is TriggerRestrictionElements.NothingHappening); //ignore req that nothing is happening
		}
	}
}