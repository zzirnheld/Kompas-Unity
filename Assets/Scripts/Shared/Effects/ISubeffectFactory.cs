public interface ISubeffectFactory
{
    Subeffect FromJson(SubeffectType subeffectType, string json, Effect parent, int subeffIndex);
}
