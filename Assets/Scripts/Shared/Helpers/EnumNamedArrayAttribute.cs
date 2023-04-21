using UnityEngine;

public class EnumNamedArrayAttribute : PropertyAttribute
{
	public string[] names;

	public EnumNamedArrayAttribute(System.Type enumType)
	{
		this.names = System.Enum.GetNames(enumType);
	}
}