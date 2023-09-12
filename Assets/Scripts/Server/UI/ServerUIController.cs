using KompasCore.UI;

namespace KompasServer.UI
{
	public class ServerUIController : UIController
	{
		public SidebarCardViewController cardViewController;
		public override SidebarCardViewController CardViewController => cardViewController;

		public CardViewReminderTextParentController reminderTextParentController;
		public override IReminderTextParentController ReminderTextParentUIController => reminderTextParentController;
	}
}