
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasCore.UI
{
    /// <summary>
    /// A version of the stacked elements thing that shows a grid of elements
    /// </summary>
    public abstract class StackableGameLocationUIController : StackableEntitiesController
    {
        [Header("OVERRIDE: Grid offsets")]
        [Tooltip("X offset for objects in the grid pattern. Overrides the behavior of the constant vector offset specified in the base class")]
        public float localXOffset = 2f;
        [Tooltip("Z offset for objects in the grid pattern. Overrides the behavior of the constant vector offset specified in the base class")]
        public float localZOffset = -2f;

        protected abstract IGameLocation GameLocation { get; }
        protected virtual IEnumerable<GameCard> Cards => GameLocation.Cards;
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
            gameObject.SetActive(true);
            foreach (var obj in Objects) TakeOwnershipOf(obj);
            base.ShowCollapsed();
        }

        protected override void ShowExpanded()
        {
            foreach (var obj in Objects) TakeOwnershipOf(obj);
            gameObject.SetActive(true);
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