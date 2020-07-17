using System.Collections;
using System.Collections.Generic;
using SubterfugeCore.Core.Network;
using UnityEngine;
using UnityEngine.UI;

public class ShowLobbyInfo : MonoBehaviour
{
    public Text lobbyDetails;
    
    // Start is called before the first frame update
    void Start()
    {
        this.displayLobbyInformation();
    }

    public void displayLobbyInformation()
    {
        GameRoom room = ApplicationState.currentGameRoom;
        lobbyDetails.text = "Room Id: " + room.Room_Id + "\nTitle: " + room.Description + "\nMap: " + room.Map + "\nAnonymous: " + room.Anonymity + "\nCreator: " + room.Creator_Id + "\nGoal: " + room.Goal + "\nRanked: " + room.Rated + "\nMinimum Rank: " + room.Min_Rating + "\nPlayers (" + room.Players.Count + "):";
        foreach (NetworkUser netUser in room.Players)
        {
            lobbyDetails.text = lobbyDetails.text + "\n" + netUser.Name;
        }
    }
}
