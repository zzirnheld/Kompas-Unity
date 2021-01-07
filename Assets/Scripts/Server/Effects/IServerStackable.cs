using KompasServer.GameCore;
using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public interface IServerStackable : IStackable
    {
        Task StartResolution(ActivationContext context);

        ServerPlayer ServerController { get; }
    }
}