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

        private int lastReadedHealth;

        public GameObject playerObject;

        void Start()
        {
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

            this.player = this.playerObject.GetComponent<Player>();

            UpdateHud();

        }



        void Update()
        {
            if (lastReadedHealth != player.Life)
            {
                
                this.UpdateHud();
            }
            
        }

        void UpdateHud()
        {

            string weaponText = "";
            print("actual life:" + player.Life);

            int life = //player == null ? CellsIncorporator.amount : 
                player.Life; // print powyzej i tak by rzucil NullReferenceException,
            // zamiast tego lepiej zrobic w metodzie Start() sprawdzanie, czy player jest nullem
            // i rzucanie tam bledu czy jakiegos komunikatu ze nie moze byc nullem.

            float healthBarLenght = 0;

            if (life > 0 && life < tierRanges[0]) tier = 0;
            else if (life >= tierRanges[0] && life < tierRanges[1]) tier = 1;
            else if (life >= tierRanges[1] && life < tierRanges[2]) tier = 2;

            healthBarLenght = (float)life / (float)tierRanges[tier];
            
            tierText.text = (tier + 1).ToString();
            actualColor = colorsOfHud[tier];

            healthBarGraphic.rectTransform.localScale = new Vector3(2.2f * healthBarLenght, healthBarGraphic.rectTransform.localScale.y);
            healthBarGraphic.color = actualColor;

            healthIco.color = actualColor;
            backgroundOfHud.color = new Color(actualColor.r, actualColor.g, actualColor.b, actualColor.a / 2);

            healthText.text = weaponText + " " + life;

            this.lastReadedHealth = life;
        }
        
    }

}
