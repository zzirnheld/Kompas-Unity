namespace KompasCore.Effects.Identities
{
    namespace GamestateNumberIdentities
    {
        public class Constant : ContextlessLeafIdentityBase<int>
        {
            public static Constant One => new Constant { constant = 1 };

            public int constant;

            protected override int AbstractItem => constant;
        }
    }
}