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
        protected override bool ForceCollapse => toShow.Count() == 0; //Should be collapsed unless searching
        protected override bool ForceExpand => !ForceCollapse;
        protected override IEnumerable<GameCard> Cards => base.Cards.Where(toShow.Contains);
        public GameObject deckCardBackObject;

        //ShowExpanded will update on next Update()
        public void ShowSearching(IEnumerable<GameCard> toShow)
        {
            foreach (var c in toShow) c.CardController.ShownInSearch = true;
            this.toShow = new HashSet<GameCard>(toShow);
            Update();
        }

        public void StopSearching()
        {
            foreach (var c in toShow) c.CardController.ShownInSearch = true;
            toShow = Enumerable.Empty<GameCard>();
        }


        protected override void ShowCollapsed()
        {
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
