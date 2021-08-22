using KompasCore.Effects;

[System.Serializable]
public class TriggerData
{
    public string triggerCondition;
    public TriggerRestriction triggerRestriction;

    public bool optional = false;
    public string blurb = "Trigger";
    public bool showX = false;
}
