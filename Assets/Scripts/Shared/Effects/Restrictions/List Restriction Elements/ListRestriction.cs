using System.Collections.Generic;
using KompasCore.Cards;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions
{
	/// <summary>
	/// Special, because many list restrictions have to be aware of characteristics of each other when deciding whether it's even possible to select a valid target.
	/// That is, deciding whether a valid target exists is more complicated than checking whether any one option fulfills the requirements.
	/// For instance, a "can pay costs" restriction must also take into account that you can't just pay for 4 copies of a 1 cost, if the "distinct names" requirement is also specified.
	/// </summary>
	public interface IListRestriction : IRestriction<IEnumerable<GameCardBase>>
	{
		public static IListRestriction SingleElement { get; } = ConstantCount(1);

		public static IListRestriction ConstantCount(int count)
		{
			var bound = new Identities.Numbers.Constant() { constant = count };
			return new ListRestrictionElements.AllOf()
			{
				elements = {
					new ListRestrictionElements.Minimum() { bound = bound, stashedBound = count }, //TODO replace setting stashedBound here with prepare for sending func?
					new ListRestrictionElements.Maximum() { bound = bound, stashedBound = count }
				}
			};
		} 

		public bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context);

		public bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context);

		/// <summary>
		/// If you don't specifically want to constrain the list (i.e. by deduplicating on a particular value),
		/// have this just return the source sequence.
		/// This will be overridden by restrictions like "distinct names" and "distinct costs"
		/// </summary>
		public IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options);

		/// <summary>
        /// Get the current minimum for this resolution context.
        /// If null is passed in, the stashed minimum is returned instead.
        /// </summary>
		public int GetMinimum(IResolutionContext context);
		/// <summary>
        /// Get the current maximum for this resolution context.
        /// If null is passed in, the stashed maximum is returned instead.
        /// </summary>
		public int GetMaximum(IResolutionContext context);
	}

	public static class ListRestrictionExtensions
	{
		public static bool HaveEnough(this IListRestriction restriction, int currCount)
			=> restriction.GetMinimum(null) <= currCount && currCount <= restriction.GetMaximum(null);

		public static int GetStashedMinimum(this IListRestriction restriction) => restriction.GetMinimum(null);
		public static int GetStashedMaximum(this IListRestriction restriction) => restriction.GetMaximum(null);
	}

	namespace ListRestrictionElements
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
				foreach (var elem in elements) localOptions = elem.Deduplicate(localOptions);
				return localOptions;
			}

			public int GetMinimum(IResolutionContext context)
				=> elements
					.Select(elem => elem.GetMinimum(context))
					.DefaultIfEmpty(0)
					.Max();

			public int GetMaximum(IResolutionContext context)
				=> elements
					.Select(elem => elem.GetMaximum(context))
					.DefaultIfEmpty(int.MaxValue)
					.Min();
		}

		public abstract class ListRestrictionElementBase : RestrictionBase<IEnumerable<GameCardBase>>, IListRestriction
		{
			public virtual int GetMinimum(IResolutionContext context) => 0;
			public virtual int GetMaximum(IResolutionContext context) => int.MaxValue;

			public virtual IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options) => options; //No dedup, by default

			public virtual bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context) => IsValid(options, context);

			public abstract bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context);
		}
	}
}