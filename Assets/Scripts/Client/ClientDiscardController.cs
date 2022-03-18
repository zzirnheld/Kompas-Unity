using KompasCore.GameCore;
using UnityEngine;

namespace KompasClient.GameCore
{
    public class ClientDiscardController : DiscardController
    {
        private const int WrapLen = 5;

        public ClientGame ClientGame;

        public void OnMouseDown()
        {
            ClientGame.searchCtrl.StartSearch(Discard.ToArray(), null, targetingSearch: false);
        }

        public override void SpreadOutCards()
        {
            int x = 0, y = 0;
            for (int i = 0; i < Discard.Count; i++)
            {
                Discard[i].transform.localPosition = new Vector3(2f * (x + y), 0f, -2f * y);

                x = (x + 1) % WrapLen;
                if (x == 0) y++;
            }
        }
    }
}