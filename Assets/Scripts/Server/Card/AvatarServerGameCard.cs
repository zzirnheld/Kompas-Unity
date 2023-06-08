using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasServer.Effects;
using KompasServer.GameCore;
using System;

namespace KompasServer.Cards
{
	public class AvatarServerGameCard : ServerGameCard
	{
		public override bool Summoned => false;
		public override bool IsAvatar => true;

		public override bool Remove(IStackable stackSrc = null)
		{
			if (Location == CardLocation.Nowhere) return true;
			var corner = Space.AvatarCornerFor(ControllerIndex);
			var unfortunate = Game.BoardController.GetCardAt(corner);
			if (unfortunate != null && unfortunate != this && !unfortunate.IsAvatar)
				unfortunate.Annihilate(stackSrc);
			Move(to: corner, normalMove: false, stackSrc: stackSrc);
			return false;
		}

		private void Move(Space to, bool normalMove, IStackable stackSrc)
		{
			throw new NotImplementedException();
		}

		public override void SetE(int e, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetE(e, stackSrc, onlyStatBeingSet);
			LoseIfDead();
		}

		public void LoseIfDead()
		{
			if (E <= 0) ServerGame.Lose(ControllerIndex);
		}

		public AvatarServerGameCard(ServerSerializableCard card, int id, ServerCardController serverCardController, ServerPlayer owner, ServerEffect[] effects)
			: base(card, id, serverCardController, owner, effects)
		{

		}

	}
}
