using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
	public class ServerTrigger : Trigger
	{
		public ServerEffect serverEffect;

		public override GameCard Source => serverEffect.Source;
		public override Effect Effect
		{
			get => serverEffect;
			protected set => serverEffect = value as ServerEffect;
		}

		private bool responded = false;
		/// <summary>
		/// Represents whether this trigger, if optional, has been responded to (to accept or decline).
        /// If not optional, is true.
		/// </summary>
		public bool Responded
		{
			get => !Optional || responded;
			set => responded = value;
		}

		private bool confirmed = false;
		/// <summary>
		/// Represents whether this trigger, if optional, was chosen to be used or not.
		/// </summary>
		public bool Confirmed
		{
			get => !Optional || confirmed;
			set => confirmed = value;
		}

		/// <summary>
		/// Represents the order this trigger has been given, amongst other simultaneously triggered triggers.
		/// </summary>
		private int order = -1;
		public int Order
		{
			get => order;
			set
			{
				order = value;
				Responded = true;
			}
		}
		public bool Ordered => order != -1;

		public ServerTrigger(TriggerData triggerData, ServerEffect parent) : base(triggerData, parent)
		{
			if (!TriggerConditions.Contains(triggerData.triggerCondition))
				throw new System.ArgumentNullException("triggerCondition", $"invalid trigger condition for effect of {parent.Source.CardName}");

			parent.serverGame.effectsController.RegisterTrigger(TriggerCondition, this);
		}

		/// <summary>
		/// Checks all relevant trigger restrictions, and whether the card is negated
		/// </summary>
		/// <param name="cardTriggerer">The card that triggered this, if any.</param>
		/// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
		/// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
		/// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
		public bool ValidForTriggeringContext(TriggeringEventContext context) => !Source.Negated && TriggerRestriction.IsValid(context, default);

		/// <summary>
		/// Rechecks any trigger restrictions that might have changed between the trigger triggering and being ordered.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public bool StillValidForContext(TriggeringEventContext context)
			=> TriggerRestriction is KompasCore.Effects.Restrictions.TriggerRestrictionElements.AllOf allOf
			&& allOf.IsStillValidTriggeringContext(context);

		/// <summary>
		/// Resets Confirmed and Responded, for the next time this effect might be triggered
		/// </summary>
		public void ResetConfirmation()
		{
			Responded = false;
			Confirmed = false;
			order = -1;
		}

		public async Task Ask(int x)
		{
			Confirmed = await serverEffect.ServerController.awaiter.GetOptionalTriggerChoice(this, x, TriggerData.showX);
			Responded = true;
		}
	}
}