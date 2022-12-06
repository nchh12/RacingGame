using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;
using PacketHandler;
using System.Threading.Tasks;

public class MainMenu : MonoBehaviour
{
    public static string user_name;
    public static ServerAllowJoinRoom roomData = null;

    public TMP_InputField user_input;
    public Button startBtn;

    List<string> mockUsernames;
    // Only accept'a'...'z', 'A'...'Z', '0'...'9', '_' and
    // the length is not longer than 10 characters
    Regex checkPattern = new Regex(@"^[a-zA-Z0-9_]+$");

    bool patternError = false;
    bool lengthError = false;
    bool uniqueError = false;
    bool connectionError = false;
    bool isRoomFull = false;

    TMP_Text patternErrorText;
    TMP_Text lengthErrorText;
    TMP_Text uniqueErrorText;
    TMP_Text connectionErrorText;
    TMP_Text roomFullText;

    // tcp://8.tcp.ngro k.io:12162
    const string LOCAL_HOST = "8.tcp.ngrok.io";
    const int LOCAL_PORT = 12162;

    private void Start()
    {
        // Start Connect -> Get ID
        APIUtil.AddListenerForConnectedEvent();
        API.Instance.ConnectAndListen(LOCAL_HOST, LOCAL_PORT);

        getInfoFromServer();
        // Disable 'start' button when nothing is inputted
        startBtn.interactable = false;
        user_input.Select();

        //patternErrorText = GameObject.Find("PatternError").GetComponent<TMP_Text>();
        //lengthErrorText = GameObject.Find("LengthError").GetComponent<TMP_Text>();
        //uniqueErrorText = GameObject.Find("UniqueError").GetComponent<TMP_Text>();
        //connectionErrorText = GameObject.Find("ConnectionError").GetComponent<TMP_Text>();
        //roomFullText = GameObject.Find("RoomFullError").GetComponent<TMP_Text>();
    }

    public void UpdateInputField()
    {
        if (!string.IsNullOrEmpty(user_input.text))
        {
            startBtn.interactable = true;
        }
        else
        {
            startBtn.interactable = false;
        }
    }

    void getInfoFromServer()
    {
        mockUsernames = new List<string> { "test", "tmp", "john", "bob" };
    }

    bool sendUsername()
    {
        patternError = false;
        lengthError = false;
        uniqueError = false;
        connectionError = false;
        isRoomFull = false;

        // TODO: Cannot connected case
        if(API.Instance.ClientID is null)
        {
            Debug.Log("~sendUsername->Null ClientId");
            connectionError = true;
            return false;
        }

        //TODO: Room full error

        if (API.Instance.ClientID.Length < 3)
        {
            Debug.Log("~sendUsername->ClientId " + API.Instance.ClientID + " is in wrong format");
            return false;
        }

        if (user_input.text.Length == 0 || user_input.text.Length > 10)
        {
            Debug.Log("Your username is too long");
            lengthError = true;
            return false;
        }

        if (!checkPattern.IsMatch(user_input.text))
        {
            Debug.Log("Your username is in wrong format");
            patternError = true;
            return false;
        }

        // Get username section
        user_name = user_input.text;
        // Add the last 3 character of ClientID
        var _tmp = API.Instance.ClientID.Substring(API.Instance.ClientID.Length - 3, 3);
        user_name += _tmp;


        // Prepare: Add listener for ServerAllowJoinRoom
        Action<string> _listenForAllowJoinPacket = (response) =>
        {
            var _wrapper = PacketWrapper<ServerAllowJoinRoom>.FromString<ServerAllowJoinRoom>(response);
            if (!_wrapper.IsValid()) return;
            try
            {
                roomData = _wrapper.GetData();
                API.Instance.ClearHandler();
                SceneManager.LoadScene("LobbyScreen");

            }
            catch (Exception e)
            {
                Debug.Log("~_listenForAllowJoinPacket->Exception : " + e.Message);
                Debug.Log("~_listenForAllowJoinPacket->StackTrace: " + e.StackTrace);
                Debug.Log("~_listenForAllowJoinPacket->Source    : " + e.Source);
            }
        };

        API.Instance.AddHandler(_listenForAllowJoinPacket);

        // Start: Send a request to server
        var _joinPacket = new JoinRoomPacket(user_name);
        var _packet = PacketWrapper<JoinRoomPacket>.FromData(_joinPacket);
        API.Instance.StartSendTask(_packet.StringifyPayload());

        // Wait for the response -> roomData = _data;

        return true;
        
    }

    public void PlayGame()
    {
        if (sendUsername())
        {
            Debug.Log("Hello " + user_name);
        }
        else
        {
            //display error
            if (connectionError == true)
            {
                connectionErrorText.enabled = true;
                patternErrorText.enabled = false;
                lengthErrorText.enabled = false;
                uniqueErrorText.enabled = false;
                roomFullText.enabled = false;
            }
            else
            {
                connectionErrorText.enabled = false;
                patternErrorText.enabled = true;
                lengthErrorText.enabled = true;
                uniqueErrorText.enabled = true;
                roomFullText.enabled = true;
            }

            if (isRoomFull == true)
            {
                roomFullText.enabled = true;
                patternErrorText.enabled = false;
                lengthErrorText.enabled = false;
                uniqueErrorText.enabled = false;
                connectionErrorText.enabled = false;
            }
            else
            {
                roomFullText.enabled = false;
                patternErrorText.enabled = true;
                lengthErrorText.enabled = true;
                uniqueErrorText.enabled = true;
                connectionErrorText.enabled = true;
            }
                
            if (patternError == true)
                patternErrorText.color = new Color32(255, 0, 9, 255);
            else
                patternErrorText.color = new Color32(255, 255, 255, 255);

            if (lengthError == true)
                lengthErrorText.color = new Color32(255, 0, 9, 255);
            else
                lengthErrorText.color = new Color32(255, 255, 255, 255);

            if (uniqueError == true)
                uniqueErrorText.color = new Color32(255, 0, 9, 255);
            else
                uniqueErrorText.color = new Color32(255, 255, 255, 255);

            //clear input field to retry
            user_input.text = null;
            user_input.Select();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
