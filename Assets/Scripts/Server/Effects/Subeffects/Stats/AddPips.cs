using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class AddPips : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			PlayerTarget.Pips += Count;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}