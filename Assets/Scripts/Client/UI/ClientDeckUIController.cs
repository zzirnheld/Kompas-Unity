using System.Collections.Generic;
using System.Linq;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;

namespace KompasClient.UI
{
	public class ClientDeckUIController : StackableGameLocationUIController
	{
		public ClientDeckController deckController;

		protected override IGameLocation GameLocation => deckController;

		private IEnumerable<GameCard> toShow = Enumerable.Empty<GameCard>();
		protected override bool ForceCollapse => toShow.Where(c => c.Location == CardLocation.Deck).Count() == 0; //Should be collapsed unless searching
		protected override bool ForceExpand => !ForceCollapse;
		protected override IEnumerable<GameCard> Cards => base.Cards.Where(toShow.Contains);
		public GameObject deckCardBackObject;

		//ShowExpanded will update on next Update()
		public void ShowSearching(IEnumerable<GameCard> toShow)
		{
			Debug.Log($"Showing searching {string.Join(", ", toShow.Select(c => c.CardName))}");
			foreach (var c in toShow) c.CardController.ShownInSearch = true;
			this.toShow = new HashSet<GameCard>(toShow);
			Update();
		}

		public void StopSearching()
		{
			Debug.Log($"Stopping searching {string.Join(", ", toShow.Select(c => c.CardName))}");
			foreach (var c in toShow) c.CardController.ShownInSearch = false;
			toShow = Enumerable.Empty<GameCard>();
		}

		protected override int WrapLen(int objCount) => 4;

		protected override void ShowCollapsed()
		{
			//Debug.Log($"Showing collapsed! Shwoing {Cards}")
			TakeOwnership();
			gameObject.SetActive(false);
			deckCardBackObject.SetActive(true);
		}

		protected override void ShowExpanded()
		{
			base.ShowExpanded();
			gameObject.SetActive(true);
			deckCardBackObject.SetActive(false);
		}
	}
}
