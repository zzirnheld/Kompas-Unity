using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Exceptions;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities
{
	/// <summary>
	/// Can be used whether or not the caller does or doesn't care about an ActivationContext)
	/// </summary>
	public interface IIdentity<ReturnType> : IContextInitializeable
	{
		public ReturnType From(IResolutionContext context, IResolutionContext secondaryContext);
	}

	public static class IdentityExtensions
	{
		public static ReturnType From<ReturnType>(this IIdentity<ReturnType> identity,
			TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
				=> identity.From(IResolutionContext.Dummy(triggeringContext), resolutionContext);

		public static ReturnType From<ReturnType>(this IIdentity<ReturnType> identity,
			IResolutionContext resolutionContext)
				=> identity.From(resolutionContext, default);
	}

	/// <summary>
	/// An identity that needs context, either for itself or to pass on to its children.
	/// </summary>
	public abstract class ContextualParentIdentityBase<ReturnType> : ContextInitializeableBase,
		IIdentity<ReturnType>
	{
		[JsonProperty]
		public bool secondaryContext = false;

		protected IResolutionContext ContextToConsider(IResolutionContext context, IResolutionContext secondaryContext)
			=> this.secondaryContext ? secondaryContext : context;

		/// <summary>
		/// Override this one if you need to pass on BOTH contexts.
		/// </summary>
		protected abstract ReturnType AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext);

		/// <summary>
		/// Gets the abstract stackable from the first one, that only knows about the context to consider,
		/// then the one that knows about both contexts if the first one came up empty.
		/// </summary>
		public ReturnType From(IResolutionContext context, IResolutionContext secondaryContext)
		{
			ComplainIfNotInitialized();

			return AbstractItemFrom(context, secondaryContext);
		}

		public ReturnType Item => From(InitializationContext.effect.ResolutionContext, default);

		protected Attack GetAttack(TriggeringEventContext effectContext)
		{
			if (effectContext.stackableEvent is Attack eventAttack) return eventAttack;
			if (effectContext.stackableCause is Attack causeAttack) return causeAttack;
			else throw new NullCardException("Stackable event wasn't an attack!");
		}
	}

	public abstract class EffectContextualLeafIdentityBase<ReturnType> : ContextualParentIdentityBase<ReturnType>
	{

		/// <summary>
		/// Gets the abstract stackable from the first one, that only knows about the context to consider,
		/// then the one that knows about both contexts if the first one came up empty.
		/// </summary>
		protected override ReturnType AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var contextToConsider = ContextToConsider(context, secondaryContext);
			return AbstractItemFrom(contextToConsider);
		}

		protected abstract ReturnType AbstractItemFrom(IResolutionContext toConsider);

	}

	public abstract class TriggerContextualLeafIdentityBase<ReturnType> : EffectContextualLeafIdentityBase<ReturnType>
	{
		protected override ReturnType AbstractItemFrom(IResolutionContext toConsider)
			=> AbstractItemFrom(toConsider.TriggerContext);

		/// <summary>
		/// Override this one if you ONLY need to know about the context you should actually be considering
		/// </summary>
		/// <param name="contextToConsider">The ActivationContext you actually should be considering.</param>
		protected abstract ReturnType AbstractItemFrom(TriggeringEventContext contextToConsider);
	}

	public abstract class ContextlessLeafIdentityBase<ReturnType> : ContextInitializeableBase,
		IIdentity<ReturnType>
	{
		protected abstract ReturnType AbstractItem { get; }

		public ReturnType Item
		{
			get
			{
				ComplainIfNotInitialized();
				return AbstractItem;
			}
		}

		public ReturnType From(IResolutionContext context, IResolutionContext secondaryContext) => Item;
	}

	public abstract class ContextlessLeafCardIdentityBase : ContextlessLeafIdentityBase<GameCardBase>, IIdentity<Space>, IIdentity<IReadOnlyCollection<GameCardBase>>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;

		IReadOnlyCollection<GameCardBase> IIdentity<IReadOnlyCollection<GameCardBase>>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> new [] { Item };
	}

	public abstract class TriggerContextualCardIdentityBase : TriggerContextualLeafIdentityBase<GameCardBase>, IIdentity<Space>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;
	}

	public abstract class EffectContextualCardIdentityBase : EffectContextualLeafIdentityBase<GameCardBase>, IIdentity<Space>
	{
		Space IIdentity<Space>.From(IResolutionContext context, IResolutionContext secondaryContext)
			=> Item.Position;
	}
}