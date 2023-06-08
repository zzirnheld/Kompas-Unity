using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
	public class ThisCardNow : ContextlessLeafCardIdentityBase
	{
		protected override GameCardBase AbstractItem => InitializationContext.source;
	}
}