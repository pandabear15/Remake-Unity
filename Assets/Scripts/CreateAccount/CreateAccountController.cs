using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SubterfugeCore.Core.Network;
using SubterfugeCore.Core.Players;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateAccountController : MonoBehaviour
{
    public Text username;
    public Text password;
    public Text email;
    public Text responseInfo;
    
    public void Start()
    {
    }

    public async void onRegister()
    {
        Api api = new Api();
        NetworkResponse<RegisterResponse> response = await api.RegisterAccount(username.text, password.text, email.text);

        if (response.IsSuccessStatusCode())
        {
            ApplicationState.player = new Player(response.Response.User);
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("password", password.text);
            PlayerPrefs.SetString("token", response.Response.Token);
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            responseInfo.text = response.ErrorContent.Message;
        }
    }

    public void onCancel()
    {
        SceneManager.LoadScene("LoginScreen");
    }
}
