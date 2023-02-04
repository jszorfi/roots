using System;

public enum ResourceType { wood, plough, fertilizer, potato, carrot, radish }

public abstract class ResourceCreator : Building
{
    protected ResourceType resource;
    protected int resourceCount;

    public virtual Tuple<ResourceType, int> collectResources()
    {
        return new Tuple<ResourceType, int>(resource, resourceCount);
    }

}
