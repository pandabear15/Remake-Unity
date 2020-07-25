using System.Collections;
using System.Collections.Generic;
using SubterfugeCore.Core.Network;
using Translation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefs.DeleteAll();
        
        // Set the API url.
        // Api api = new Api("http://18.220.154.6");
        Api api = new Api("http://localhost");
        
        // Load language strings.
        StringFactory.LoadStrings();
        
        // Go to login screen.
        SceneManager.LoadScene("LoginScreen");
    }
}
