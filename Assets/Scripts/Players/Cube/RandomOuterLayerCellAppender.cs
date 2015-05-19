using NeonShooter.Utils;
using System;
using System.Collections.Generic;

namespace NeonShooter.Players.Cube
{
    public class RandomOuterLayerCellAppender : ICubeStructureCellsModifier
    {
        public List<IVector3> ModifyCells(CubeStructure structure, int count)
        {
            var addedCells = new List<IVector3>();

            for (int i = 0; i < count; i++)
            {
                structure.TryExpand();

                var layer = structure.GetLastLayer();
                if (layer == null) 
                    throw new Exception("Expanded, but last layer is still null - this should never happen!");

                IVector3? position = layer.GetRandomFreeSpace();
                if (!position.HasValue)
                    throw new Exception("Expanded, but last layer is still full - this should never happen!");

                structure.SetCell(position.Value, true);
                addedCells.Add(position.Value);
            }

            return addedCells;
        }
    }
}
