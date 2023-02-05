public class Cowpen : ResourceCreator
{
    //public int deadBunnyCount = 0;
    Cowpen() { resource = ResourceType.fertilizer; }
    public override void onClicked()
    {
        //nope
    }
    //public override Tuple<ResourceType, int> collectResources()
    //{
    //    var fertilizerCreated = deadBunnyCount * resourceCount;
    //    deadBunnyCount = 0;
    //    return new Tuple<ResourceType, int>(resource, fertilizerCreated);
    //}

    public override void refresh()
    {
        //deadBunnyCount = 0;
    }

    void Update()
    {

    }
}
