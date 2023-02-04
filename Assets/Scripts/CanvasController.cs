using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private CanvasGroup potatoSkills;
    private CanvasGroup carrotSkills;
    private CanvasGroup radishSkills;
    private CanvasGroup builderOptions;

    private CanvasGroup displayedSkills;
    private void Start()
    {
        potatoSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        carrotSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        radishSkills = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        builderOptions = gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
    }

    private void changeDisplayedSkills(CanvasGroup skillsToDisplay)
    {
        if (displayedSkills != null)
            displayedSkills.alpha = 0;
        skillsToDisplay.alpha = 1;
        displayedSkills = skillsToDisplay;
    }

    public void displayPotatoSkills()
    {
        changeDisplayedSkills(potatoSkills);
    }
    public void displayCarrotSkills()
    {
        changeDisplayedSkills(carrotSkills);
    }
    public void displayRadishSkills()
    {
        changeDisplayedSkills(radishSkills);
    }
    public void displayBuilderOptions()
    {
        changeDisplayedSkills(builderOptions);
    }

    public void displayShrineOptions()
    {

    }
}
