namespace KompasClient.UI
{

	public class ClientUIController : PlayerSideUIController
	{
		public ClientSettingsUIController clientUISettingsController;

		public override void ApplySettings(ClientSettings clientSettings)
		{
			base.ApplySettings(clientSettings);
			connectionUIController.ApplySettings(clientSettings);
		}
	}
}