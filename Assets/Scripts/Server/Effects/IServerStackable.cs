using KompasServer.GameCore;
using KompasCore.Effects;

namespace KompasServer.Effects
{
    public interface IServerStackable : IStackable
    {
        void StartResolution(ActivationContext context);

        ServerPlayer ServerController { get; }
    }
}