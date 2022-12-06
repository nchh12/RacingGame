using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using PacketHandler;

public class WaitingLobby : MonoBehaviour
{
    const int MIN_NUM_OF_PLAYER = 2;
    const float COUNT_DOWN = 0.5f;

    private int minNumOfPlayer;
    private string curPlayerUsername;
    private int curNumOfPlayer;
    private float timeLeft = COUNT_DOWN;

    public GameObject GameInfoFrame;
    public GameObject TimerFrame;
    public TMP_Text countdownTxt;
    public TMP_Text timerTxt;

    public static List<User> curUserList = new List<User>(); 

    void getGameInfoFromServer()
    {
        minNumOfPlayer = MIN_NUM_OF_PLAYER;
        getListOfPlayers();
        curPlayerUsername = MainMenu.user_name != null ? MainMenu.user_name : "test";
        //curUserList.Add(new User(curPlayerUsername, curPlayerUsername, 0));
        curNumOfPlayer = curUserList.Count;
    }

    void getListOfPlayers()
    {
        var _roomData = MainMenu.roomData;
        foreach(var username in _roomData.listUsers)
        {
            curUserList.Add(new User(username, username, 0));
        }
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

        Action<string> _listenForStartRoundPacket = (response) =>
        {
            try
            {
                var wrappedPacket = PacketWrapper<ServerStartRoundPacket>.FromString<ServerStartRoundPacket>(response);
                if (!wrappedPacket.IsValid()) return;
                var _data = wrappedPacket.GetData();
                // Read the first packet with this type
                // => Used to pass to the next scene
                firstServerPacket = _data;

                // Clear the handler to prevent future reading
                API.Instance.ClearHandler();
            }
            catch (Exception)
            {

            }
        };

        API.Instance.AddHandler(_listenForStartRoundPacket);
    }

    public static ServerStartRoundPacket firstServerPacket = null;
    //public static float timesec = null;

    // Update is called once per frame
    void Update()
    {
        curNumOfPlayer = curUserList.Count;
        updatePlayerUI(curUserList);
        countdownTxt.text = (minNumOfPlayer - curNumOfPlayer).ToString();
        if (curNumOfPlayer >= minNumOfPlayer)
        {
            // For each frame -> check the time-left
            StartCoroutine(startGame());
        }
    }

    IEnumerator startGame()
    {
        // Wait for 2 second
        yield return new WaitForSeconds(2f);
        GameInfoFrame.SetActive(false);
        TimerFrame.SetActive(true);

        // if the time-left is positive
        // update UI and return
        if (timeLeft > 0)
        {
            //Debug.Log(timeLeft);
            timeLeft -= Time.deltaTime;
            updateTimer(timeLeft);
        }
        else
        {
            // Time zero -> proceed to next frame
            Debug.Log("Starting Game!");
            timeLeft = 0;

            // But if the packet haven't arrive -> return and
            // let the next routine check if the packet arrived?
            if (firstServerPacket is null)
            {
                Debug.Log("Packet haven't arrive");
            }
            else
            {
                Debug.Log("Packet arrive");
                SceneManager.LoadScene("RoundScreen");
            }
        }
        
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
