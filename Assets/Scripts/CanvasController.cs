using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private CanvasGroup potatoSkills;
    private CanvasGroup carrotSkills;
    private CanvasGroup radishSkills;

    private CanvasGroup builderOptions;
    private CanvasGroup shrineOptions;
    private CanvasGroup fieldOptions;

    private CanvasGroup displayedGroup;
    private void Start()
    {
        potatoSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        carrotSkills = gameObject.transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();
        radishSkills = gameObject.transform.GetChild(2).gameObject.GetComponent<CanvasGroup>();

        builderOptions = gameObject.transform.GetChild(3).gameObject.GetComponent<CanvasGroup>();
        shrineOptions = gameObject.transform.GetChild(4).gameObject.GetComponent<CanvasGroup>();
        fieldOptions = gameObject.transform.GetChild(5).gameObject.GetComponent<CanvasGroup>();

        foreach (Transform trafo in gameObject.transform)
        {
            trafo.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void clear()
    {
        if (displayedGroup != null)
        {
            displayedGroup.alpha = 0;
            displayedGroup = null;
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
