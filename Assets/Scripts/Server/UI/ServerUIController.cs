using KompasCore.UI;

namespace KompasServer.UI
{
    public class ServerUIController : UIController
    {
        public override bool AllowDragging => false;
        public SidebarCardViewController cardViewController;
        public override SidebarCardViewController CardViewController => cardViewController;
    }
}