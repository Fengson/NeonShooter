using System;
using System.Collections.Generic;
using NeonShooter.Utils;
using UnityEngine;

namespace NeonShooter.Cube
{
    public class CubeCell
    {
        CubeStructure parentStructure;

        GameObject owner;
        GameObject sidesParent;
        Dictionary<CubeCellPlaneSide, GameObject> sides;

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }
        public int Z { get { return Position.Z; } }

        public IVector3 Position { get; private set; }

        public CubeCell(CubeStructure parentStructure, GameObject owner, IVector3 position)
        {
            this.parentStructure = parentStructure;
            this.owner = owner;
            sidesParent = null;
            sides = new Dictionary<CubeCellPlaneSide, GameObject>();

            Position = position;
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
                GameObjectHelper.SetParentDontMessUpCoords(sidesParent, owner);
            }
            GameObject sideObject = CubeGameObjectMaker.CreateCubeCellPlane(side, parentStructure.Visible);
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

        public void UpdateVisible()
        {
            Material material = (parentStructure.Visible || Globals.Instance == null) ?
                null : Globals.Instance.invisibleShadowCasterMaterial;
            if (material == null) material = Globals.DefaultMaterial;
            foreach (var side in sides.Values)
            {
                var renderer = side.GetComponent<MeshRenderer>();
                renderer.material = material;
            }
        }
    }
}
