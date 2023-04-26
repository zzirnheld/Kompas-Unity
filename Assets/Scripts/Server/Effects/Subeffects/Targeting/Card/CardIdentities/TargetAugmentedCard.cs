using KompasCore.Effects.Identities.Cards;

namespace KompasServer.Effects.Subeffects
{
	public class TargetAugmentedCard : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new AugmentedCard() { ofThisCard = new ThisCardNow() };
			base.Initialize(eff, subeffIndex);
		}
	}
}