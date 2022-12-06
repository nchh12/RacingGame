using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using System;
using PacketHandler;

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
    private const float TIME_PER_ROUND = 10 - 2; // is the delay time for UI
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
        numOfRounds = 5;
    }
    
    static bool bEndRound = false;


    void AddStartRoundListen()
    {
        // Listen for the middle (not the first start round packet)
        Action<string> _listenForMiddleStartRound = (response) =>
        {
            try
            {
                Debug.Log("[RESPONSE] - " + response);
                var wrappedPacket = PacketWrapper<ServerStartRoundPacket>.FromString<ServerStartRoundPacket>(response);
                if (!wrappedPacket.IsValid()) return;
                var _data = wrappedPacket.GetData();
                // Signaling the end-round
                bEndRound = true;
                gameRound = _data.round;
                currentServerStartRoundPacket = _data;


            }
            catch (Exception e)
            {
                Debug.Log("~_listenForMiddleStartRound->" + e.Message);
                Debug.Log("~_listenForMiddleStartRound->" + e.StackTrace);
            }
        };
        API.Instance.AddHandler(_listenForMiddleStartRound);
    }


    public void updateData()
    {
        var _data = currentServerStartRoundPacket;
        // Update Q/ A data
        questionList.Add(new Question(_data.question, _data.answer));

        // Update user list with info
        WaitingLobby.curUserList.Clear();
        foreach (var _playerInfo in _data.listRankedUser)
        {
            try
            {
                WaitingLobby.curUserList.Add(new User(
                    id: _playerInfo["id"],
                    username: _playerInfo["username"],
                    score: Int32.Parse(_playerInfo["score"]))
                   );
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("~_listenForMiddleStartRound->KeyNotFoundException->Message" + e.Message);
                Debug.Log("~_listenForMiddleStartRound->KeyNotFoundException->StackTrace" + e.StackTrace);
            }
        }
    }


    public static ServerStartRoundPacket currentServerStartRoundPacket = null;
    // Start is called before the first frame update
    void Start()
    {
        currentServerStartRoundPacket = WaitingLobby.firstServerPacket;
        if(currentServerStartRoundPacket is null)
        {
            Debug.Log("~Start->currentServerStartRoundPacket is null" +
                "->This can not be happend...");
        }

        // Update the data for render
        updateData();

        //// Add data from the first packet
        //questionList.Clear();
        //questionList.Add(new Question(currentServerStartRoundPacket.answer, ))

        AddStartRoundListen();
        getGameInfoFromServer();

        timeLeft = TIME_PER_ROUND;
        totalRoundNumberTxt.text = "/" + numOfRounds.ToString();

        user_input.Select();

        //getListOfPlayers(); -> mock-data

        initiatePlayerUI(WaitingLobby.curUserList);
        submitBtn.onClick.AddListener(delegate { submitAnswer(timeLeft); });
        
        StartCoroutine(startGame());
        
    }

    IEnumerator startGame()
    {
        for (int i = 1; i <= numOfRounds + 1; ++i)
        {
            // Start-round -> true
            if(i <= numOfRounds)
            {
                bEndRound = false;
            }
            else
            {
                // Extra round -> end round immediately
                bEndRound = true;
            }
            // Update UI for new round
            timeLeft = TIME_PER_ROUND;
            // Reset answer
            curPlayerAnswer = null;
            user_input.text = null;
            user_input.interactable = true;
            submitBtn.interactable = true;
            resultImg.enabled = false;
            user_input.textComponent.color = Color.white;
            
            user_input.Select();
            Debug.Log("------------- Round " + i.ToString() + " -------------");
            roundNumberTxt.text = i.ToString();

            //getQuestionFromServer();
            updateQuestion(questionList[i - 1]);

            // For each second -> update the UI
            while (timeLeft >= 0 && !bEndRound)
            {
                timeLeft -= 1;
                yield return new WaitForSeconds(1f);
                updateTimer(timeLeft);
            }

            // Wait for End-round signal
            // which is set by the API handler of
            // ServerStartRoundPacket
            while (!bEndRound)
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Wait an extra 0.5s
            yield return new WaitForSeconds(0.5f);

            // Update data
            updateData();
            Debug.Log("[ROUND - " + i + "] - UPDATED DATA");
            // Update UI -> Answer
            updateAnswer(questionList[i - 1]);
            resultImg.enabled = true;
            Debug.Log("[ROUND - " + i + "] - UPDATED DATA");


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

            // Update UI -> Score
            // updateScore(WaitingLobby.curUserList);
            // Update Leaderboard
            WaitingLobby.curUserList.Sort(SortUserByScore);

            updatePlayerUI(WaitingLobby.curUserList);
            if (i > numOfRounds)
                break;

            yield return new WaitForSeconds(0.5f);
            
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

    public static int gameRound = 1;
    public void submitAnswer(float answerTime)
    {
        curPlayerAnswer = user_input.text;
        Debug.Log("Answer Submitted: " + curPlayerAnswer + " - Time: " + answerTime.ToString());
        user_input.interactable = false;
        submitBtn.interactable = false;
        user_input.textComponent.color = Color.gray;

        var _ansPacket = new ClientAnswer(round: gameRound, answer: curPlayerAnswer);
        var _packet = PacketWrapper<ClientAnswer>.FromData(_ansPacket);
        API.Instance.StartSendTask(_packet.StringifyPayload());

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

    //void updateScore(List<User> playerList)
    //{
    //    for (int i = 1; i <= playerList.Count; i++)
    //    {
    //        playerList[i - 1].score += random.Next(1, 10);
    //    }
    //}

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
        SceneManager.LoadScene("MenuScreen");
    }
}
