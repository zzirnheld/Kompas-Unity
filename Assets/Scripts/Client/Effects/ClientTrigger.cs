using KompasCore.Cards;
using KompasCore.Effects;

namespace KompasClient.Effects
{
	public class ClientTrigger : Trigger
	{
		public ClientEffect ClientEffect { get; private set; }

		public override GameCard Source => ClientEffect.Source;
		public override Effect Effect
		{
			get => ClientEffect;
			protected set => ClientEffect = value as ClientEffect;
		}


		public ClientTrigger(TriggerData triggerData, ClientEffect parent) : base(triggerData, parent) { }
	}
}