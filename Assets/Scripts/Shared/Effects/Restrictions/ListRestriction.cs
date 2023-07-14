using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects
{
	namespace Restrictions
	{
		public interface IListRestriction : IRestriction<IEnumerable<GameCardBase>>
		{
			public bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context);

			public bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context);

			/// <summary>
            /// If you don't specifically want to constrain the list (i.e. by deduplicating on a particular value),
            /// have this just return the source sequence.
            /// This will be overridden by restrictions like "distinct names" and "distinct costs"
            /// </summary>
			public IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options);

			public int GetMinimum(IResolutionContext context);
		}
	}

	namespace Restrictions.ListRestrictionElements
	{
		public class AllOf : AllOfBase<IEnumerable<GameCardBase>, IListRestriction>, IListRestriction
		{
			private bool clientSide;

			protected override bool Validate(IListRestriction element, IEnumerable<GameCardBase> item, IResolutionContext context)
				=> clientSide
				? element.IsValidClientSide(item, context)
				: element.IsValid(item, context);

			public bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
			{
				ComplainIfNotInitialized();
				return elements.All(elem => elem.AllowsValidChoice(options, context));
			}

			public bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context)
			{
				clientSide = true;
				bool ret = IsValid(options, context);
				clientSide = false;
				return ret;
			}

			public IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options)
			{
				var localOptions = options;
				foreach(var elem in elements) localOptions = elem.Deduplicate(localOptions);
				return localOptions;
			}

			public int GetMinimum(IResolutionContext context)
				=> elements
					.Select(elem => elem.GetMinimum(context))
					.DefaultIfEmpty(0)
					.Max();
		}

		public abstract class ListRestrictionElementBase : RestrictionBase<IEnumerable<GameCardBase>>, IListRestriction
		{
			public virtual int GetMinimum(IResolutionContext context) => 0;

			public virtual IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options) => options; //No dedup, by default

			public virtual bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context) => IsValid(options, context);

			public abstract bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context);
		}

		public abstract class NumberBound : ListRestrictionElementBase
		{
			/// <summary>
            /// Used for sending a current minimum to the client.
            /// Obviously, the simpler solution is to assume that we're always either a constant or X,
            /// but that makes any other manipulation much harder than it needs to be.
            /// This is more flexible long-term, even if it is more annoying.
            /// </summary>
			public int stashedBound;

			public IIdentity<int> bound;

			/// <summary>
			/// This function should always be called before the list restriction is sent over.
			/// Consider replacing with a "prep for sending" function?
			/// </summary>
			public int StashBound(IResolutionContext context)
			{
				stashedBound = bound.From(context);
				return stashedBound;
			}
		}

		public class Minimum : NumberBound
		{
			protected override bool IsValidLogic(IEnumerable<GameCardBase> item, IResolutionContext context)
				=> item.Count() >= StashBound(context);

			public override bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
				=> options.Count() >= StashBound(context);

			public override bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context)
				=> options.Count() >= stashedBound;

			public override int GetMinimum(IResolutionContext context) => StashBound(context);
		}

		public class Maximum : NumberBound
		{
			protected override bool IsValidLogic(IEnumerable<GameCardBase> item, IResolutionContext context)
				=> item.Count() <= StashBound(context);

			public override bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
				=> true;

			public override bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context)
				=> options.Count() <= stashedBound;
		}

		public class ControllerCanPayCost : ListRestrictionElementBase
		{
			protected override bool IsValidLogic(IEnumerable<GameCardBase> item, IResolutionContext context)
				=> item.Select(c => c.Cost).Sum() <= InitializationContext.Controller.Pips;

			public override bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
			{
				if (!(InitializationContext.parent is AllOf parent)) return true;

				int min = parent.elements
					.Where(elem => elem is Minimum)
					.Select(min => min as Minimum)
					.Select(min => min.StashBound(context))
					.DefaultIfEmpty(0)
					.Max(); //We want the highest (i.e. most constraining) lower bound

				//Accounts for all deduplicating of other possible things like distinct name, but doesn't check that there are enough (those deduplicators check that)
				return parent.Deduplicate(options)
					.Select(c => c.Cost)
					.OrderBy(c => c)
					.Take(min)
					.Sum() <= InitializationContext.Controller.Pips;
			}
		}

		public abstract class Distinct : ListRestrictionElementBase
		{
			protected delegate object DistinguishingValueSelector(GameCardBase card);
			protected abstract DistinguishingValueSelector SelectDistinguishingValue { get; }

			private class DistinctCardComparer : IEqualityComparer<GameCardBase>
			{
				private readonly DistinguishingValueSelector selectDistinguishingValue;

				public bool Equals(GameCardBase x, GameCardBase y) => selectDistinguishingValue(x) == selectDistinguishingValue(y);
				public int GetHashCode(GameCardBase obj) => selectDistinguishingValue(obj).GetHashCode();

				public DistinctCardComparer(DistinguishingValueSelector selectDistinguishingValue)
				{
					this.selectDistinguishingValue = selectDistinguishingValue;
				}
			}

			public override bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context)
			{
				if (!(InitializationContext.parent is AllOf parent)) return true;

				//Ensure there exists a selection that fits the required minimum count
				return options
					.Select(card => SelectDistinguishingValue(card))
					.Count() >= parent.GetMinimum(context);
			}

			protected override bool IsValidLogic(IEnumerable<GameCardBase> item, IResolutionContext context)
				=> item.Count()
				== item.Select(card => SelectDistinguishingValue(card)).Distinct().Count(); //Ensure that particular selection is distinct

			public override IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options)
				=> options.Distinct(new DistinctCardComparer(SelectDistinguishingValue));
		}

		public class DistinctNames : Distinct
		{
			protected override DistinguishingValueSelector SelectDistinguishingValue => card => card.CardName;
		}

		public class DistinctCosts : Distinct
		{
			protected override DistinguishingValueSelector SelectDistinguishingValue => card => card.Cost;
		}
	}

	public class ListRestriction : ContextInitializeableBase
	{
		[JsonIgnore]
		public Subeffect Subeffect => InitializationContext.subeffect;

		//if i end up living towards the heat death of the universe,
		//i will refactor this to instead be objects that get deserialized.
		//they will probably be tiny little classes at the bottom of this.
		//dang. that actually makes it sound mostly trivial.
		#region restrictions
		public const string MinCanChoose = "Min Can Choose";
		public const string MaxCanChoose = "Max Can Choose";

		public const string ControllerCanPayCost = "Can Pay Cost"; //effect's controller is able to pay the cost of all of them together
		public const string DistinctCosts = "Distinct Costs";
		public const string Distinct = "Distinct";

		public const string MinOfX = "Min Can Choose: X";
		public const string MaxOfX = "Max Can Choose: X";
		#endregion restrictions

		public string[] listRestrictions = new string[0];

		/// <summary>
		/// A quick little property that tells you whether the list restriction has a limit to how many can be chosen.
		/// </summary>
		public bool HasMax => listRestrictions.Contains(MaxCanChoose) || listRestrictions.Contains(MaxOfX);

		/// <summary>
		/// Quick little property that informs you whether the list restriction has minimum
		/// </summary>
		public bool HasMin => listRestrictions.Contains(MinCanChoose) || listRestrictions.Contains(MinOfX);

		/// <summary>
		/// Quick little property to summarize whether or not we're choosing more than one card.
		/// </summary>
		public bool ChooseMultiple => !(HasMax && maxCanChoose <= 1);

		/// <summary>
		/// Quick little method that tells you if you have selected enough items.
		/// </summary>
		/// <param name="count">Number of items currently selected</param>
		/// <returns>Whether the number of items currently selected is enough.</returns>
		public bool HaveEnough(int count) => !HasMin || count >= minCanChoose;

		/// <summary>
		/// The maximum number of cards that can be chosen.
		/// Default: one card can be chosen
		/// </summary>
		public int maxCanChoose = 1;

		/// <summary>
		/// The minimum number of cards that must be chosen.
		/// If is < 0, gets set to maxCanChoose
		/// Default: set to maxCanChoose
		/// </summary>
		public int minCanChoose = -1;

		/// <summary>
		/// Default ListRestriction. <br></br>
		/// Specifies a max and min of 1 card.
		/// </summary>
		public static ListRestriction Default => new ListRestriction()
		{
			listRestrictions = new string[] { MinCanChoose, MaxCanChoose }
		};

		/// <summary>
		/// Default ListRestriction Json. <br></br>
		/// A json representation of <see cref="Default"/>
		/// </summary>
		public static readonly string DefaultJson = JsonConvert.SerializeObject(Default);

		/// <summary>
		/// You can read, you know what this does.
		/// Initializes the list restriction to know who its daddy is, and make any shtuff match up
		/// </summary>
		/// <param name="subeffect"></param>
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (minCanChoose < 0 && listRestrictions.Contains(MaxCanChoose))
				minCanChoose = maxCanChoose;
		}

		/// <summary>
		/// Prepares the list restriction to be sent to a player alongside a get card target request.
		/// This exists in case I ever need to add information, 
		/// so I can make the compiler tell me where else needs to provide information.
		/// </summary>
		/// <param name="x">The value of x to use, in case the list restriction cares about X.</param>
		public void PrepareForSending(int x)
		{
			if (listRestrictions.Contains(MinOfX)) minCanChoose = x;
			if (listRestrictions.Contains(MaxOfX)) maxCanChoose = x;
		}

		private bool IsRestrictionValid(string restriction, IEnumerable<GameCard> cards) => restriction switch
		{
			MinCanChoose => cards.Count() >= minCanChoose,
			MaxCanChoose => cards.Count() <= maxCanChoose,

			ControllerCanPayCost => Subeffect.Controller.Pips >= cards.Sum(c => c.Cost),
			DistinctCosts => cards.Select(c => c.Cost).Distinct().Count() == cards.Count(),
			Distinct => cards.Select(c => c.CardName).Distinct().Count() == cards.Count(),

			MinOfX => cards.Count() <= Subeffect.Effect.X,
			MaxOfX => cards.Count() <= Subeffect.Effect.X,
			_ => throw new System.ArgumentException($"Invalid list restriction {restriction}", "restriction"),
		};

		/*
		//TODO use #ifdef to be able to turn on debug with a compile flag or something
		private bool EvaluateRestrictionWithDebug(string restriction, IEnumerable<GameCard> cards)
		{
			bool valid = EvaluateRestriction(restriction, cards);
			if (!valid) Debug.Log($"Invalid list of cards {string.Join(", ", cards.Select(c => c.CardName))} " +
				$"flouts list restriction {restriction}");
			return valid;
		}*/

		/// <summary>
		/// Checks the list of cards passed into see if they collectively fit a restriction.
		/// </summary>
		/// <param name="choices">The list of cards to collectively evaluate.</param>
		/// <returns><see langword="true"/> if the cards fit all the required restrictions collectively, 
		/// <see langword="false"/> otherwise</returns>
		public bool IsValidList(IEnumerable<GameCard> choices, IEnumerable<GameCard> potentialTargets)
		{
			ComplainIfNotInitialized();
			return choices != null
				&& !choices.Except(potentialTargets).Any() //Are there any choices that aren't potential targets?
				&& listRestrictions.All(r => IsRestrictionValid(r, choices));
		}

		private bool CanPayCost(IEnumerable<GameCard> potentialTargets)
		{
			int costAccumulation = 0;
			int i = 1;
			foreach (var card in potentialTargets.OrderBy(c => c.Cost))
			{
				if (i > minCanChoose) break;
				costAccumulation += card.Cost;
				i++;
			}
			if (i < minCanChoose) return false;
			return costAccumulation <= Subeffect.Controller.Pips;
		}

		private bool DoesRestrictionAllowValidChoice(string restriction, IEnumerable<GameCard> potentialTargets) => restriction switch
		{
			MinOfX => potentialTargets.Count() >= Subeffect.Effect.X,
			MinCanChoose => potentialTargets.Count() >= minCanChoose,

			ControllerCanPayCost => CanPayCost(potentialTargets),
			DistinctCosts => potentialTargets.Select(c => c.Cost).Distinct().Count() > (HasMin ? minCanChoose : 0),
			Distinct => potentialTargets.Select(c => c.CardName).Count() > (HasMin ? minCanChoose : 0),

			MaxOfX => true,
			MaxCanChoose => true,

			_ => throw new System.ArgumentException($"Invalid list restriction {restriction}", "restriction"),
		};

		public bool ExistsValidChoice(IEnumerable<GameCard> potentialTargets)
		{
			ComplainIfNotInitialized();
			return listRestrictions.All(r => DoesRestrictionAllowValidChoice(r, potentialTargets));
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("List Restriction:");
			foreach (var r in listRestrictions) sb.AppendLine(r);
			return sb.ToString();
		}
	}
}