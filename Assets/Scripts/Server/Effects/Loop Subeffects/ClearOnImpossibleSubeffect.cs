namespace KompasServer.Effects
{
    /// <summary>
    /// Removes any effect currently set to trigger if an effect is declared impossible.
    /// </summary>
    public class ClearOnImpossibleSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.OnImpossible = null;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}