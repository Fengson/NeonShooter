using NeonShooter.Utils;
using System.Collections.Generic;

namespace NeonShooter.Cube
{
    public interface ICubeStructureCellsModifier
    {
        List<IVector3> ModifyCells(CubeStructure structure, int count);
    }
}
