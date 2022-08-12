namespace KompasCore.Effects.Identities
{
    namespace GamestateNumberIdentities
    {
        public class Constant : NoActivationContextIdentityBase<int>
        {
            public static Constant ONE => new Constant { constant = 1 };

            public int constant;

            protected override int AbstractItem => constant;
        }
    }
}