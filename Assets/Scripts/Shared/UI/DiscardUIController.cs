using System;
using System.Linq;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasCore.UI
{
    public class DiscardUIController : CircleStackableGameLocationUIController
    {
        public DiscardController discardController;

        protected override IGameLocation GameLocation => discardController;

        protected override bool Complain => true;
    }
}