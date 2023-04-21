using KompasClient.GameCore;
using KompasCore.GameCore;

public class ClientHandController : HandController
{
	public ClientPlayer owner;
	public override Player Owner => owner;

	public virtual void IncrementHand()
		=> throw new System.NotImplementedException("Can't increment hand of the friendly player!");

	public virtual void DecrementHand()
		=> throw new System.NotImplementedException("Can't decrement hand of the friendly player!");
}
