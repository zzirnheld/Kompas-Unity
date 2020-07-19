namespace KompasServer.Effects
{
    [System.Serializable]
    public class ChangeNESWSubeffect : ServerSubeffect
    {
        public int nChange = 0;
        public int eChange = 0;
        public int sChange = 0;
        public int wChange = 0;

        public override bool Resolve()
        {
            Target.AddToCharStats(nChange, eChange, sChange, wChange);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}