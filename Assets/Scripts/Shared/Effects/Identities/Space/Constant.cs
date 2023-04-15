namespace KompasCore.Effects.Identities.Spaces
{
    public class Constant : ContextlessLeafIdentityBase<Space>
    {
        public int x;
        public int y;

        protected override Space AbstractItem => (x, y);
    }
}