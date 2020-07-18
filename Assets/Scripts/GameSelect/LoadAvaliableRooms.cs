using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SubterfugeCore.Core.Network;
using SubterfugeCore.Core.Players;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadAvaliableRooms : MonoBehaviour
{
    public HorizontalLayoutGroup hlRoomItemTemplate;
    public HorizontalLayoutGroup hlCreateRoomTemplate;

    public Button btnOpenGames;
    public Button btnOngoingGames;
    public Button btnEndedGames;
    public Button btnContextActionTemplate;
    public Button btnCreatePublicGameTemplate;
    public Button btnCreatePrivateGameTemplate;
    public GameRoomButton btnRoomItemTemplate;
    
    public Sprite cancel;
    public Sprite resign;
    public Sprite leave;

    public Color activeButtonSelector = Color.white;
    public Color inactiveButtonSelector = new Color32(219, 212, 212, 255);

    Api api = new Api();

    // Start is called before the first frame update
    async void Start()
    {
        LoadOpenRooms();
    }

    public async void LoadOpenRooms()
    {
        // Switch button colors
        btnOpenGames.GetComponentInChildren<Text>().color = activeButtonSelector;
        btnOngoingGames.GetComponentInChildren<Text>().color = inactiveButtonSelector;
        btnEndedGames.GetComponentInChildren<Text>().color = inactiveButtonSelector;
        
        NetworkResponse<GameRoomResponse> roomResponse = await api.GetOpenRooms();
        
        // Destroy all existing rooms.
        HorizontalLayoutGroup[] existingButtons = FindObjectsOfType<HorizontalLayoutGroup>();
        foreach (HorizontalLayoutGroup gameRoomButton in existingButtons)
        {
            Destroy(gameRoomButton.gameObject);
        }

        if (roomResponse.IsSuccessStatusCode())
        {
            foreach (GameRoom room in roomResponse.Response.array)
            {
                // Leave Rooms out which you already joined
                if(room.Players.Exists(x => x.Id == ApplicationState.player.GetId()))
                    continue;
                
                // Create new items from templates
                HorizontalLayoutGroup hlRoomItem = Instantiate(hlRoomItemTemplate, hlRoomItemTemplate.transform.parent, false);
                hlRoomItem.gameObject.SetActive(true);
                
                Button btnContextAction = Instantiate(btnContextActionTemplate, hlRoomItem.transform, false);
                btnContextAction.gameObject.SetActive(true);

                GameRoomButton btnRoomItem = Instantiate(btnRoomItemTemplate, hlRoomItem.transform, false);
                btnRoomItem.gameObject.SetActive(true);
                btnRoomItem.room = room;
                btnRoomItem.GetComponent<Button>().onClick.AddListener(delegate { GoToGameLobby(room); });

                // Set the text
                Text roomMisc = btnRoomItem.transform.Find("Misc").GetComponent<Text>();
                Text roomPlayerCount = btnRoomItem.transform.Find("PlayerCount").GetComponent<Text>();
                Text roomTitle = btnRoomItem.transform.Find("RoomTitle").GetComponent<Text>();
                Text roomRating = btnRoomItem.transform.Find("RatedNumber").GetComponent<Text>();
                Text roomGoal = btnRoomItem.transform.Find("GoalNumber").GetComponent<Text>();

                roomPlayerCount.text = room.Players.Count + " of " + room.Max_Players + " present";
                roomTitle.text = room.Description;
                roomRating.text = room.Rated ? room.Min_Rating.ToString() : "All";
                roomGoal.text = "G:" + room.Goal;
                roomMisc.text = "GameId: " + room.Room_Id + ", Seed: " + room.Seed
                                + ", Created By: " + room.Creator_Id;

                // Set anonymity icon depending on room
                Image roomAnonymity = btnRoomItem.transform.Find("Anonymity").GetComponent<Image>();

                if(room.Anonymity)
                    roomAnonymity.gameObject.SetActive(true);
                else
                    roomAnonymity.gameObject.SetActive(false);
                
            }

            // Create "create rooms" items from templates
            HorizontalLayoutGroup bottomLayoutGroup = Instantiate(hlCreateRoomTemplate, hlCreateRoomTemplate.transform.parent, false);
            bottomLayoutGroup.gameObject.SetActive(true);
            
            Button btnCreatePublicGame = Instantiate(btnCreatePublicGameTemplate, bottomLayoutGroup.transform, false);
            btnCreatePublicGame.gameObject.SetActive(true);
            
            Button btnCreatePrivateGame = Instantiate(btnCreatePrivateGameTemplate, bottomLayoutGroup.transform, false);
            btnCreatePrivateGame.gameObject.SetActive(true);

            
            // TODO: Add some text to notify user they are offline.
        }
    }

    public async void LoadOngoingRooms()
    {
        // Switch button colors
        btnOngoingGames.GetComponentInChildren<Text>().color = activeButtonSelector;
        btnOpenGames.GetComponentInChildren<Text>().color = inactiveButtonSelector;
        btnEndedGames.GetComponentInChildren<Text>().color = inactiveButtonSelector;
        
        NetworkResponse<GameRoomResponse> roomResponse = await api.GetOngoingRooms();
        
        // Destroy all existing rooms.
        HorizontalLayoutGroup[] existingButtons = FindObjectsOfType<HorizontalLayoutGroup>();
        foreach (HorizontalLayoutGroup gameRoomButton in existingButtons)
        {

            Destroy(gameRoomButton.gameObject);
        }

        if (roomResponse.IsSuccessStatusCode())
        {

            foreach (GameRoom room in roomResponse.Response.array)
            {
                // Create new items from templates
                HorizontalLayoutGroup hlRoomItem = Instantiate(hlRoomItemTemplate, hlRoomItemTemplate.transform.parent, false);
                hlRoomItem.gameObject.SetActive(true);
                
                Button btnContextAction = Instantiate(btnContextActionTemplate, hlRoomItem.transform, false);
                btnContextAction.gameObject.SetActive(true);

                GameRoomButton btnRoomItem = (GameRoomButton) Instantiate(btnRoomItemTemplate, hlRoomItem.transform, false);
                btnRoomItem.gameObject.SetActive(true);
                btnRoomItem.room = room;
                btnRoomItem.GetComponent<Button>().onClick.AddListener(delegate { GoToGameLobby(room); });

                // Set the text
                Text roomMisc = btnRoomItem.transform.Find("Misc").GetComponent<Text>();
                Text roomPlayerCount = btnRoomItem.transform.Find("PlayerCount").GetComponent<Text>();
                Text roomTitle = btnRoomItem.transform.Find("RoomTitle").GetComponent<Text>();
                Text roomRating = btnRoomItem.transform.Find("RatedNumber").GetComponent<Text>();
                Text roomGoal = btnRoomItem.transform.Find("GoalNumber").GetComponent<Text>();
                Text contextText = btnContextAction.transform.Find("Join").GetComponent<Text>();


                roomPlayerCount.text = room.Players.Count + " of " + room.Max_Players + " present";
                roomTitle.text = room.Description;
                roomRating.text = room.Rated ? room.Min_Rating.ToString() : "All";
                roomGoal.text = "G:" + room.Goal.ToString();
                roomMisc.text = "GameId: " + room.Room_Id + ", Seed: " + room.Seed
                                + ", Created By: " + room.Creator_Id;

                // Set anonymity icon depending on room
                Image roomAnonymity = btnRoomItem.transform.Find("Anonymity").GetComponent<Image>();
                if(room.Anonymity)
                    roomAnonymity.gameObject.SetActive(true);
                else
                    roomAnonymity.gameObject.SetActive(false);

                // Set context (resign, cancel, leave) depending on room status
                Image roomContext = btnContextAction.transform.Find("Context").GetComponent<Image>();
                if (room.Status.Equals("ongoing"))
                {
                    roomContext.overrideSprite = resign;
                    contextText.text = "resign";
                }
                else if(room.Status.Equals("open") && room.Creator_Id == ApplicationState.player.GetId())
                {
                    roomContext.overrideSprite = cancel;
                    contextText.text = "cancel";
                }
                else
                {
                    roomContext.overrideSprite = leave;
                    contextText.text = "leave";
                }
            }
            HorizontalLayoutGroup bottomLayoutGroup = Instantiate(hlCreateRoomTemplate, hlCreateRoomTemplate.transform.parent, false);
            bottomLayoutGroup.gameObject.SetActive(true);
            
            Button btnCreatePublicGame = Instantiate(btnCreatePublicGameTemplate, bottomLayoutGroup.transform, false);
            btnCreatePublicGame.gameObject.SetActive(true);
            
            Button btnCreatePrivateGame = Instantiate(btnCreatePrivateGameTemplate, bottomLayoutGroup.transform, false);
            btnCreatePrivateGame.gameObject.SetActive(true);
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
