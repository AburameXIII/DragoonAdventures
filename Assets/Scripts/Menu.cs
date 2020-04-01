using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public List<Button> AllButtons;
    public Animator UIFadeOutAnimator;

    public void DisableButtons()
    {
        foreach(Button b in AllButtons)
        {
            b.interactable = false;
        }
    }

    IEnumerator LoadScene(int i)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(i);
    }

    public void PlayGame()
    {
        DisableButtons();
        UIFadeOutAnimator.SetTrigger("FadeOut");
        StartCoroutine(LoadScene(1));
    }

    public void MainMenu()
    {
        DisableButtons();
        UIFadeOutAnimator.SetTrigger("FadeOut");
        StartCoroutine(LoadScene(0));
    }

    public void QuitGame()
    {
        DisableButtons();
        Application.Quit();
    }
}
