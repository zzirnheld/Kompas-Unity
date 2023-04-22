using System;
using System.Linq;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasCore.UI
{
	public class DiscardUIController : StackableGameLocationUIController
	{
		public DiscardController discardController;

		protected override IGameLocation GameLocation => discardController;

        public float bigCircleRadius = 10f;
        public float unitSize = 2f;

        protected override bool Complain => true;

        protected override void Spread()
        {
            var arr = ShownObjects.ToArray();
            for (int objIdx = 0, numLoops = 1; objIdx < arr.Length; objIdx++, numLoops++)
			{
				// radius of the circle whose arc this is is numLoops * unitSize.
				float smallCircleRadius = numLoops * unitSize;

				// an isoceles triangle is formed with side lengths bigCircleRadius, bigCircleRadius, and smallCircleRadius
				// the angle of the arc we want is the matching angles of the isoceles triangle.
				// so via trig, that's given by:
				double arcSpreadRadians = Math.Acos(smallCircleRadius / (2d * bigCircleRadius));
				// note: that angle is off from the line between the smaller and larger circle's centers

				// the arc that we want has a chord of length unitSize.
				// this forms an isoceles triangle with side lengths smallCircleRadius, smallCircleRadius, and unitSize
				// so the angle of the arc we want is given
				double arcSegmentRadians = 2d * Math.Asin(unitSize / (2d * smallCircleRadius));

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