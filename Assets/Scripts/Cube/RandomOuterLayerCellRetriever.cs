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

                IVector3? position = layer.GetRandomCellSpace();
                if (!position.HasValue) break;

                structure.SetCell(position.Value, false);
                removedCells.Add(position.Value);

                if (structure.CanShrink()) structure.Shrink();
            }

            return removedCells;
        }
    }
}
