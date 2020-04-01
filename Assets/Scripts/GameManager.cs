using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }


    public Animator GameOverUIAnimator;
    public int Objective;
    public Text ObjectiveUI;
    public Text ObjectiveUIText;

    // Start is called before the first frame update
    void Start()
    {
        ObjectiveUI.text = Objective.ToString();

        if (Objective == 1)
        {
            ObjectiveUIText.text = "enemy left";
        }
        else
        {
            ObjectiveUIText.text = "enemies left";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Objective <= 0)
        {
            Victory();
        }
    }

    public void Victory()
    {
        GameOverUIAnimator.SetTrigger("Victory");
        StartCoroutine(LoadScene(3));
    }

    public void MonsterKilled()
    {
        if (Objective > 0)
        {
            Objective--;
            ObjectiveUI.text = Objective.ToString();

            if (Objective == 1)
            {
                ObjectiveUIText.text = "enemy left";
            }
            else
            {
                ObjectiveUIText.text = "enemies left";
            }
        }
    }


    public void GameOver()
    {
        GameOverUIAnimator.SetTrigger("GameOver");
        StartCoroutine(LoadScene(2));
    }


    IEnumerator LoadScene(int i)
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(i);
    }
}
