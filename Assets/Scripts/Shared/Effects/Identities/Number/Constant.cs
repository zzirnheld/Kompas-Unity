namespace KompasCore.Effects.Identities.Numbers
{
	public class Constant : ContextlessLeafIdentityBase<int>
	{
		public static Constant One => new Constant { constant = 1 };
		public static Constant Zero => new Constant { constant = 0 };

		public int constant;

		protected override int AbstractItem => constant;
	}
}