using System.Collections;
using System.Collections.Generic;
using SubterfugeCore.Core.Network;
using UnityEngine;
using UnityEngine.UI;

public class ShowLobbyInfo : MonoBehaviour
{
    //public Text lobbyDetails;
    public VerticalLayoutGroup vlPlayers;
    public VerticalLayoutGroup vlGameInfo;
    public Button btnPlayerTemplate;
    public Button btnStartNow;
    public Button btnInviteYourFriends;
    public Button btnAction;
    
    // Start is called before the first frame update
    void Start()
    {
        this.displayLobbyInformation();
    }

    public void displayLobbyInformation()
    {
        GameRoom room = ApplicationState.currentGameRoom;
        Text playersPresent = vlGameInfo.transform.Find("Players").GetComponent<Text>();
        playersPresent.text = room.Players.Count + " of " + room.Max_Players + " present (x medals)";
        Text goal = vlGameInfo.transform.Find("Goal").GetComponent<Text>();
        goal.text = room.Goal == 1 ? "Mine 200 Neptunium to win" : "Unknown Goal";
        Text ranked = vlGameInfo.transform.Find("Ranked").GetComponent<Text>();
        ranked.text = room.Rated ? "Rated game (" + room.Min_Rating + " min rating to join)" : "Casual game";

        if (!room.Players.Exists(x => x.Id == ApplicationState.player.GetId()))
        {
            btnStartNow.gameObject.SetActive(false);
            btnInviteYourFriends.gameObject.SetActive(false);
            Text actionText = btnAction.transform.Find("ActionText").GetComponent<Text>();
            actionText.text = "Join";
        } else if (room.Creator_Id != ApplicationState.player.GetId())
        {
            btnStartNow.gameObject.SetActive(false);
        }

        //lobbyDetails.text = "Room Id: " + room.Room_Id + "\nTitle: " + room.Description + "\nMap: " + room.Map + "\nAnonymous: " + room.Anonymity + "\nCreator: " + room.Creator_Id + "\nGoal: " + room.Goal + "\nRanked: " + room.Rated + "\nMinimum Rank: " + room.Min_Rating + "\nPlayers (" + room.Players.Count + "):";
        foreach (NetworkUser netUser in room.Players)
        {
            Button btnPlayer = Instantiate(btnPlayerTemplate, vlPlayers.transform, false);
            btnPlayer.gameObject.SetActive(true);
            Text playerName = btnPlayer.transform.Find("PlayerName").GetComponent<Text>();
            playerName.text = netUser.Name;
        }

        int spotsFree = room.Max_Players - room.Players.Count;
        for (int i = 0; i < spotsFree; i++)
        {
            Button btnPlayerNotAssigned = Instantiate(btnPlayerTemplate, vlPlayers.transform, false);
            btnPlayerNotAssigned.gameObject.SetActive(true);
            btnPlayerNotAssigned.interactable = false;
        }
    }
}
