
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasCore.UI
{
    public abstract class StackableGameLocationUIController : StackableEntitiesController
    {
        public float localXOffset = 2f;
        public float localZOffset = -2f;

        protected abstract IGameLocation GameLocation { get; }
        protected IEnumerable<GameCard> Cards => GameLocation.Cards;
        protected BaseCardViewController CardViewController => GameLocation.Game.UIController.CardViewController;

        public override IEnumerable<GameObject> Objects => Cards.Select(c => c.CardController.gameObject);
        protected override bool ForceExpand => Cards.Any(c => c == CardViewController.FocusedCard);

        private void TakeOwnershipOf(GameObject obj)
        {
            obj.transform.parent = transform;
            obj.SetActive(true);
        }

        protected override void ShowCollapsed()
        {
            foreach (var obj in Objects) TakeOwnershipOf(obj);
            base.ShowCollapsed();
        }

        protected override void ShowExpanded()
        {
            int wrapLen = Mathf.CeilToInt(Mathf.Sqrt(Objects.Count()));
            int x = 0, y = 0;
            foreach (var obj in Objects)
            {
                TakeOwnershipOf(obj);
                obj.transform.localPosition = new Vector3(localXOffset * (x + y), 0f, localZOffset * y);

                x = (x + 1) % wrapLen;
                if (x == 0) y++;
            }
        }
    }
}