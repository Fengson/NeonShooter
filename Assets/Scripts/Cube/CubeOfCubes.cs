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

        public float rotationSpeed;

        public GameObject aim;

        void Start()
        {
            Structure = new CubeStructure(gameObject, radius,
                new RandomOuterLayerCellRetriever());//, part);
            x = radius - 1;
            y = radius - 1;
            z = radius - 1;
            //Structure.SetCell(2, -2, 2, false);
            //Structure.SetCell(-2, -2, 2, false);
            //Structure.SetCell(-1, 2, -2, false);
            //Structure.SetCell(0, 2, 2, false);
            //Structure.SetCell(0, -1, -2, false);
            //Structure.SetCell(2, 0, 2, false);
            //Structure.SetCell(2, 0, 0, false);
            //Structure.SetCell(2, -2, 1, false);
            //Structure.SetCell(1, -2, -1, false);
        }

        void Update()
        {
            //this.aim.transform.Rotate (new Vector3 (0,0,rotationSpeed * Time.deltaTime));

            if (Input.GetKey(KeyCode.Z))
            {

                if (rotationSpeed > -2000)
                    rotationSpeed -= Time.deltaTime * 200;

                //if (x != radius && y != radius && z != radius)
                //{
                //    Structure.SetCell(x, y, z, false);
                //    if (z != (radius - 1) * (-1))
                //        z = z - 1;
                //    else
                //    {
                //        z = radius - 1;
                //        if (x != (radius - 1) * (-1))
                //            x = x - 1;
                //        else
                //        {
                //            x = radius - 1;
                //            if (y != (radius - 1) * (-1))
                //            {
                //                y = y - 1;
                //            }
                //            else
                //            {

                //                Destroy(gameObject);
                //            }
                //        }
                //    }
                //}
                IVector3? cell = Structure.RetrieveCell();
                if (cell == null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Instantiate(part, transform.localPosition + cell.Value, transform.rotation);
                }
            }
            else
            {
                if (rotationSpeed < -90)
                    rotationSpeed += Time.deltaTime * 200;
            }
        }
    }
}