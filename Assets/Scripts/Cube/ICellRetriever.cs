using NeonShooter.Utils;
using System.Collections.Generic;

namespace NeonShooter.Cube
{
    public interface ICellRetriever
    {
        List<IVector3> RetrieveCells(CubeStructure structure, int count);
    }
}
