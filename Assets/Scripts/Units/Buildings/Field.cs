using System;

public enum SeedType { none, potato, carrot, radish }

public class Field : ResoruceCreator
{
    private SeedType planted;

    public override void onClicked()
    {
        canvasController.displayFieldOptions();
    }

    public void plant(SeedType seed)
    {
        planted = seed;
    }

    public override Tuple<ResourceType, int> collectResources()
    {
        ResourceType createdPlant = ResourceType.potato; ;
        int amount = resourceCount;
        switch (planted)
        {
            case SeedType.none: amount = 0; break;
            case SeedType.potato: createdPlant = ResourceType.potato; break;
            case SeedType.carrot: createdPlant = ResourceType.carrot; break;
            case SeedType.radish: createdPlant = ResourceType.radish; break;
        }
        return new Tuple<ResourceType, int>(createdPlant, amount);
    }
}
