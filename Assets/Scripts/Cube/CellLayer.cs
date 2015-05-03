using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonShooter.Cube
{
    public class CellLayer
    {
        HashSet<CubeCell> cells;

        public int Index { get; private set; }
        public int Capacity { get; private set; }

        public int Count { get { return cells.Count; } }

        public bool Empty { get { return Count == 0; } }
        public bool Full { get { return Count == Capacity; } }

        public CellLayer(int index)
        {
            Index = index;
            Capacity = CalculateCapacity(index);

            cells = new HashSet<CubeCell>();
        }

        public bool AddCell(CubeCell cell)
        {
            if (Count == Capacity) return false;

            cells.Add(cell);
            return true;
        }

        public CubeCell GetRandomCell()
        {
            if (Count == 0) return null;
            return cells.ElementAt(new Random().Next(Count));
        }

        public void RemoveCell(CubeCell cell)
        {
            cells.Remove(cell);
        }

        private static int CalculateCapacity(int index)
        {
            int diameter = index * 2 + 1;
            int capacity = diameter * diameter * diameter;
            if (index == 0) return capacity;

            int prevDiameter = diameter - 2;
            capacity -= prevDiameter * prevDiameter * prevDiameter;
            return capacity;
        }
    }
}
