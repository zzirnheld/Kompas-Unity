using KompasCore.GameCore;

namespace KompasClient.GameCore
{
    public class ClientAnnihilationController : AnnihilationController
    {
        public ClientPlayer owner;

        public override Player Owner => owner;
    }
}