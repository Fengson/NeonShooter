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

        public int Count { 
			get { return cells.Count(); }
		}

        public bool Empty { get { return Count == 0; } }
        public bool Full { get { return Count == Capacity; } }

		public List<String> freeIndexes;

        public CellLayer(int index)
        {
            Index = index;
            Capacity = CalculateCapacity(index);
            cells = new HashSet<CubeCell>();
			freeIndexes = new List<string> ();
			GenerateFreeIndexes (index);

        }

        public bool AddCell(CubeCell cell)
        {
            if (Count == Capacity) return false;

            cells.Add(cell);
			this.freeIndexes.Remove (cell.X + "_" + cell.Y + "_" + cell.Z);
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
			this.freeIndexes.Add (cell.X + "_" + cell.Y + "_" + cell.Z);
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

		private void GenerateFreeIndexes(int index)
		{
			for (int i = -index; i<=index; i++) {
				for(int j = -index; j<=index; j++)
					if(!this.freeIndexes.Contains(index+"_"+i+"_"+j))
						this.freeIndexes.Add(index+"_"+i+"_"+j);
			}

			for (int i = -index; i<=index; i++) {
				for(int j = -index; j<=index; j++)
					if(!this.freeIndexes.Contains(-index+"_"+i+"_"+j))
						this.freeIndexes.Add(-index+"_"+i+"_"+j);
			}

			for (int i = -index; i<=index; i++) {
				for(int j = -index; j<=index; j++)
					if(!this.freeIndexes.Contains(i+"_"+index+"_"+j))
						this.freeIndexes.Add(i+"_"+index+"_"+j);
			}

			for (int i = -index; i<=index; i++) {
				for(int j = -index; j<=index; j++)
					if(!this.freeIndexes.Contains(i+"_"+-index+"_"+j))
						this.freeIndexes.Add(i+"_"+-index+"_"+j);
			}

			for (int i = -index; i<=index; i++) {
				for(int j = -index; j<=index; j++)
					if(!this.freeIndexes.Contains(i+"_"+j+"_"+index))
						this.freeIndexes.Add(i+"_"+j+"_"+index);
			}

			for (int i = -index; i<=index; i++) {
				for(int j = -index; j<=index; j++)
					if(!this.freeIndexes.Contains(i+"_"+j+"_"+-index))
						this.freeIndexes.Add(i+"_"+j+"_"+-index);
			}

		}

		public String getRandomCellSpace()
		{
			return this.freeIndexes[new Random ().Next (freeIndexes.Count)];
		}
    }
}
