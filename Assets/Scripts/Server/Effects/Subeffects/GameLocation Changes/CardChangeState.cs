namespace KompasServer.Effects.Subeffects
{
    /// <summary>
    /// Moves cards between discard/field/etc
    /// </summary>
    public abstract class CardChangeState : ServerSubeffect
    {
        public override bool IsImpossible() => CardTarget == null || CardTarget.Location == destination;

        protected abstract CardLocation destination { get; }

    }
}