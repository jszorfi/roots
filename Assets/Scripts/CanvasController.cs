using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    private CanvasGroup potatoSkills;
    private CanvasGroup carrotSkills;
    private CanvasGroup radishSkills;

    private CanvasGroup builderOptions;
    private CanvasGroup shrineOptions;
    private CanvasGroup fieldOptions;

    public TMP_Text wood;
    public TMP_Text plough;
    public TMP_Text fertilizer;
    public TMP_Text potatoSeed;
    public TMP_Text carrotSeed;
    public TMP_Text radishSeed;

    private Dictionary<UnitType, Button> builderButtons = new Dictionary<UnitType, Button>();

    private CanvasGroup displayedGroup;
    private GameController gameController;
    private void Start()
    {
        gameController = GameObject.Find("GameController").gameObject.GetComponent<GameController>();

        var skills = gameObject.transform.Find("Skills").gameObject.transform;
        potatoSkills = skills.Find("PotatoSkills").gameObject.GetComponent<CanvasGroup>();
        carrotSkills = skills.Find("CarrotSkills").gameObject.GetComponent<CanvasGroup>();
        radishSkills = skills.Find("RadishSkills").gameObject.GetComponent<CanvasGroup>();

        var buttons = skills.Find("BuilderOptions").gameObject;
        builderOptions = buttons.GetComponent<CanvasGroup>();
        shrineOptions = skills.Find("ShrineOptions").gameObject.GetComponent<CanvasGroup>();
        fieldOptions = skills.Find("FieldOptions").gameObject.GetComponent<CanvasGroup>();

        builderButtons[UnitType.Field] = buttons.transform.GetChild(0).GetComponent<Button>();
        builderButtons[UnitType.Shed] = buttons.transform.GetChild(1).GetComponent<Button>();
        builderButtons[UnitType.Woodmill] = buttons.transform.GetChild(2).GetComponent<Button>();
        builderButtons[UnitType.Potato] = buttons.transform.GetChild(3).GetComponent<Button>();
        builderButtons[UnitType.Carrot] = buttons.transform.GetChild(4).GetComponent<Button>();
        builderButtons[UnitType.Radish] = buttons.transform.GetChild(5).GetComponent<Button>();

        foreach (var b in builderButtons)
        {
            b.Value.onClick.AddListener(delegate { gameController.placeUnit(b.Key); });
        }

        foreach (Transform skill in skills)
        {
            skill.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        }
        updateResources();
    }

    public void clear()
    {
        if (displayedGroup != null)
        {
            displayedGroup.alpha = 0;
            displayedGroup = null;
        }
    }


    public void updateResources()
    {
        var res = gameController.Resources;
        wood.text = res[ResourceType.wood].ToString();
        plough.text = res[ResourceType.plough].ToString();
        fertilizer.text = res[ResourceType.fertilizer].ToString();
        potatoSeed.text = res[ResourceType.potato].ToString();
        carrotSeed.text = res[ResourceType.carrot].ToString();
        radishSeed.text = res[ResourceType.radish].ToString();

        foreach (var button in builderButtons)
        {
            button.Value.interactable = gameController.haveEnoughResourceForUnit(button.Key);
        }
    }

    private void changeDisplayedGroup(CanvasGroup groupToDisplay)
    {
        if (displayedGroup != null)
            displayedGroup.alpha = 0;
        groupToDisplay.alpha = 1;
        displayedGroup = groupToDisplay;
    }

    public void displayPotatoSkills()
    {
        changeDisplayedGroup(potatoSkills);
    }
    public void displayCarrotSkills()
    {
        changeDisplayedGroup(carrotSkills);
    }
    public void displayRadishSkills()
    {
        changeDisplayedGroup(radishSkills);
    }
    public void displayBuilderOptions()
    {
        changeDisplayedGroup(builderOptions);
    }
    public void displayShrineOptions()
    {
        changeDisplayedGroup(shrineOptions);
    }
    public void displayFieldOptions()
    {
        changeDisplayedGroup(fieldOptions);
    }
}
