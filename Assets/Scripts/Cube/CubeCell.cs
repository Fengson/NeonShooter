using System;
using System.Collections.Generic;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Cube
{
    public class CubeCell
    {
        GameObject cube;
        GameObject sidesParent;
        Dictionary<CubeCellPlaneSide, GameObject> sides;

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public CubeCell(GameObject cube, int x, int y, int z)
        {
            this.cube = cube;
            sidesParent = null;
            sides = new Dictionary<CubeCellPlaneSide, GameObject>();

            X = x;
            Y = y;
            Z = z;
        }

        public bool ContainsSide(CubeCellPlaneSide side)
        {
            return sides.ContainsKey(side);
        }

        public void AddSide(CubeCellPlaneSide side)
        {
            if (ContainsSide(side)) return;

            if (sidesParent == null)
            {
                sidesParent = GameObjectMaker.CreateGameObject(
                    String.Format("CubeCell [ {0}, {1}, {2} ]", X, Y, Z),
                    new Vector3(X, Y, Z), Vector3.zero, Vector3.one);
                GameObjectHelper.SetParentDontMessUpCoords(sidesParent, cube);
            }
            GameObject sideObject = CubeGameObjectMaker.CreateCubeCellPlane(side);
            GameObjectHelper.SetParentDontMessUpCoords(sideObject, sidesParent);

            sides.Add(side, sideObject);
        }

        public void RemoveSide(CubeCellPlaneSide side)
        {
            if (!ContainsSide(side)) return;

            GameObject sideObject = sides[side];
            sides.Remove(side);

            sideObject.SetActive(false);
            GameObject.Destroy(sideObject);

            if (sides.Count == 0)
            {
                GameObject.Destroy(sidesParent);
                sidesParent = null;
            }
        }

        public void ClearSides()
        {
            List<GameObject> sideObjects = new List<GameObject>(sides.Values);
            sides.Clear();

            foreach (GameObject sideObject in sideObjects)
            {
                sideObject.SetActive(false);
                GameObject.Destroy(sideObject);
            }

            GameObject.Destroy(sidesParent);
            sidesParent = null;
        }

        public void TurnSideOnOff(CubeCellPlaneSide side, bool isOn)
        {
            if (isOn) AddSide(side);
            else RemoveSide(side);
        }
    }
}
