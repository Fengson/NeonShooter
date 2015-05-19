using NeonShooter.Utils;
using System.Collections.Generic;

namespace NeonShooter.Players.Cube
{
    public interface ICubeStructureCellsModifier
    {
        List<IVector3> ModifyCells(CubeStructure structure, int count);
    }
}
