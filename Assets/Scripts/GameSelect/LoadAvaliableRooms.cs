using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubterfugeCore.Core.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadAvaliableRooms : MonoBehaviour
{
    public HorizontalLayoutGroup hGroup;
    public HorizontalLayoutGroup bottomGroup;
    public Button joinButton;
    public Button bPub;
    public Button bPriv;
    public GameRoomButton scrollItemTemplate;
    Api api = new Api();

    // Start is called before the first frame update
    async void Start()
    {
        LoadOpenRooms();
    }

    public async void LoadOpenRooms()
    {
        NetworkResponse<GameRoomResponse> roomResponse = await api.GetOpenRooms();

        // Destroy all existing rooms.
        GameRoomButton[] existingButtons = FindObjectsOfType<GameRoomButton>();
        foreach (GameRoomButton gameRoomButton in existingButtons)
        {
            if (gameRoomButton.isActiveAndEnabled)
            {
                Destroy(gameRoomButton.gameObject);
            }
        }

        if (roomResponse.IsSuccessStatusCode())
        {
            foreach (GameRoom room in roomResponse.Response.array)
            {
                HorizontalLayoutGroup newGroup = Instantiate(hGroup);
                newGroup.gameObject.SetActive(true);

                // Create a new templated item
                Button join = Instantiate(joinButton);
                join.gameObject.SetActive(true);

                GameRoomButton scrollItem = (GameRoomButton) Instantiate(scrollItemTemplate);
                scrollItem.gameObject.SetActive(true);
                scrollItem.room = room;
                scrollItem.GetComponent<Button>().onClick.AddListener(delegate { GoToGameLobby(room); });

                // Set the text
                Text misc = scrollItem.transform.Find("Misc").GetComponent<Text>();
                Text playerCount = scrollItem.transform.Find("PlayerCount").GetComponent<Text>();
                Text roomTitle = scrollItem.transform.Find("RoomTitle").GetComponent<Text>();
                Image anon = scrollItem.transform.Find("Anonymity").GetComponent<Image>();
                Text ratedNumber = scrollItem.transform.Find("RatedNumber").GetComponent<Text>();
                Text goalNumber = scrollItem.transform.Find("GoalNumber").GetComponent<Text>();

                playerCount.text = room.Players.Count + " of " + room.Max_Players + " present";
                roomTitle.text = room.Description;
                ratedNumber.text = room.Rated ? room.Min_Rating.ToString() : "All";
                goalNumber.text = "G:" + room.Goal.ToString();
                
                if(room.Anonymity)
                    anon.gameObject.SetActive(true);
                else
                    anon.gameObject.SetActive(false);


                if (misc != null)
                {
                    misc.text = "GameId: " + room.Room_Id + ", Seed: " + room.Seed
                                 + ", Created By: " + room.Creator_Id;
                }
                else
                {
                    Debug.Log("No Text.");
                }

                // Set the button's parent to the scroll item template.
                join.transform.SetParent(newGroup.transform, false);
                scrollItem.transform.SetParent(newGroup.transform, false);
                newGroup.transform.SetParent(hGroup.transform.parent, false);

            }

            HorizontalLayoutGroup bottomLayoutGroup = Instantiate(bottomGroup);
            bottomLayoutGroup.gameObject.SetActive(true);
            Button pub = Instantiate(bPub);
            pub.gameObject.SetActive(true);
            Button priv = Instantiate(bPriv);
            priv.gameObject.SetActive(true);

            pub.transform.SetParent(bottomLayoutGroup.transform, false);
            priv.transform.SetParent(bottomLayoutGroup.transform, false);
            bottomLayoutGroup.transform.SetParent(bottomGroup.transform.parent, false);

            // TODO: Add some text to notify user they are offline.
        }
    }

    public async void LoadOngoingRooms()
    {
        NetworkResponse<GameRoomResponse> roomResponse = await api.GetOngoingRooms();
        
        // Destroy all existing rooms.
        GameRoomButton[] existingButtons = FindObjectsOfType<GameRoomButton>();
        foreach (GameRoomButton gameRoomButton in existingButtons)
        {
            if (gameRoomButton.isActiveAndEnabled)
            {
                Destroy(gameRoomButton.gameObject);
            }
        }

        if (roomResponse.IsSuccessStatusCode())
        {

            foreach (GameRoom room in roomResponse.Response.array)
            {
                // Create a new templated item
                GameRoomButton scrollItem = (GameRoomButton) Instantiate(scrollItemTemplate);
                scrollItem.gameObject.SetActive(true);
                scrollItem.room = room;
                scrollItem.GetComponent<Button>().onClick.AddListener(delegate { GoToGame(room); });

                // Set the text
                Text text = scrollItem.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = "[ GameId: " + room.Room_Id + " Title: " + room.Description + ", Seed: " + room.Seed +
                                ", Players: " + room.Players.Count + "/" + room.Max_Players + ", Anonymous: " +
                                room.Anonymity + ", Created By: " + room.Creator_Id + "]";
                }
                else
                {
                    Debug.Log("No Text.");
                }

                // Set the button's parent to the scroll item template.
                scrollItem.transform.SetParent(scrollItemTemplate.transform.parent, false);
            }
        }
        // TODO: Add some text to notify the user that they are offline.
    }

    public Button.ButtonClickedEvent GoToGameLobby(GameRoom room)
    {
        // Set the gameroom to the selected game
        ApplicationState.currentGameRoom = room;
        
        // Load the game scene
        SceneManager.LoadScene("GameLobby");
        return null;
    }

    public Button.ButtonClickedEvent GoToGame(GameRoom room)
    {
        // Set the gameroom to the selected game
        ApplicationState.currentGameRoom = room;
        
        // Load the game scene
        SceneManager.LoadScene("Game");
        return null;
    }
    
    public void onCreateGameClicked()
    {
        SceneManager.LoadScene("CreateGame");
    }

    public void onBackClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
