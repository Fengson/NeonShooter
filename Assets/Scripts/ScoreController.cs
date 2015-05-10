using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace NeonShooter
{
    public class ScoreController : MonoBehaviour
    {
        public Text lifeText;
        int life = 125;

        void Start()
        {
            lifeText = GetComponent<Text>();
        }

        void Update()
        {
            string weaponText = "";
            if(CellsIncorporator.selectedWeapon!=null) {
                weaponText="Weapon:\n"+ CellsIncorporator.selectedWeapon.getWeaponName()+"\n";
            }
            lifeText.text = weaponText + "Life: " + (life + CellsIncorporator.amount);
        }
    }
}
