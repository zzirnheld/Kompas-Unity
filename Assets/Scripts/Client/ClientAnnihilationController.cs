using KompasCore.GameCore;
using KompasCore.UI;

namespace KompasClient.GameCore
{
    public class ClientAnnihilationController : AnnihilationController
    {
        public ClientPlayer owner;

        public override Player Owner => owner;
    }
}