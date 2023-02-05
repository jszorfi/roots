using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Building
{
    IntroController introController;
    SpriteRenderer dipslayedSprite;
    public List<Sprite> sprites;

    public new void Start()
    {
        base.Start();
        introController = gameController.gameObject.GetComponent<IntroController>();
        dipslayedSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    void OnDestroy()
    {
        if (!gameController.won)
            introController.Defeat();
    }
    public override void onClicked()
    {
        //canvasController.displayShrineOptions();
    }

    public void setPhase(int phase)
    {
        dipslayedSprite.sprite = sprites[phase];
    }
    public override void refresh()
    {
        //nope
    }
}
