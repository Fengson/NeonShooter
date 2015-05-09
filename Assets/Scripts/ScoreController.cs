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
                weaponText="\nWeapon:\n"+ CellsIncorporator.selectedWeapon.getWeaponName();
            }
            lifeText.text = "Life: " + (life + CellsIncorporator.amount)+ weaponText;
        }
    }
}
