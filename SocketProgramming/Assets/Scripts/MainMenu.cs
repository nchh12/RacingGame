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

    public Image loadingCircle;

    List<string> mockUsernames;
    // Only accept'a'...'z', 'A'...'Z', '0'...'9', '_' and
    // the length is not longer than 10 characters
    Regex checkPattern = new Regex(@"^[a-zA-Z0-9_]+$");

    bool patternError = false;
    bool lengthError = false;
    bool uniqueError = false;
    bool connectionError = false;
    bool roomFullError = false;

    public TMP_Text patternErrorText;
    public TMP_Text lengthErrorText;
    public TMP_Text uniqueErrorText;
    public TMP_Text connectionErrorText;
    public TMP_Text roomFullErrorText;

    bool isLoading = false;

    const string LOCAL_HOST = "192.168.1.89";
    const int LOCAL_PORT = 5555;

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
        //roomFullErrorText = GameObject.Find("RoomFullError").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        LoadingInfo();
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
        //isLoading = true;
        Debug.Log(user_input.text);
        patternError = false;
        lengthError = false;
        uniqueError = false;
        connectionError = false;
        roomFullError = false;

        // TODO: Cannot connected case
        if (API.Instance.ClientID is null)
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

            roomData = _wrapper.GetData();
            SceneManager.LoadScene("LobbyScreen");
        };

        API.Instance.AddHandler(_listenForAllowJoinPacket);

        // Start: Send a request to server
        var _joinPacket = new JoinRoomPacket(user_name);
        var _packet = PacketWrapper<JoinRoomPacket>.FromData(_joinPacket);
        API.Instance.StartSendTask(_packet.StringifyPayload());

        // Wait for the response -> roomData = _data;
        loadingCircle.gameObject.SetActive(true);

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
                connectionErrorText.color = new Color32(255, 0, 9, 255);
            else
                connectionErrorText.color = new Color32(255, 255, 255, 0);

            if (roomFullError == true)
                roomFullErrorText.color = new Color32(255, 0, 9, 255);
            else
                roomFullErrorText.color = new Color32(255, 255, 255, 0);

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

    void LoadingInfo()
    {
        loadingCircle.gameObject.SetActive(isLoading);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
