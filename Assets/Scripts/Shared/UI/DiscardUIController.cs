using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.UI
{
    public class DiscardUIController : StackableEntitiesController
    {
        public float localXOffset = 2f;
        public float localZOffset = -2f;
        public DiscardController discardController;

        protected override IEnumerable<GameObject> Objects => discardController.Cards.Select(c => c.CardController.gameObject);
        protected override bool ForceExpand => discardController.Cards.Any(c => c == discardController.game.UIController.CardViewController.FocusedCard);

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