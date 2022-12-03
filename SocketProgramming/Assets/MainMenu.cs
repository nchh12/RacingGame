using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class MainMenu : MonoBehaviour
{
    public string user_name;
    public TMP_InputField user_input;
    public Button startBtn;
    public TMP_Text error_text;

    List<string> mockUsernames = new List<string> { "test", "tmp", "john", "bob" };
    Regex checkPattern = new Regex(@"^[a-zA-Z0-9_]{1,10}$"); //Only accept'a'...'z', 'A'...'Z', '0'...'9', '_'and the length is not longer than 10 characters

    private void Start()
    {
        startBtn.interactable = false;
        user_input.Select();
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

    bool sendUsername()
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
            }
        }
        else
        {
            Debug.Log("Your username is in wrong format");
        }
        user_input.text = null;
        user_input.Select();
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
            error_text.color = new Color32(255, 252, 0, 255);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    
}
