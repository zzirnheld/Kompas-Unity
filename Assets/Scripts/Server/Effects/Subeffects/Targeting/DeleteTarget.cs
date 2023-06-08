using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class DeleteTarget : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			RemoveTarget();
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}