﻿using System.Collections.Generic;
using UnityEngine;
using SubterfugeCore.Core;
using SubterfugeCore.Core.Entities.Positions;
using SubterfugeCore.Core.Generation;
using SubterfugeCore.Core.Players;
using SubterfugeCore.Core.Topologies;
using TMPro;

public class OutpostSpawn : MonoBehaviour
{
    public Transform outpostPrefab;
    public Transform outpostObject;
    private System.Random rand = new System.Random(0);
    List<Player> players = new List<Player>();


    // Start is called before the first frame update
    void Start()
    {
        Game server = ApplicationState.CurrentGame;
        GameConfiguration config = server.Configuration;
        List<Outpost> outposts;
        List<Vector3> outpostLocations = new List<Vector3>();
        outposts = ApplicationState.CurrentGame.TimeMachine.GetState().GetOutposts();

        foreach (Outpost outpost in outposts)
        {

            Sprite sprite = getOutpostSprite(outpost.GetOutpostType());
            
            // Spawn in all 9 quadrants for map wrapping.
            for (var vertical = 0; vertical < 3; vertical++)
            {
                for (var horizontal = 0; horizontal < 3; horizontal++)
                {
                    Vector2 location = new Vector2(outpost.GetCurrentPosition().X, outpost.GetCurrentPosition().Y);
                    if (vertical == 0)
                    {
                        location.y -= RftVector.Map.Height;
                    }

                    if (vertical == 2)
                    {
                        location.y += RftVector.Map.Height;
                    }

                    if (horizontal == 0)
                    {
                        location.x -= RftVector.Map.Width;
                    }

                    if (horizontal == 2)
                    {
                        location.x += RftVector.Map.Width;
                    }
                    
                    // Create a game object.
                    outpostObject = Instantiate(outpostPrefab, location, Quaternion.identity);
                    
                    // Set the outpost sprite
                    outpostObject.GetComponent<SpriteRenderer>().sprite = sprite;
                    
                    // Set the outpost and id in the manager.
                    outpostObject.GetComponent<OutpostManager>().ID = outpost.GetId();
                    outpostObject.GetComponent<OutpostManager>().outpost = outpost;
                    
                    outpostLocations.Add(location);
                    
                }
            }
        }

        Debug.Log("Spawned outposts");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite getOutpostSprite(OutpostType type)
    {
        Sprite sprite = Resources.Load<Sprite>("Locations/Unknown");;
        switch (type)
        {
            case OutpostType.Generator:
                sprite = Resources.Load<Sprite>("Locations/GeneratorFill");
                break;
            case OutpostType.Destroyed:
                sprite = Resources.Load<Sprite>("Locations/Destroyed");
                break;
            case OutpostType.Factory:
                sprite = Resources.Load<Sprite>("Locations/FactoryFill");
                break;
            case OutpostType.Mine:
                sprite = Resources.Load<Sprite>("Locations/MineFill");
                break;
            case OutpostType.Watchtower:
                sprite = Resources.Load<Sprite>("Locations/Watchtower");
                break;
            default:
                sprite = Resources.Load<Sprite>("Locations/Unknown");
                break;
        }

        return sprite;
    }
}
