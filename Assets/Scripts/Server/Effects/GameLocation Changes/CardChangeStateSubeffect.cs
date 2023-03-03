namespace KompasServer.Effects
{
    /// <summary>
    /// Moves cards between discard/field/etc
    /// </summary>
    public abstract class CardChangeStateSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.Location == destination;

        protected abstract CardLocation destination { get; }

    }
}