using SubterfugeCore.Core.Network;
using SubterfugeCore.Core.Players;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Login
{
    public class CheckUserLogin : MonoBehaviour
    {
        public Text username;

        public Text password;

        public Text loginInfo;
        // Start is called before the first frame update
        async void Start()
        {
            Api api = new Api();
            string username = PlayerPrefs.GetString("username");
            string password = PlayerPrefs.GetString("password");
        
            if (username != null && password != null && username != "" && password != "")
            {
                // TODO: Set a loading indicator variable here to let the user know that we are trying to log them in automatically.
            
                // Try to login.
                NetworkResponse<LoginResponse> response = await api.Login(username, password);
                if (response.IsSuccessStatusCode())
                {
                    // Save the player
                    ApplicationState.player = new Player(response.Response.user);
                
                    // Go to the main menu.
                    PlayerPrefs.SetString("token", response.Response.token);
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {
                    // TODO: Remove the loading indicator and allow the user to enter their credentials.
                }
            }
        }

        public async void onLogin()
        {
            Api api = new Api();
            Debug.Log("Sending login request");
            Debug.Log(username.text);
            Debug.Log(password.text);
            NetworkResponse<LoginResponse> response = await api.Login(username.text, password.text);
            Debug.Log(response.ResponseContent);
            if (response.IsSuccessStatusCode())
            {
                // Save the player
                ApplicationState.player = new Player(response.Response.user);
                
                // Go to the main menu.
                PlayerPrefs.SetString("token", response.Response.token);
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                loginInfo.text = response.ErrorContent.Message;
            }
        }

        public void onCreateAccount()
        {
            SceneManager.LoadScene("CreateAccount");
        }
    }
}
