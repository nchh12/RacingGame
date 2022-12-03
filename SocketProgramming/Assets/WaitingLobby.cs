using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingLobby : MonoBehaviour
{
    private float timeAmount;
    private float timeLeft;
    private int minNumOfPlayer;
    private string curPlayerUsername;
    private int curNumOfPlayer;

    bool timerOn = false;

    public TMP_Text timerTxt;

    private List<User> curUserList = new List<User>(); 

    void getGameInfoFromServer()
    {
        timeAmount = 10;
        minNumOfPlayer = 3;
        getListOfPlayers();
        curPlayerUsername = "test";
        curUserList.Add(new User(curPlayerUsername, curPlayerUsername, 0));
        curNumOfPlayer = curUserList.Count;
    }

    void getListOfPlayers()
    {
        curUserList.Add(new User("client-1", "client-1", 0));
        curUserList.Add(new User("client-2", "client-2", 0));
    }

    void updatePlayerUI(List<User> playerList)
    {
        //update current players avatar and username
        for (int i = 1; i <= playerList.Count; i++)
        {
            Image playerAvatar = GameObject.Find("Avatar" + i).GetComponent<Image>();
            TMP_Text playerUsername = GameObject.Find("Username" + i).GetComponent<TMP_Text>();
            playerUsername.text = playerList[i - 1].username;
            playerAvatar.sprite = Resources.Load<Sprite>("Sprites/" + playerList[i-1].avatarImage);
            playerAvatar.color = new Color(255, 255, 255, 255);
        }

        //remove duplicate
        for (int i = playerList.Count + 1; i <= 4; i++)
        {
            Image playerAvatar = GameObject.Find("Avatar" + i).GetComponent<Image>();
            TMP_Text playerUsername = GameObject.Find("Username" + i).GetComponent<TMP_Text>();
            playerUsername.text = null;
            playerAvatar.sprite = null;
            playerAvatar.color = new Color(255, 255, 255, 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        getGameInfoFromServer();
        TMP_Text minNumPlayer = GameObject.Find("MinNumPlayer").GetComponent<TMP_Text>();
        minNumPlayer.text = minNumOfPlayer.ToString();
        timeLeft = timeAmount;
    }

    // Update is called once per frame
    void Update()
    {
        curNumOfPlayer = curUserList.Count;
        updatePlayerUI(curUserList);
        timerTxt.text = (minNumOfPlayer - curNumOfPlayer).ToString();
        if (curNumOfPlayer >= minNumOfPlayer)
        {
            //start timer
            //timerOn = true;
            //if (timeLeft > 7.99 && timeLeft < 8.0)
            //{
            //    curUserList.Add(new User("client-2", "client-2", 0));
            //}
            //if (timeLeft > 5.99 && timeLeft < 6.0)
            //{
            //    curUserList.RemoveAt(0);
            //}
            //if (timeLeft > 3.99 && timeLeft < 4.0)
            //{
            //    curUserList.Add(new User("client-1", "client-1", 0));
            //    //curUserList.RemoveAt(1);
            //}
            //if (timeLeft > 1.99 && timeLeft < 2.0)
            //{
            //    curUserList.RemoveAt(1);
            //}
            //if (timeLeft > 0)
            //{
            //    //Debug.Log(timeLeft);
            //    timeLeft -= Time.deltaTime;
            //    updateTimer(timeLeft);
            //}
            //else
            //{
                Debug.Log("Starting Game!");
            //timeLeft = 0;
            //timerOn = false;
            StartCoroutine(startGame());
                
            //}
        }
        //else
        //{
        //    reset timer
        //    timerOn = false;
        //    timeLeft = timeAmount;
        //}
    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("RoundScreen");
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerTxt.text = ((int)seconds).ToString();
    }

    public void returnToMainMenu()
    {
        SceneManager.LoadScene("MenuScreen");
    }

}
