using UnityEngine;
using System.Collections;
using NeonShooter.Utils;

namespace NeonShooter.Cube
{
    public class CubeOfCubes : MonoBehaviour
    {
        public CubeStructure Structure { get; private set; }
        public int radius;
        public GameObject part;
        public int x, y, z;

        void Start()
        {
            Structure = new CubeStructure(gameObject, radius, part);
            x = radius - 1;
            y = radius - 1;
            z = radius - 1;
            Structure.SetCell(2, -2, 2, false);
            Structure.SetCell(-2, -2, 2, false);
            Structure.SetCell(-1, 2, -2, false);
            Structure.SetCell(0, 2, 2, false);
            Structure.SetCell(0, -1, -2, false);
            Structure.SetCell(2, 0, 2, false);
            Structure.SetCell(2, 0, 0, false);
            Structure.SetCell(2, -2, 1, false);
            Structure.SetCell(1, -2, -1, false);
            
        }

        void Update()
		{

            if (Input.GetKey (KeyCode.Z)) {

				if (x != radius && y != radius && z != radius) {
					Structure.SetCell (x, y, z, false);
					if (z != (radius - 1) * (-1))
						z = z - 1;
					else {
						z = radius - 1;
						if (x != (radius - 1) * (-1))
							x = x - 1;
						else {
							x = radius - 1;
							if (y != (radius - 1) * (-1)) {
								y = y - 1;
							} else {

								Destroy (gameObject);
							}
						}
					}
				}
			}
        }
    }
}