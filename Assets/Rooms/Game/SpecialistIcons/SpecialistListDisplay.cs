using System.Collections.Generic;
using System.Linq;
using SubterfugeCore.Core.Entities.Specialists;
using UnityEngine;
using UnityEngine.UI;

namespace Rooms.Multiplayer.Game
{
    public class SpecialistListDisplay : MonoBehaviour
    {
        public Canvas specialistListDisplay;
        public Image specialistIconPrefab;

        private Dictionary<Specialist, Image> currentlyDisplayed = new Dictionary<Specialist, Image>();

        public void DisplaySpecialists(List<Specialist> specs)
        {
            foreach (Specialist s in specs.ToList())
            {
                if (currentlyDisplayed.ContainsKey(s))
                    continue;

                Image specialistIconHolder = Instantiate(specialistIconPrefab);
                SpecialistIconHelper helper = specialistIconHolder.GetComponentInChildren<SpecialistIconHelper>();
                helper.specialist = s;
                helper.setImage(Resources.Load<Sprite>("Specialists/Queen"));
                specialistIconHolder.transform.SetParent(specialistListDisplay.transform);
                specialistIconHolder.transform.gameObject.SetActive(true);
                currentlyDisplayed.Add(s, specialistIconHolder);
            }

            foreach (Specialist s in currentlyDisplayed.Keys.ToList())
            {
                if (specs.Contains(s))
                    continue;

                // Remove the specialist icon from being displayed.
                Destroy(currentlyDisplayed[s]);
                currentlyDisplayed.Remove(s);
            }
        }
    }
}