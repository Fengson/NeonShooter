using NeonShooter.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace NeonShooter.Players.Cube
{
    public class RandomOuterLayerCellRetriever : ICubeStructureCellsModifier
    {
        public List<IVector3> ModifyCells(CubeStructure structure, int count)
        {
            var removedCells = new List<IVector3>();

            for (int i = 0; i < count; i++)
            {
                var layer = structure.GetLastLayer();
                if (layer == null) break;

                IVector3? position = layer.GetRandomCellSpace();
                if (!position.HasValue) break;

                structure.SetCell(position.Value, false);
                removedCells.Add(position.Value);

                structure.TryShrink();
            }

            return removedCells;
        }
    }
}
