namespace KompasClient.Cards
{
    public class ClientDummyCardController : ClientCardController
    {
        public override void ShowForCardType(char cardType, bool zoomed) { }

        public override void SetImage(string cardFileName, bool zoomed) { }

        public override void ShowValidTarget(bool valid = true) { }
        public override void ShowCurrentTarget(bool current = true) { }
        public override void ShowPrimaryOfStackable(bool show = true) { }
        public override void ShowSecondaryOfStackable(bool show = true) { }
        public override void ShowUniqueCopy(bool copy = true) { }
        public override void SetPhysicalLocation(CardLocation location) { }
    }
}