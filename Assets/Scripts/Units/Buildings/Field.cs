using System;
using UnityEngine;

public enum SeedType { none, potato, carrot, radish }

public class Field : ResourceCreator
{
    public SeedType planted;
    SpriteRenderer render;
    public Sprite emptySprite = null;

    Field()
    {
        resourceCount = 1;
    }
    public override void onClicked()
    {
        if (gameController.phase != Phase.Build)
        {
            return;
        }
        canvasController.displayFieldOptions(gameController.resources[ResourceType.fertilizer] > 0);
    }

    public void plant(SeedType seed)
    {
        planted = seed;
    }

    public override void refresh()
    {
        if(render == null) render = gameObject.GetComponent<SpriteRenderer>();
        planted = SeedType.none;
        resourceCount = 1;
        render.sprite = emptySprite;
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

    public void fertilize()
    {
        resourceCount += 1;
        gameController.resources[ResourceType.fertilizer] -= 1;
        canvasController.updateResources();
        canvasController.displayFieldOptions(gameController.resources[ResourceType.fertilizer] > 0);
    }
}
