using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace KompasCore.Effects
{
	public interface IContextInitializeable
	{
		public void Initialize(EffectInitializationContext initializationContext);

		/// <summary>
        /// This is separate from the rest of initialization because it can happen at an arbitrary later time,
        /// i.e. when a partial keyword subeffect is expanded and now needs to affect other subeffects
        /// </summary>
        /// <param name="increment">How much the subeffect indices need to be adjusted by</param>
        /// <param name="startingAtIndex">A threshold index for which ones need to be adjusted
        /// (i.e. the starting index of the newly inserted subeffects)</param>
		public void AdjustSubeffectIndices(int increment, int startingAtIndex = 0);
	}

	/// <summary>
	/// Base class for initializeable things, like restrictions or identities.
    /// Since these are all being loaded from JSON, make sure to mark any relevant fields as [JsonProperty]
	/// </summary>
	[DataContract]
	public abstract class ContextInitializeableBase : IContextInitializeable
	{
		protected bool Initialized { get; private set; }

		protected EffectInitializationContext InitializationContext { get; private set; }

		protected virtual IEnumerable<IInitializationRequirement> InitializationRequirements => Enumerable.Empty<IInitializationRequirement>();

		public virtual void Initialize(EffectInitializationContext initializationContext)
		{
			if (Initialized)
			{
				Debug.Log($"Was already initialized with {InitializationContext}, but now being initialized with {initializationContext}");
			}
			InitializationContext = initializationContext;

			Initialized = true;
		}

		protected virtual void ComplainIfNotInitialized()
		{
			if (!Initialized) throw new NotImplementedException($"You forgot to initialize a {GetType()}!\n{this}");
		}
		
		protected static bool AllNull(params object[] objs) => objs.All(o => o == null);
		protected static bool MultipleNonNull(params object[] objs) => objs.Count(o => o != null) > 1;

		public override string ToString()
		{
			return GetType().ToString();
		}

		public virtual void AdjustSubeffectIndices(int increment, int startingAtIndex = 0) { }

		public static void AdjustSubeffectIndices(int[] subeffectIndices, int increment, int startingAtIndex)
		{
			if (subeffectIndices == null) return;

			for (int i = 0; i < subeffectIndices.Length; i++)
				if (subeffectIndices[i] >= startingAtIndex)
					subeffectIndices[i] += increment;
		}
	}

	public interface IInitializationRequirement
	{
		public bool Validate(EffectInitializationContext initializationContext);
	}

	public class SubeffectInitializationRequirement : IInitializationRequirement
	{
		public bool Validate(EffectInitializationContext initializationContext)
		{
			if (initializationContext.subeffect == null) throw new ArgumentNullException($"{GetType()} must be initialized by/with a Subeffect");

			return true;
		}
	}

	public class EffectInitializationRequirement : IInitializationRequirement
	{
		public bool Validate(EffectInitializationContext initializationContext)
		{
			if (initializationContext.effect == null) throw new ArgumentNullException($"{GetType()} must be initialized by/with an Effect");

			return true;
		}
	}
}