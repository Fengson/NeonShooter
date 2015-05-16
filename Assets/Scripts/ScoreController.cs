﻿using NeonShooter.Players;
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
            player = playerObject.GetComponent<Player>();
        }

        void Update()
        {
            string weaponText = "";
            var selectedWeapon = player.SelectedWeapon.Value;
            if (selectedWeapon != null)
            {
                weaponText = "Weapon:\n" + selectedWeapon.getWeaponName() + "\n";
            }
            lifeText.text = weaponText + "Life: " + CellsIncorporator.amount;
        }
    }
}
