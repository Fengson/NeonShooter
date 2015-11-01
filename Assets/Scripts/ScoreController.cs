using NeonShooter.Players;
using UnityEngine;
using UnityEngine.UI;

namespace NeonShooter
{
    public class ScoreController : MonoBehaviour
    {
        Player player;
        public Text healthText;
        public Text ammoText;
        public Text tierText;
        public Image healthBarGraphic;
        Image backgroundOfHud;
        public Image healthIco;

        public Sprite[] weapons;
        public Color[] colorsOfHud;
        public Color actualColor;

        private int tier;
        private int[] tierRanges;



        public GameObject playerObject;

        void Start()
        {
            //lifeText = GetComponent<Text>();
            player = playerObject == null ? null : playerObject.GetComponent<Player>();

            backgroundOfHud = GetComponent<Image>();

            int tierLimit = 3;

            tierRanges = new int[tierLimit];
            tierRanges[0] = 11;
            tierRanges[1] = 101;
            tierRanges[2] = 1001;

            if (colorsOfHud.Length == 0)
            {
                colorsOfHud = new Color[3];
                colorsOfHud[0] = new Color(0.8f, 0.0f, 0.0f);
                colorsOfHud[1] = new Color(0.0f, 0.8f, 0.0f);
                colorsOfHud[2] = new Color(0.0f, 0.0f, 0.8f);
            }

            actualColor = colorsOfHud[0];
            
            tier = 0;

           // transform.FindChild("healthBarGraphic").gameObject.GetComponent<Image>();
        }

        void Update()
        {          

            string weaponText = "";
            /*
            var selectedWeapon = player.SelectedWeapon.Value;
            if (selectedWeapon != null)
            {
                weaponText = "Weapon:\n" + selectedWeapon.getWeaponName() + "\n";
            }*/
            int life = player == null ? CellsIncorporator.amount : player.Life;            
            float healthBarLenght = 0;

            if (life > 0 && life < tierRanges[0])  tier = 0;            
            else if (life >= tierRanges[0] && life < tierRanges[1])  tier = 1;            
            else if (life >= tierRanges[1] && life < tierRanges[2]) tier = 2;

            healthBarLenght = (float)life / (float)tierRanges[tier];



            tierText.text = (tier + 1).ToString();
            actualColor = colorsOfHud[tier];

            //if (healthBarGraphic == null) healthBarGraphic = transform.FindChild("healthBarGraphic").gameObject.GetComponent<Image>();

            healthBarGraphic.rectTransform.localScale = new Vector3(2.2f * healthBarLenght, healthBarGraphic.rectTransform.localScale.y);
            healthBarGraphic.color = actualColor;
            
            healthIco.color = actualColor;
            backgroundOfHud.color = new Color(actualColor.r, actualColor.g, actualColor.b, actualColor.a / 2);


            healthText.text = weaponText + " " + life;
        }
    }
}
