using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerEvaluator : MonoBehaviour
{

    // singleton pattern and awake method is based on solution found here:
    // - https://answers.unity.com/questions/891380/unity-c-singleton.html
    #region SINGLETON PATTERN
    private static AnswerEvaluator _instance;
    public static AnswerEvaluator Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AnswerEvaluator>();
                
                if (_instance == null)
                {
                    GameObject container = new GameObject("AnswerEvaluator");
                    _instance = container.AddComponent<AnswerEvaluator>();
                }
            }
        
            return _instance;
        }
    }
    #endregion
    
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }
    
        _instance = this;
        //DontDestroyOnLoad( this.gameObject );
    }

    public GameObject yes_button;

    public GameObject no_button;

    public GameObject question;

    public GameObject right_answer;


    // Start is called before the first frame update
    void Start()
    {
        PrepareMyChildern();
        DisableMyChildren();
    }

    private void PrepareMyChildern()
    {
        yes_button.GetComponent<Button>().onClick.AddListener(AnswerCorrect);
        no_button.GetComponent<Button>().onClick.AddListener(AnswerIncorrect);

        question.GetComponent<Text>().text = "Odpoveď týmu je: ";

    }

    public void ShowQuery(string right_answer_text)
    {
        // activate components
        yes_button.SetActive(true);
        no_button.SetActive(true);
        question.SetActive(true);

        right_answer.GetComponent<Text>().text = "Správná odpověď: " + right_answer_text;
        right_answer.SetActive(true);
    }

    private void DisableMyChildren()
    {
        yes_button.SetActive(false);
        no_button.SetActive(false);
        question.SetActive(false);
        right_answer.SetActive(false);
    }

    public void AnswerCorrect()
    {
        // let gamemanager know
        GameManager.Instance.LastAnswerWas(true);

        // disable all
        DisableMyChildren();
    }

    public void AnswerIncorrect()
    {
        // let gamemanager know
        GameManager.Instance.LastAnswerWas(false);
        
        // disable all
        DisableMyChildren();
    }
}
