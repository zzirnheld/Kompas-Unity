using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasClient.GameCore
{
    public class ClientDiscardController : DiscardController
    {
        public ClientGame ClientGame;

        public void OnMouseDown()
        {
            ClientGame.searchCtrl.StartSearch(Discard.ToArray(), null, targetingSearch: false);
        }
    }
}