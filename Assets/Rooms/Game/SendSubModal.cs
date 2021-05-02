using System.Collections;
using System.Collections.Generic;
using Rooms.Multiplayer.Game;
using SubterfugeCore.Core.Entities.Positions;
using SubterfugeCore.Core.Entities.Specialists;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class SendSubModal : MonoBehaviour
{

    public GameObject sourceInformation;
    public GameObject subInformation;

    public SpecialistListDisplay SourceSpecialistList;
    public SpecialistListDisplay LaunchSpecialistList;
    public List<Specialist> specialistsToSend = new List<Specialist>();
    public List<Specialist> specialistsAtSource = new List<Specialist>();
    
    public Outpost source;
    public Outpost destination;

    void Start()
    {
        specialistsAtSource.AddRange(source.GetSpecialistManager().GetSpecialists());
        updateSpecialistDisplay();
    }

    private void updateSpecialistDisplay()
    {
        SetSourceSpecialists();
        SetLaunchSpecialists();
    }

    private void SetSourceSpecialists()
    {
        SourceSpecialistList.DisplaySpecialists(specialistsAtSource);
    }
    
    private void SetLaunchSpecialists()
    {
        LaunchSpecialistList.DisplaySpecialists(specialistsToSend);
    }

    public void addSpecialistToSend(SpecialistIconHelper specialist)
    {
        specialistsAtSource.Remove(specialist.specialist);
        specialistsToSend.Add(specialist.specialist);
        updateSpecialistDisplay();
    }
    
    public void removeSpecialistToSend(SpecialistIconHelper specialist)
    {
        specialistsAtSource.Add(specialist.specialist);
        specialistsToSend.Remove(specialist.specialist);
        updateSpecialistDisplay();
    }

}
