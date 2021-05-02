using System.Collections;
using System.Collections.Generic;
using Rooms.Multiplayer.Game;
using SubterfugeCore.Core.Entities;
using SubterfugeCore.Core.Entities.Positions;
using SubterfugeCore.Core.Entities.Specialists;
using SubterfugeCore.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class SouceLaunchInformation : MonoBehaviour
{
    public Outpost source;

    // Update is called once per frame
    void Update()
    {
        Text text = gameObject.GetComponentInChildren<Text>();
        text.text = "====Source Outpost====\n" +
                    "Outpost Id: " + source.GetId() + "\n" +
                    "Shields: " + source.GetShieldManager().GetShields() + "\n" +
                    "Drillers: " + source.GetDrillerCount() + "\n" +
                    "Specialists: " + source.GetSpecialistManager().GetSpecialistCount();

        foreach (Specialist s in source.GetSpecialistManager().GetSpecialists())
        {
            SpecialistListDisplay specialistListDisplay = GetComponentInChildren<SpecialistListDisplay>();
            specialistListDisplay.DisplaySpecialists(source.GetSpecialistManager().GetSpecialists());
        }
    }
}
