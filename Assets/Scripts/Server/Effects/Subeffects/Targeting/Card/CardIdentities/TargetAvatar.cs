using KompasCore.Effects.Identities.Cards;

namespace KompasServer.Effects.Subeffects
{
	public class TargetAvatar : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new Avatar() { player = new KompasCore.Effects.Identities.Players.TargetIndex() };
			base.Initialize(eff, subeffIndex);
		}
	}
}