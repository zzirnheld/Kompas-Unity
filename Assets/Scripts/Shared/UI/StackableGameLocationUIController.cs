
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
        [Tooltip("X offset for objects in the grid pattern, multiplied by the column the object is in. Overrides the behavior of the constant vector offset specified in the base class")]
        public float localXOffset = 2f; //By column
        [Tooltip("X offset for objects in the grid pattern, multiplied by the row the object is in")]
        public float localXOffsetByRow = 0f;
        [Tooltip("Z offset for objects in the grid pattern. Overrides the behavior of the constant vector offset specified in the base class")]
        public float localZOffset = -2f;

        protected abstract IGameLocation GameLocation { get; }
        protected virtual IEnumerable<GameCard> Cards => GameLocation.Cards;
        protected BaseCardViewController CardViewController => GameLocation.Game.UIController.CardViewController;

        public override IEnumerable<GameObject> ShownObjects => Cards.Select(c => c.CardController.gameObject);
        protected override bool ForceExpand => Cards.Any(c => c == CardViewController.FocusedCard);

        private void TakeOwnershipOf(GameObject obj)
        {
            //Debug.Log($"{name} taking ownership of {obj}");
            if (obj.transform.parent != transform) Debug.Log($"{name} Newly taking ownership of {obj}");
            obj.transform.parent = transform;
            obj.SetActive(true);
        }

        protected override void ShowCollapsed()
        {
            TakeOwnership();
            gameObject.SetActive(true);
            base.ShowCollapsed();
        }

        protected virtual int WrapLen(int objCount) => Mathf.CeilToInt(Mathf.Sqrt(objCount));

        protected override void ShowExpanded()
        {
            TakeOwnership();
            gameObject.SetActive(true);
            int wrapLen = WrapLen(ShownObjects.Count());
            int x = 0, y = 0;
            foreach (var obj in ShownObjects)
            {
                TakeOwnershipOf(obj);
                obj.transform.localPosition = new Vector3(localXOffset * (x + y) + localXOffsetByRow * y, 0f, localZOffset * y);

                x = (x + 1) % wrapLen;
                if (x == 0) y++;
            }
        }

        protected void TakeOwnership()
        {
            foreach (var obj in GameLocation.Cards.Select(c => c.CardController.gameObject)) TakeOwnershipOf(obj);
        }
    }
}