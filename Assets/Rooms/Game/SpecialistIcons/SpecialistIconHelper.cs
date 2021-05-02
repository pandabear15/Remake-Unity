using SubterfugeCore.Core.Entities.Specialists;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Rooms.Multiplayer.Game
{
    public class SpecialistIconHelper : MonoBehaviour
    {
        public Image specialistIconImage;
        public Specialist specialist;

        public void setImage(Sprite sprite)
        {
            specialistIconImage.sprite = sprite;
        }

    }
}