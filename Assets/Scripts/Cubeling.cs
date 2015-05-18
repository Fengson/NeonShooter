using UnityEngine;
using System.Collections;
using NeonShooter.Players;
using NeonShooter.Utils;

namespace NeonShooter
{
    public class Cubeling : MonoBehaviour
    {
        public float speed;

        void Update()
        {
            GameObject player = GameObject.FindWithTag("Player");
            float step = speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            transform.eulerAngles = new Vector3(1, 1, 1);
        }
    }
}
