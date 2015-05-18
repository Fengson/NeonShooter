using NeonShooter.Players;
using UnityEngine;
using UnityEngine.UI;

namespace NeonShooter
{
    public class ScoreController : MonoBehaviour
    {
        Player player;
        Text lifeText;

        public GameObject playerObject;

        void Start()
        {
            lifeText = GetComponent<Text>();
            player = playerObject == null ? null : playerObject.GetComponent<Player>();
        }

        void Update()
        {
            string weaponText = "";
            var selectedWeapon = player.SelectedWeapon.Value;
            if (selectedWeapon != null)
            {
                weaponText = "Weapon:\n" + selectedWeapon.getWeaponName() + "\n";
            }
            int life = player == null ? CellsIncorporator.amount : player.Life;
            lifeText.text = weaponText + "Life: " + life;
        }
    }
}
