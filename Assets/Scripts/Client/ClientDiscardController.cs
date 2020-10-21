using KompasCore.GameCore;

namespace KompasClient.GameCore
{
    public class ClientDiscardController : DiscardController
    {
        public ClientGame ClientGame;

        public void OnMouseDown()
        {
            ClientGame.clientUICtrl.StartSearch(Discard.ToArray(), targetingSearch: false);
        }
    }
}