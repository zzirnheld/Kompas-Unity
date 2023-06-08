namespace KompasCore.Effects
{
	public class SerializableEffect
	{
		//array of strings that will get deserialized
		public string[] subeffects;

		//used for knowing what trigger to deserialize as
		public string triggerCondition;

		//string to be deserialized
		public string trigger;

		public IActivationRestriction activationRestriction;

		public string blurb = "Effect";
	}
}