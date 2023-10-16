using FMODPlus;

[System.Serializable]
public class EventRefAtKey 
{
    public EventReferenceByKey[] EventRefList;

    public int Length => EventRefList.Length;
    public int Count => EventRefList.Length;
}
