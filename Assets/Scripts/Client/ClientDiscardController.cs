using KompasCore.GameCore;
using UnityEngine;

namespace KompasClient.GameCore
{
	public class ClientDiscardController : DiscardController
	{
		public ClientPlayer owner;
		public override Player Owner => owner;
	}
}