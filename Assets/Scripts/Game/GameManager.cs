﻿using System.Linq;
using GameEventModels;
using Google.Protobuf;
using SubterfugeCore.Core;
using SubterfugeCore.Core.Entities.Positions;
using SubterfugeCore.Core.GameEvents;
using SubterfugeCore.Core.Timing;
using SubterfugeRemakeService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool showLaunchHud = false;
    public GameObject launchHud;
    public Outpost launchOutpost;
    public Outpost destinationOutpost;
    public Slider drillerSlider;
    
    // Start is called before the first frame update
    async void Start()
    {
        launchHud.SetActive(false);
        // TODO: Add a configuration to the `Game` object to determine if it is a multiplayer game.
        if (ApplicationState.CurrentGame != null && ApplicationState.CurrentGame.Configuration.IsMultiplayer)
        {
            loadMultiplayerGame();
        }
        else
        {
            loadSinglePlayerGame();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the pressed location was an outpost. If it was, the user is trying to launch a sub.
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "Outpost")
            {
                // Clicked object is an outpost, don't move the camera.
                launchOutpost = hit.collider.gameObject.GetComponent<OutpostManager>().outpost;
                return;
            }   
        }
        // If the mouse button is released, apply velocity to the map to scroll
        if (Input.GetMouseButtonUp(0))
        {
            // If the first click was on an outpost, check if the second is on another outpost for a launch.
            if (launchOutpost != null && showLaunchHud == false)
            {
                // Check if the pressed location was an outpost. If it was, the user is trying to launch a sub.
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject.tag == "Outpost")
                {
                    // Clicked object is an outpost, don't move the camera.
                    destinationOutpost = hit.collider.gameObject.GetComponent<OutpostManager>().outpost;
                    
                    // only show the hud if the souce outpost is owned by the current player & the destination is not the source.
                    if (launchOutpost != destinationOutpost &&
                        launchOutpost.GetOwner().GetId() == ApplicationState.player.GetId())
                    {

                        SouceLaunchInformation sourcePanel = launchHud.GetComponentInChildren<SouceLaunchInformation>();
                        sourcePanel.source = launchOutpost;
                        SubLaunchInformation informationPanel = launchHud.GetComponentInChildren<SubLaunchInformation>();
                        informationPanel.destination = destinationOutpost;
                        informationPanel.sourceOutpost = launchOutpost;
                        drillerSlider.maxValue = launchOutpost.GetDrillerCount();

                        this.SetLaunchHub(true);
                    }
                    else
                    {
                        launchOutpost = null;
                    }
                }
            } else if (showLaunchHud)
            {
                // Determine if the click was in the panel
                if (EventSystem.current.IsPointerOverGameObject()) return;
                this.SetLaunchHub(false);
            }
        }
    }

    public void AdvanceTimemachine(int ticks)
    {
        ApplicationState.CurrentGame.TimeMachine.Advance(ticks);
    }

    public void SetLaunchHub(bool state)
    {
        showLaunchHud = state;
        launchHud.SetActive(state);
    }

    public void launchSub()
    {
        LaunchEvent launchEvent = new LaunchEvent(
            new GameEventModel()
            {
                EventData = new LaunchEventData()
                {
                    SourceId = launchOutpost.GetId(),
                    DestinationId = destinationOutpost.GetId(),
                    DrillerCount = (int) drillerSlider.value,
                    SpecialistIds = { },
                }.ToByteString(),
                OccursAtTick = ApplicationState.CurrentGame.TimeMachine.GetCurrentTick().GetTick(),
            });
        
        ApplicationState.CurrentGame.TimeMachine.AddEvent(launchEvent);
        this.SetLaunchHub(false);

        // Submit event to online services.
        if (ApplicationState.CurrentGame.Configuration.IsMultiplayer)
        {
            var client = ApplicationState.Client.getClient();

            var response = client.SubmitGameEvent(new SubmitGameEventRequest()
            {
                EventData = new GameEventRequest() {
                    EventData = launchEvent.ToGameEventModel().ToByteString(),
                    EventType = launchEvent.GetEventType(),
                    OccursAtTick = launchEvent.GetOccursAt().GetTick(),
                },
                RoomId = ApplicationState.currentGameRoom.RoomId
            });

            if (!response.Status.IsSuccess)
            {
                ApplicationState.CurrentGame.TimeMachine.RemoveEvent(launchEvent);
                // Indicate an error, don't add to time machine.
            }
        }
    }
    
    public void loadMultiplayerGame()
    {

        if (!ApplicationState.Client.isConnected)
        {
            // Show info here to try reconnect.
        }
        else
        {
            var gameEvents = ApplicationState.Client.getClient().GetGameRoomEvents(
                new GetGameRoomEventsRequest()
                {
                    RoomId = ApplicationState.currentGameRoom.RoomId,
                });

            if (gameEvents.Status.IsSuccess)
            {
                ApplicationState.CurrentGame.LoadGameEvents(gameEvents.GameEvents.ToList());
            } else {
                // TODO: Tell the user that they are offline or an error occurred.
            }
                
            // go to current tick.
            GameTick tick = new GameTick(ApplicationState.CurrentGame.Configuration.StartTime, NtpConnector.GetNetworkTime());
            ApplicationState.CurrentGame.TimeMachine.GoTo(tick);
        }
    }

    public void loadSinglePlayerGame()
    {
            
    }
    
    
}
