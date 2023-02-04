using TMPro;
using UnityEngine;

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
    public TMP_Text potato;
    public TMP_Text carrot;
    public TMP_Text radish;

    private CanvasGroup displayedGroup;
    private GameController gameController;
    private void Start()
    {
        gameController = GameObject.Find("GameController").gameObject.GetComponent<GameController>();

        var skills = gameObject.transform.Find("Skills").gameObject.transform;
        potatoSkills = skills.Find("PotatoSkills").gameObject.GetComponent<CanvasGroup>();
        carrotSkills = skills.Find("CarrotSkills").gameObject.GetComponent<CanvasGroup>();
        radishSkills = skills.Find("RadishSkills").gameObject.GetComponent<CanvasGroup>();

        builderOptions = skills.Find("BuilderOptions").gameObject.GetComponent<CanvasGroup>();
        shrineOptions = skills.Find("ShrineOptions").gameObject.GetComponent<CanvasGroup>();
        fieldOptions = skills.Find("FieldOptions").gameObject.GetComponent<CanvasGroup>();

        //gameObject.transform.Get
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
        potato.text = res[ResourceType.potato].ToString();
        carrot.text = res[ResourceType.carrot].ToString();
        radish.text = res[ResourceType.radish].ToString();
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
