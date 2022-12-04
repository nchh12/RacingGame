using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class MainMenu : MonoBehaviour
{
    public static string user_name;
    public TMP_InputField user_input;
    public Button startBtn;

    List<string> mockUsernames; 
    Regex checkPattern = new Regex(@"^[a-zA-Z0-9_]+$"); //Only accept'a'...'z', 'A'...'Z', '0'...'9', '_'and the length is not longer than 10 characters

    bool patternError = false;
    bool lengthError = false;
    bool uniqueError = false;

    TMP_Text patternErrorText;
    TMP_Text lengthErrorText;
    TMP_Text uniqueErrorText;

    private void Start()
    {
        getInfoFromServer();
        //Disable 'start' button when nothing is inputted
        startBtn.interactable = false; 
        user_input.Select();

        patternErrorText = GameObject.Find("PatternError").GetComponent<TMP_Text>();
        lengthErrorText = GameObject.Find("LengthError").GetComponent<TMP_Text>();
        uniqueErrorText = GameObject.Find("UniqueError").GetComponent<TMP_Text>();
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

        if (user_input.text.Length > 0 && user_input.text.Length <= 10)
        {
            if (checkPattern.IsMatch(user_input.text))
            {
                if (!mockUsernames.Contains(user_input.text))
                {
                    user_name = user_input.text;
                    return true;
                }
                else
                {
                    Debug.Log("Username already existed");
                    uniqueError = true;
                }
            }
            else
            {
                Debug.Log("Your username is in wrong format");
                patternError = true;
            }
        }
        else
        {
            Debug.Log("Your username is too long");
            lengthError = true;
        }
        
        return false;
    }
    public void PlayGame()
    {
        if (sendUsername())
        {
            Debug.Log("Hello " + user_name);
            SceneManager.LoadScene("LobbyScreen");
        }
        else
        {
            //display error
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
