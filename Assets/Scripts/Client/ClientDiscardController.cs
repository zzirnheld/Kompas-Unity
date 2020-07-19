using KompasCore.GameCore;

namespace KompasClient.GameCore
{
    public class ClientDiscardController : DiscardController
    {
        public ClientGame ClientGame;

        public void OnMouseDown()
        {
            if (ClientGame.friendlyDiscardCtrl == this)
                ClientGame.clientNotifier.RequestRehand(GetLastDiscarded());
        }
    }
}