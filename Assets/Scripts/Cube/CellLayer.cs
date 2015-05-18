using NeonShooter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Cube
{
    public class CellLayer
    {
        List<IVector3> cellSpaces;
        List<IVector3> freeSpaces;

        public int Index { get; private set; }
        public int Capacity { get; private set; }

        public int CellSpacesCount { get { return cellSpaces.Count; } }
        public int FreeSpacesCount { get { return freeSpaces.Count; } }

        public bool Empty { get { return CellSpacesCount == 0; } }
        public bool Full { get { return FreeSpacesCount == 0; } }

        public IEnumerable<IVector3> CellSpacesEnumerable { get { return cellSpaces; } }
        public IEnumerable<IVector3> FreeSpacesEnumerable { get { return freeSpaces; } }

        public CellLayer(int index)
        {
            cellSpaces = new List<IVector3>();
            freeSpaces = new List<IVector3>();

            Index = index;
            Capacity = CalculateCapacity(index);

            InitializeLists();
        }

        public bool AddCellSpace(IVector3 space)
        {
            if (CellSpacesCount == Capacity) return false;
            if (!freeSpaces.Contains(space)) return false;

            freeSpaces.Remove(space);
            cellSpaces.Add(space);

            return true;
        }

        public bool RemoveCellSpace(IVector3 space)
        {
            if (FreeSpacesCount == Capacity) return false;
            if (!cellSpaces.Contains(space)) return false;

            cellSpaces.Remove(space);
            freeSpaces.Add(space);

            return true;
        }

        public IVector3? GetRandomCellSpace()
        {
            if (CellSpacesCount == 0) return null;
            return cellSpaces[new Random().Next(CellSpacesCount)];
        }

        public IVector3? GetRandomFreeSpace()
        {
            if (FreeSpacesCount == 0) return null;
            return freeSpaces[new Random().Next(FreeSpacesCount)];
        }

        private static int CalculateCapacity(int index)
        {
            int diameter = index * 2 + 1;
            int capacity = MathHelper.IntPow(diameter, 3);

            if (index == 0) return capacity;

            int prevDiameter = diameter - 2;
            capacity -= MathHelper.IntPow(prevDiameter, 3);
            return capacity;
        }

        private void InitializeLists()
        {
            if (Index == 0)
            {
                freeSpaces.Add(IVector3.Zero);
                return;
            }

            for (int x = -Index; x <= Index; x++)
                for (int y = -Index; y <= Index; y++)
                {
                    freeSpaces.Add(new IVector3(x, y, Index));
                    freeSpaces.Add(new IVector3(x, y, -Index));
                }

            for (int y = -Index; y <= Index; y++)
                for (int z = -Index + 1; z < Index; z++)
                {
                    freeSpaces.Add(new IVector3(Index, y, z));
                    freeSpaces.Add(new IVector3(-Index, y, z));
                }

            for (int z = -Index + 1; z < Index; z++)
                for (int x = -Index + 1; x < Index; x++)
                {
                    freeSpaces.Add(new IVector3(x, Index, z));
                    freeSpaces.Add(new IVector3(x, -Index, z));
                }
        }
    }
}
