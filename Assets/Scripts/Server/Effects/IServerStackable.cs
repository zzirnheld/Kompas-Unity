using KompasServer.GameCore;
using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public interface IServerStackable : IStackable
    {
        Task StartResolution(ResolutionContext context);

        ServerPlayer ServerController { get; }
    }
}