using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Players.Cube
{
    public abstract class BaseCubeling : Atom
    {
        public BasePlayer Spawner { get; set; }
        public abstract bool Pickable { get; }

		protected override void OnAwake()
		{
			gameObject.layer = Globals.CubelingsLayer;
		}
    }

}
