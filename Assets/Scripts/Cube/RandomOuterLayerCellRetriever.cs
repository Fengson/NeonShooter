using NeonShooter.Utils;
using System.Collections.Generic;

namespace NeonShooter.Cube
{
    public class RandomOuterLayerCellRetriever : ICellRetriever
    {
        public List<IVector3> RetrieveCells(CubeStructure structure, int count)
        {
            var removedCells = new List<IVector3>();

            for (int i = 0; i < count; i++)
            {
                var layer = structure.GetLastLayer();
                if (layer == null) break;

                CubeCell cell = layer.GetRandomCell();
                structure.SetCell(cell.X, cell.Y, cell.Z, false);
                removedCells.Add(new IVector3(cell.X, cell.Y, cell.Z));

                if (structure.CanShrink()) structure.Shrink();
            }

            return removedCells;
        }
    }
}
