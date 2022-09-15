using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public class ImageOnlyCardViewController : BaseCardViewController
    {
        public Image image;

        protected override void DisplayCardImage()
        {
            throw new System.NotImplementedException();
        }

        protected override void DisplayCardNumericStats() { }

        protected override void DisplayCardRulesText() { }
    }
}