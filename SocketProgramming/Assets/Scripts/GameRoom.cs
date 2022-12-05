using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

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
    private static readonly Random random = new Random();

    private int numOfRounds;
    private float timePerRound = 3;
    private float timeLeft;
    private string curPlayerAnswer;

    public TMP_Text questionTxt;
    public TMP_Text roundNumberTxt;
    public TMP_Text totalRoundNumberTxt;
    public TMP_Text timerTxt;

    public TMP_InputField user_input;

    public Button submitBtn;

    public Image resultImg;

    public GameObject finalResultBoard;

    private List<Question> questionList = new List<Question>();

    void getGameInfoFromServer()
    {
        numOfRounds = 3;
    }
    
    void getQuestionFromServer()
    {
        questionList.Add(new Question("8 x 6", "48"));
        questionList.Add(new Question("9 + 10", "19"));
        questionList.Add(new Question("-5 : 1", "-5"));
        questionList.Add(new Question("12 % 5", "2"));
        questionList.Add(new Question("9 - 10", "-1"));
    }

    void getListOfPlayers()
    {
        WaitingLobby.curUserList.Add(new User("client-1", "client-1", 0));
        WaitingLobby.curUserList.Add(new User("client-2", "client-2", 0));
        WaitingLobby.curUserList.Add(new User("test", "test", 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        getGameInfoFromServer();
        timeLeft = timePerRound;
        totalRoundNumberTxt.text = "/" + numOfRounds.ToString();
        user_input.Select();
        getListOfPlayers();
        initiatePlayerUI(WaitingLobby.curUserList);
        submitBtn.onClick.AddListener(delegate { submitAnswer(timeLeft); });
        
        StartCoroutine(startGame());
        
    }

    IEnumerator startGame()
    {
        for (int i = 1; i <= numOfRounds; ++i)
        {
            //Update UI for new round
            timeLeft = timePerRound;
            curPlayerAnswer = null; //reset answer
            user_input.text = null;
            user_input.interactable = true;
            submitBtn.interactable = true;
            resultImg.enabled = false;
            user_input.textComponent.color = Color.white;
            
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
            resultImg.enabled = true;
            if (questionList[i - 1].answer.Equals(curPlayerAnswer))
            {
                Debug.Log("Round " + i.ToString() + " - Correct");
                resultImg.sprite = Resources.Load<Sprite>("Sprites/Checked_04");
                user_input.textComponent.color = Color.green;
            }
            else
            {
                Debug.Log("Round " + i.ToString() + " - Wrong Answer");
                resultImg.sprite = Resources.Load<Sprite>("Sprites/Checked_03");
                user_input.textComponent.color = Color.red;
            }

            updateScore(WaitingLobby.curUserList);

            WaitingLobby.curUserList.Sort(SortUserByScore);

            updatePlayerUI(WaitingLobby.curUserList);

            yield return new WaitForSeconds(3f);
            
            continue; //move to next question

        }

        endGame(WaitingLobby.curUserList[0]);
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

    public void submitAnswer(float answerTime)
    {
        curPlayerAnswer = user_input.text;
        Debug.Log("Answer Submitted: " + curPlayerAnswer + " - Time: " + answerTime.ToString());
        user_input.interactable = false;
        submitBtn.interactable = false;
        user_input.textComponent.color = Color.gray;

        //TODO: send answer to server
    }

    void initiatePlayerUI(List<User> playerList)
    {
        //update current players avatar and username
        updatePlayerUI(playerList);

        //hide empty player frame
        for (int i = playerList.Count + 1; i <= 4; i++)
        {
            Image playerFrame = GameObject.Find("Player" + i).GetComponent<Image>();
            playerFrame.transform.gameObject.SetActive(false);
        }
    }

    void updatePlayerUI(List<User> playerList)
    {
        //update current players avatar and username
        for (int i = 1; i <= playerList.Count; i++)
        {
            Image playerAvatar = GameObject.Find("Avatar" + i).GetComponent<Image>();
            TMP_Text playerUsername = GameObject.Find("Username" + i).GetComponent<TMP_Text>();
            TMP_Text playerScore = GameObject.Find("Score" + i).GetComponent<TMP_Text>();
            playerUsername.text = playerList[i - 1].username;
            playerAvatar.sprite = Resources.Load<Sprite>("Sprites/" + playerList[i - 1].avatarImage);
            playerScore.text = playerList[i - 1].score.ToString();
        }
    }

    void updateScore(List<User> playerList)
    {
        for (int i = 1; i <= playerList.Count; i++)
        {
            playerList[i - 1].score += random.Next(1, 10);
        }
    }

    static int SortUserByScore(User p1, User p2)
    {
        return p2.score.CompareTo(p1.score);
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerTxt.text = ((int)seconds).ToString();
    }

    void endGame(User winner)
    {
        finalResultBoard.transform.gameObject.SetActive(true);

        //declare winner
        Image playerAvatar = GameObject.Find("WinnerAvatar").GetComponent<Image>();
        TMP_Text playerUsername = GameObject.Find("WinnerName").GetComponent<TMP_Text>();
        TMP_Text playerScore = GameObject.Find("WinnerScore").GetComponent<TMP_Text>();
        playerUsername.text = winner.username;
        playerAvatar.sprite = Resources.Load<Sprite>("Sprites/" + winner.avatarImage);
        playerScore.text = winner.score.ToString();

        //return to lobby room
    }

    public void returnToLobby()
    {
        SceneManager.LoadScene("LobbyScreen");
    }
}
