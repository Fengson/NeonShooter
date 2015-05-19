﻿using UnityEngine;
using System.Collections;
using NeonShooter.Utils;

namespace NeonShooter.Players.Cube
{
    public class CubeOfCubes : MonoBehaviour
    {
        public int radius;
        public GameObject part;

        public bool targeted;

        public CubeStructure Structure { get; private set; }

        void Start()
        {
            Structure = new CubeStructure(gameObject, radius);
            Structure.CellRetriever = new RandomOuterLayerCellRetriever();
            Structure.CellAppender = new RandomOuterLayerCellAppender();

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
            if (Input.GetKey(KeyCode.Z))
            {
                if (targeted)
                {
                    IVector3? cell = RemoveCubeling();
                    if (cell == null) Destroy(gameObject);
                    else Instantiate(part, transform.localPosition + cell.Value, transform.rotation);
                }
            }
        }

        public IVector3? AddCubeling()
        {
            if (Structure == null) return null;
            return Structure.AppendCell();
        }

        public IVector3? RemoveCubeling()
        {
            if (Structure == null) return null;
            return Structure.RetrieveCell();
        }
    }
}