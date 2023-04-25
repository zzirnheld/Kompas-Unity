
using System;
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

        protected virtual bool Complain => false;

        protected void TakeOwnershipOf(GameObject obj)
		{
			//Debug.Log($"{name} taking ownership of {obj}");
			if (obj.transform.parent != transform) Debug.Log($"{name} Newly taking ownership of {obj}");
			if (Complain && !ShownObjects.Contains(obj)) Debug.LogWarning($"Taking ownership of object not shown");
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
            Spread();
        }

		protected virtual void Spread()
		{
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

	public abstract class CircleStackableGameLocationUIController : StackableGameLocationUIController
	{
        public float bigCircleRadius = 10f;
        public float unitSize = 2f;

        public float bufferBetweenArcsMultiplier = 1.5f;

        protected override void Spread()
        {
			if (unitSize == 0f || bufferBetweenArcsMultiplier == 0f || bigCircleRadius == 0f)
			{
                Debug.LogError($"Unit size {unitSize} or buffer {bufferBetweenArcsMultiplier} or big circle radius {bigCircleRadius} is 0 for {GetType()}, so invalid. Base spread fallback");
                base.Spread();
                return;
            }

            var arr = ShownObjects.ToArray();
            for (int objIdx = 0, numLoops = 1; objIdx < arr.Length; objIdx++, numLoops++)
			{
				// radius of the circle whose arc this is is numLoops * unitSize.
				float smallCircleRadius = numLoops * unitSize * bufferBetweenArcsMultiplier;
				if (smallCircleRadius == 0f)
				{
                	Debug.LogError($"Unit size {unitSize} or buffer {bufferBetweenArcsMultiplier} is 0 for {GetType()}, so small circle radius is {smallCircleRadius}, so invalid. Base spread fallback");
                    base.Spread();
                    return;
                }

				// an isoceles triangle is formed with side lengths bigCircleRadius, bigCircleRadius, and smallCircleRadius
				// the angle of the arc we want is the matching angles of the isoceles triangle.
				// so via trig, that's given by:
				double arcSpreadRadians = Math.Acos(smallCircleRadius / (2f * bigCircleRadius));
				// note: that angle is off from the line between the smaller and larger circle's centers

				// the arc that we want has a chord of length unitSize.
				// this forms an isoceles triangle with side lengths smallCircleRadius, smallCircleRadius, and unitSize
				// so the angle of the arc we want is given
				double arcSegmentRadians = 2d * Math.Asin(unitSize / (2f * smallCircleRadius));

				if (arcSpreadRadians == double.NaN || arcSegmentRadians == double.NaN)
				{
                    Debug.LogError($"Radians are NaN! One of those was invalid {arcSpreadRadians} or {arcSegmentRadians}");
                    base.Spread();
                    return;
                }

				//to get the number of cards we can fit on that arc, we need to take the floor of the arc spread / arc segment angles
				int cardsOnArc = (int)((2d * arcSpreadRadians) / arcSegmentRadians);

                double arcSegmentStaggeredRadians = (2d * arcSpreadRadians) / (double)cardsOnArc;

                Debug.Log($"Radius {smallCircleRadius}, arc spread rads {arcSpreadRadians}, segment is {arcSegmentRadians}. staggered mult is {arcSegmentStaggeredRadians} can fit {cardsOnArc}");

                Vector3 radiusBase = smallCircleRadius * Vector3.back;
				//<= here because I want it to be symmetrical, and otherwise it doesn't go to the max of the other side
				for (int i = 0; i <= cardsOnArc && objIdx < arr.Length; i++, objIdx++)
				{
					var obj = arr[objIdx];
					TakeOwnershipOf(obj);
					//angle from the vertical for this next item to place
					double angleFromVertical = arcSpreadRadians - ((double)i * arcSegmentStaggeredRadians);
                	Debug.Log($"Placing {obj}, at {angleFromVertical}");
					Vector3 offset = Quaternion.Euler(0, (float)(angleFromVertical * UnityEngine.Mathf.Rad2Deg), 0) * radiusBase;
					obj.transform.localPosition = offset;
				}
                objIdx--;
            }
        }
    }
}