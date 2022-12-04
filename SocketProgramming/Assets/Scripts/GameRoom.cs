using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class StringExtensions
{
    public static string AddColor(this string text, Color col) => $"<color={ColorHexFromUnityColor(col)}>{text}</color>";
    public static string ColorHexFromUnityColor(this Color unityColor) => $"#{ColorUtility.ToHtmlStringRGBA(unityColor)}";
}

public class Question
{
    public string question;
    public string answer;

    public Question(string question, string answer)
    {
        this.question = question;
        this.answer = answer;
    }
}


public class GameRoom : MonoBehaviour
{
    private int numOfRounds;
    private float timePerRound = 10;
    private float timeLeft;
    private string curPlayerAnswer;

    public TMP_Text questionTxt;
    public TMP_Text roundNumberTxt;
    public TMP_Text totalRoundNumberTxt;
    public TMP_Text timerTxt;

    public TMP_InputField user_input;

    public Button submitBtn;

    private List<Question> questionList = new List<Question>();

    void getGameInfoFromServer()
    {
        numOfRounds = 5;

    }
    
    void getQuestionFromServer()
    {
        questionList.Add(new Question("8 x 6", "48"));
        questionList.Add(new Question("9 + 10", "19"));
        questionList.Add(new Question("-5 : 1", "-5"));
        questionList.Add(new Question("12 % 5", "2"));
        questionList.Add(new Question("9 - 10", "-1"));
    }

    // Start is called before the first frame update
    void Start()
    {
        getGameInfoFromServer();
        timeLeft = timePerRound;
        totalRoundNumberTxt.text = "/" + numOfRounds.ToString();
        user_input.Select();

        StartCoroutine(startGame());
    }

    IEnumerator startGame()
    {
        for (int i = 1; i <= numOfRounds; ++i)
        {
            timeLeft = timePerRound;
            curPlayerAnswer = null; //reset answer
            user_input.text = null;
            user_input.interactable = true;
            submitBtn.interactable = true;
            user_input.Select();
            Debug.Log("------------- Round " + i.ToString() + " -------------");
            roundNumberTxt.text = i.ToString();
            getQuestionFromServer();
            updateQuestion(questionList[i - 1]);

            while (timeLeft >= 0)
            {
                timeLeft -= 1;
                yield return new WaitForSeconds(1f);
                updateTimer(timeLeft);
            }

            yield return new WaitForSeconds(1f);

            updateAnswer(questionList[i - 1]);
            if (questionList[i - 1].answer.Equals(curPlayerAnswer))
            {
                Debug.Log("Round " + i.ToString() + " - Correct");
            }
            else
            {
                Debug.Log("Round " + i.ToString() + " - Wrong Answer");
            }
            yield return new WaitForSeconds(3f);
            //update point
            continue;
            //next question

            ////GetInput
            //if (timeLeft > 0)
            //{
            //    //Debug.Log(timeLeft);
            //    timeLeft -= Time.deltaTime;
            //    updateTimer(timeLeft);
            //}
            //else
            //{
            //    Debug.Log("Finish round!");
            //    timeLeft = timePerRound;
            //    //check answer and return result
            //    updateAnswer(questionList[i - 1]);
            //    if (questionList[i - 1].answer.Equals(curPlayerAnswer))
            //    {
            //        Debug.Log("Round " + i.ToString() + " - Correct");
            //    }
            //    else
            //    {
            //        Debug.Log("Round " + i.ToString() + " - Wrong Answer");
            //    }
            //    yield return new WaitForSeconds(5f);
            //    //update point
            //    continue;
            //    //next question
            //}

        }
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 1; i <= numOfRounds; ++i)
        //{
        //    curPlayerAnswer = null; //reset answer
        //    //user_input.text = null;
        //    //user_input.enabled = true;
        //    Debug.Log("------------- Round " + i.ToString() + " -------------");
        //    roundNumberTxt.text = i.ToString();
        //    getQuestionFromServer();
        //    updateQuestion(questionList[i - 1]);
            
        //    //GetInput
        //    if (timeLeft > 0)
        //    {
        //        //Debug.Log(timeLeft);
        //        timeLeft -= Time.deltaTime;
        //        updateTimer(timeLeft);
        //    }
        //    else
        //    {
        //        Debug.Log("Finish round!");
        //        timeLeft = timePerRound;
        //        //check answer and return result
        //        updateAnswer(questionList[i - 1]);
        //        if (questionList[i - 1].answer.Equals(curPlayerAnswer))
        //        {
        //            Debug.Log("Round " + i.ToString() + " - Correct");
        //        }
        //        else
        //        {
        //            Debug.Log("Round " + i.ToString() + " - Wrong Answer");
        //        }
        //        //update point
        //        continue;
        //        //next question
        //    }
        //}

    }

    void updateQuestion(Question q)
    {
        questionTxt.text = q.question + " = ?";
    }

    void updateAnswer(Question q)
    {
        //questionTxt.SetText($"" +
        //    $"{"8 + 6 = ".AddColor(Color.white)}" +
        //    $"{"14".AddColor(Color.green)}");
        questionTxt.SetText($"" +
            $"{q.question.AddColor(Color.white)}" +
            $" = " +
            $"{q.answer.AddColor(Color.green)}");
    }

    public void submitAnswer()
    {
        curPlayerAnswer = user_input.text;
        Debug.Log("Answer Submitted: " + curPlayerAnswer);
        user_input.interactable = false;
        submitBtn.interactable = false;
    }

    void updateScore(User curUser)
    {

    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerTxt.text = ((int)seconds).ToString();
    }

    void endGame()
    {

    }
}
