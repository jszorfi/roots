using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

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


    [HideInInspector]
    public CanvasGroup displayedGroup;
    public GameController gameController;
    public MapController mapController;
    public Button finishTurn;

    private void Start()
    {
        gameController = GameObject.Find("GameController").gameObject.GetComponent<GameController>();
        mapController = GameObject.Find("Tilemap").GetComponent<MapController>();
        finishTurn = GameObject.Find("FinishTurn").GetComponent<Button>();

        var skills = gameObject.transform.Find("Skills").gameObject.transform;
        var potatoSkills2 = skills.Find("PotatoSkills").gameObject;
        potatoSkills = potatoSkills2.GetComponent<CanvasGroup>();
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

        potatoSkills2.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { mapController.Attack(); });

        foreach (var b in builderButtons)
        {
            b.Value.onClick.AddListener(delegate { gameController.placeUnit(b.Key); });
        }

        foreach (Transform skill in skills)
        {
            skill.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            skill.gameObject.GetComponent<CanvasGroup>().interactable = false;
            skill.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        updateResources();
    }

    public void clear()
    {
        if (displayedGroup != null)
        {
            displayedGroup.alpha = 0;
            displayedGroup.interactable = false;
            displayedGroup.blocksRaycasts = false;
            displayedGroup = null;
        }
    }

    public void changeButtonState(Button button, bool active)
    {
        button.interactable = active;
        var inactive = button.gameObject.transform.Find("Inactive").gameObject.GetComponent<Image>();
        inactive.enabled = !active;
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
            changeButtonState(button.Value, gameController.haveEnoughResourceForUnit(button.Key));
        }
    }

    private void changeDisplayedGroup(CanvasGroup groupToDisplay)
    {
        if (displayedGroup != null)
        {
            displayedGroup.alpha = 0;
            displayedGroup.interactable = false;
            displayedGroup.blocksRaycasts = false;
        }
        groupToDisplay.alpha = 1;
        groupToDisplay.interactable = true;
        groupToDisplay.blocksRaycasts = true;
        displayedGroup = groupToDisplay;
    }

    public void displayPotatoSkills(bool active)
    {
        foreach (Transform s in potatoSkills.gameObject.transform)
        {
            changeButtonState(s.gameObject.GetComponent<Button>(), gameController.phase != Phase.Build && active);
        }
        changeDisplayedGroup(potatoSkills);
    }
    public void displayCarrotSkills(bool active)
    {
        foreach (Transform s in carrotSkills.gameObject.transform)
        {
            changeButtonState(s.gameObject.GetComponent<Button>(), gameController.phase != Phase.Build && active);
        }
        changeDisplayedGroup(carrotSkills);
    }
    public void displayRadishSkills(bool active)
    {
        foreach (Transform s in radishSkills.gameObject.transform)
        {
            changeButtonState(s.gameObject.GetComponent<Button>(), gameController.phase != Phase.Build && active);
        }
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
