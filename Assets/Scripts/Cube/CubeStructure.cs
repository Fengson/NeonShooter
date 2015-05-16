using NeonShooter.Utils;
using NeonShooter.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonShooter.Cube
{
    /// <summary>
    /// Class representing structure of the cube with given radius. Radius is its most important property. Instance of this class can be adressed with index operator [x, y, z] to get CubeCell at given coordinates of the structure. Note that coords must always be lower than the Radius. When the Radius is 0, there are no cells. When the Radius is 1, it's a cube 1x1x1 and max (and only) absolute value of coord is 0. When the Radius is 2, it's a cube 3x3x3 and max absolute value of coord is 1. And so on.
    /// </summary>
    public class CubeStructure
    {
        GameObject cube;

        TwoWayList<TwoWayList<TwoWayList<CubeCell>>> cells;
        List<CellLayer> cellLayers;

        /// <summary>
        /// Current radius of this CubeStructure. It cannot be changed explicitly. Instead use Expand() and Shrink() methods.
        /// </summary>
        public int Radius { get; private set; }

        public int Count { get; private set; }

        public ICubeStructureCellsModifier CellRetriever { get; set; }
        public ICubeStructureCellsModifier CellAppender { get; set; }

        public SizeChangeBehaviour UnwantedSizeChangeBehaviour { get; set; }

        /// <summary>
        /// Creates new CubeStructure with given initial radius, and sets all cells active.
        /// </summary>
        /// <param name="initialRadius">Initial radius of the CubeStructure.</param>
        public CubeStructure(GameObject cube, int initialRadius)
        {
            if (initialRadius < 0)
                throw new ArgumentException("Argument initialRadius must not be lower than 0.");

            this.cube = cube;
            Radius = initialRadius;
            Count = MathHelper.IntPow(Radius + 1, 3);

            cells = CreateCellXYZCube(Radius);
            cellLayers = new List<CellLayer>();
            for (int i = 0; i < Radius; i++)
                cellLayers.Add(new CellLayer(i));

            ForEachCell((v) =>
                {
                    var cell = new CubeCell(cube, v);
                    cells[v.X][v.Y][v.Z] = cell;
                    cellLayers[GetLayerIndex(v)].AddCellSpace(v);
                });
            UpdateLastLayersSides();

            UnwantedSizeChangeBehaviour = SizeChangeBehaviour.Exception;
        }

        /// <summary>
        /// Allows to get the CubeCell of this CubeStructure at given coordinates. Throws IndexOutOfBoundsException, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="x">X coord of the cell.</param>
        /// <param name="y">Y coord of the cell.</param>
        /// <param name="z">Z coord of the cell.</param>
        public CubeCell this[int x, int y, int z]
        {
            get { return GetCell(x, y, z); }
        }

        /// <summary>
        /// Allows to get the CubeCell of this CubeStructure at given coordinates. Throws IndexOutOfBoundsException, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="position">X, Y and Z coords of the cell.</param>
        public CubeCell this[IVector3 position]
        {
            get { return this[position.X, position.Y, position.Z]; }
        }

        /// <summary>
        /// Allows to get the CubeCell of this CubeStructure at given coordinates. Throws IndexOutOfBoundsException, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="x">X coord of the cell.</param>
        /// <param name="y">Y coord of the cell.</param>
        /// <param name="z">Z coord of the cell.</param>
        public CubeCell GetCell(int x, int y, int z)
        {
            return cells[x][y][z];
        }

        /// <summary>
        /// Allows to get the CubeCell of this CubeStructure at given coordinates. Throws IndexOutOfBoundsException, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="position">X, Y and Z coords of the cell.</param>
        public CubeCell GetCell(IVector3 position)
        {
            return GetCell(position.X, position.Y, position.Z);
        }
        
        /// <summary>
        /// Allows to set the CubeCell of this CubeStructure at given coordinates. Throws IndexOutOfBoundsException, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="x">X coord of the cell.</param>
        /// <param name="y">Y coord of the cell.</param>
        /// <param name="z">Z coord of the cell.</param>
        /// <param name="cellValue">If true, the cell will be created at given coords. If false, it will be erased.</param>
        public void SetCell(int x, int y, int z, bool cellValue)
        {
        }

        /// <summary>
        /// Allows to set the CubeCell of this CubeStructure at given coordinates. Throws IndexOutOfBoundsException, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="position">X, Y and Z coords of the cell.</param>
        /// <param name="cellValue">If true, the cell will be created at given coords. If false, it will be erased.</param>
        public void SetCell(IVector3 position, bool cellValue)
        {
            CubeCell oldValue = cells[position.X][position.Y][position.Z];
            if (cellValue == (oldValue != null)) return;

            cells[position.X][position.Y][position.Z] = cellValue ? new CubeCell(cube, position) : null;
            if (!cellValue) oldValue.ClearSides();

            int layerIndex = GetLayerIndex(position);
            if (cellValue) cellLayers[layerIndex].AddCellSpace(position);
            else cellLayers[layerIndex].RemoveCellSpace(position);

            UpdateSides(position);
            UpdateNeighboursSides(position);
        }

        /// <summary>
        /// Gets the CubeCell of this CubeStructure at given coordinates. Returns null, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="x">X coord of the cell.</param>
        /// <param name="y">Y coord of the cell.</param>
        /// <param name="z">Z coord of the cell.</param>
        public CubeCell TryGetCell(int x, int y, int z)
        {
            return TryGetCell(new IVector3(x, y, z));
        }

        /// <summary>
        /// Gets the CubeCell of this CubeStructure at given coordinates. Returns null, if any coord is not lower than Radius.
        /// </summary>
        /// <param name="position">X, Y and Z coords of the cell.</param>
        public CubeCell TryGetCell(IVector3 position)
        {
            if (IsOutOfRadius(position)) return null;

            return GetCell(position);
        }

        /// <summary>
        /// Sets the CubeCell of this CubeStructure at given coordinates. Returns false, if any coord is not lower than Radius, otherwise true.
        /// </summary>
        /// <param name="x">X coord of the cell.</param>
        /// <param name="y">Y coord of the cell.</param>
        /// <param name="z">Z coord of the cell.</param>
        /// <param name="cellValue">If true, the cell will be created at given coords. If false, it will be erased.</param>
        public bool TrySetCell(int x, int y, int z, bool cellValue)
        {
            return TrySetCell(new IVector3(x, y, z), cellValue);
        }

        /// <summary>
        /// Sets the CubeCell of this CubeStructure at given coordinates. Returns false, if any coord is not lower than Radius, otherwise true.
        /// </summary>
        /// <param name="position">X, Y and Z coords of the cell.</param>
        /// <param name="cellValue">If true, the cell will be created at given coords. If false, it will be erased.</param>
        public bool TrySetCell(IVector3 position, bool cellValue)
        {
            if (IsOutOfRadius(position)) return false;

            SetCell(position, cellValue);
            return true;
        }

        public int GetLayerIndex(IVector3 position)
        {
            return MathHelper.Max(Math.Abs(position.X), Math.Abs(position.Y), Math.Abs(position.Z));
        }

        public CellLayer GetLayer(int index)
        {
            return cellLayers[index];
        }

        public CellLayer GetLastLayer()
        {
            return cellLayers.LastOrDefault();
        }

        public CellLayer GetLayer(int x, int y, int z)
        {
            return GetLayer(GetLayerIndex(new IVector3(x, y, z)));
        }

        /// <summary>
        /// Retrieves the information, whether the most outer layer of this CubeStructure is full, i.e. all of the cells are set. If CubeStructure has to grow and this method returns true, then Expand() method should be called to enable further growth.
        /// </summary>
        public bool LastLayerFull()
        {
            if (Radius == 0) return true;
            return GetLastLayer().Full;

        }

        /// <summary>
        /// Calls the method LastLayerFull() and returns its value.
        /// </summary>
        public bool ShouldExpand()
        {
            return LastLayerFull();
        }

        /// <summary>
        /// Retrieves the information, whether the most outer layer of this CubeStructure is empty, i.e. all of the cells are null. If this method returns true, then Shrink() method can be safely called to make CubeStructure smaller and preserve memory.
        /// </summary>
        public bool LastLayerEmpty()
        {
            if (Radius == 0) return false;
            //Debug.Log(String.Format("Cells: {0}, Free: {1}", GetLastLayer().CellSpacesCount, GetLastLayer().FreeSpacesCount));
            return GetLastLayer().Empty;
        }

        /// <summary>
        /// Calls the method LastLayerEmpty() and returns its value.
        /// </summary>
        public bool CanShrink()
        {
            return LastLayerEmpty();
        }

        /// <summary>
        /// Expands cube by 1 in every direction. Radius is incremented and new CubeCells can be set on coords 1 farther from center. Initial value of every new cell is null.
        /// </summary>
        /// <returns>New incremented value of Radius.</returns>
        public int Expand()
        {
            if (!ShouldExpand() && UnwantedSizeChangeBehaviour != SizeChangeBehaviour.Ignore)
            {
                string message = "Expanded CubeStructure when the most outer layer is not full.";
                switch (UnwantedSizeChangeBehaviour)
                {
                    case SizeChangeBehaviour.Warning:
                        Debug.LogWarning(message);
                        break;
                    case SizeChangeBehaviour.Error:
                        Debug.LogError(message);
                        break;
                    case SizeChangeBehaviour.Exception:
                        throw new InvalidOperationException(message);
                }
            }

            if (Radius > 0)
            {
                int newRadius = Radius + 1;
                cells.AddForward(CreateCellYZSurface(newRadius));
                cells.AddBackward(CreateCellYZSurface(newRadius));

                for (int x = -Radius + 1; x < Radius; x++)
                {
                    var xCells = cells[x];
                    xCells.AddForward(CreateCellZLine(newRadius));
                    xCells.AddBackward(CreateCellZLine(newRadius));

                    for (int y = -Radius + 1; y < Radius; y++)
                    {
                        var xyCells = xCells[y];
                        xyCells.AddForward(CreateCellNull(newRadius));
                        xyCells.AddBackward(CreateCellNull(newRadius));
                    }
                }

                Radius = newRadius;
            }
            else
            {
                Radius++;

                cells.AddForward(new TwoWayList<TwoWayList<CubeCell>>());
                cells[0].AddForward(new TwoWayList<CubeCell>());
                cells[0][0].AddForward(null);
            }
            cellLayers.Add(new CellLayer(cellLayers.Count));
            return Radius;
        }

        /// <summary>
        /// Shrinks the cube by 1 in every direction. Radius is decremented and any CubeCells on the borders are lost. Cell values can be now set only up to coords 1 closer to center than before.
        /// </summary>
        /// <returns>New decremented value of Radius.</returns>
        public int Shrink()
        {
            if (!CanShrink() && UnwantedSizeChangeBehaviour != SizeChangeBehaviour.Ignore)
            {
                string message = "Shrinked CubeStructure when the most outer layer is not empty.";
                switch (UnwantedSizeChangeBehaviour)
                {
                    case SizeChangeBehaviour.Warning:
                        Debug.LogWarning(message);
                        break;
                    case SizeChangeBehaviour.Error:
                        Debug.LogError(message);
                        break;
                    case SizeChangeBehaviour.Exception:
                        throw new InvalidOperationException(message);
                }
            }

            if (Radius > 1)
            {
                Radius--;

                for (int x = -Radius + 1; x < Radius; x++)
                {
                    var xCells = cells[x];
                    for (int y = -Radius + 1; y < Radius; y++)
                    {
                        var xyCells = xCells[y];
                        xyCells.RemoveForward();
                        xyCells.RemoveBackward();
                    }
                    xCells.RemoveForward();
                    xCells.RemoveBackward();
                }
                cells.RemoveForward();
                cells.RemoveBackward();

                cellLayers.RemoveAt(cellLayers.Count - 1);
                return Radius;
            }
            else if (Radius == 1)
            {
                Radius--;

                cells[0][0].RemoveForward();
                cells[0].RemoveForward();
                cells.RemoveForward();

                cellLayers.RemoveAt(0);
            }
            return 0;
        }

        public IVector3? RetrieveCell()
        {
            var cells = RetrieveCells(1);
            if (cells.Count == 0) return null;
            return cells[0];
        }

        public List<IVector3> RetrieveCells(int count)
        {
            if (CellRetriever == null) return new List<IVector3>();
            else return CellRetriever.ModifyCells(this, count);
        }

        public IVector3? AppendCell()
        {
            var cells = AppendCells(1);
            if (cells.Count == 0) return null;
            return cells[0];
        }

        public List<IVector3> AppendCells(int count)
        {
            if (CellAppender == null) return new List<IVector3>();
            else return CellAppender.ModifyCells(this, count);
        }

        private void UpdateSides(int x, int y, int z)
        {
            UpdateSides(new IVector3(x, y, z));
        }

        private void UpdateSides(IVector3 position)
        {
            if (IsOutOfRadius(position)) return;

            CubeCell cell = this[position];
            if (cell == null) return;

            int x = position.X;
            int y = position.Y;
            int z = position.Z;

            bool rightSide = TryGetCell(x + 1, y, z) == null;
            bool leftSide = TryGetCell(x - 1, y, z) == null;
            bool upSide = TryGetCell(x, y + 1, z) == null;
            bool downSide = TryGetCell(x, y - 1, z) == null;
            bool frontSide = TryGetCell(x, y, z + 1) == null;
            bool backSide = TryGetCell(x, y, z - 1) == null;

            cell.TurnSideOnOff(CubeCellPlaneSide.Right, rightSide);
            cell.TurnSideOnOff(CubeCellPlaneSide.Left, leftSide);
            cell.TurnSideOnOff(CubeCellPlaneSide.Up, upSide);
            cell.TurnSideOnOff(CubeCellPlaneSide.Down, downSide);
            cell.TurnSideOnOff(CubeCellPlaneSide.Front, frontSide);
            cell.TurnSideOnOff(CubeCellPlaneSide.Back, backSide);
        }

        private void UpdateNeighboursSides(IVector3 position)
        {
            if (IsOutOfRadius(position)) return;

            CubeCell cell = this[position];
            bool hasSide = cell == null;

            int x = position.X;
            int y = position.Y;
            int z = position.Z;

            var rightCell = TryGetCell(x + 1, y, z);
            var leftCell = TryGetCell(x - 1, y, z);
            var upCell = TryGetCell(x, y + 1, z);
            var downCell = TryGetCell(x, y - 1, z);
            var frontCell = TryGetCell(x, y, z + 1);
            var backCell = TryGetCell(x, y, z - 1);

            if (rightCell != null) rightCell.TurnSideOnOff(CubeCellPlaneSide.Left, hasSide);
            if (leftCell != null) leftCell.TurnSideOnOff(CubeCellPlaneSide.Right, hasSide);
            if (upCell != null) upCell.TurnSideOnOff(CubeCellPlaneSide.Down, hasSide);
            if (downCell != null) downCell.TurnSideOnOff(CubeCellPlaneSide.Up, hasSide);
            if (frontCell != null) frontCell.TurnSideOnOff(CubeCellPlaneSide.Back, hasSide);
            if (backCell != null) backCell.TurnSideOnOff(CubeCellPlaneSide.Front, hasSide);
        }

        private void UpdateLastLayersSides()
        {
            for (int x = -Radius + 1; x < Radius; x++)
                for (int y = -Radius + 1; y < Radius; y++)
                {
                    UpdateSides(x, y, -Radius + 1);
                    UpdateSides(x, y, Radius - 1);
                }

            for (int y = -Radius + 1; y < Radius; y++)
                for (int z = -Radius + 2; z < Radius - 1; z++)
                {
                    UpdateSides(-Radius + 1, y, z);
                    UpdateSides(Radius - 1, y, z);
                }

            for (int z = -Radius + 2; z < Radius - 1; z++)
                for (int x = -Radius + 2; x < Radius - 1; x++)
                {
                    UpdateSides(x, -Radius + 1, z);
                    UpdateSides(x, Radius - 1, z);
                }
        }

        private void ForEachCell(Action<IVector3> action)
        {
            for (int x = -Radius + 1; x < Radius; x++)
            {
                for (int y = -Radius + 1; y < Radius; y++)
                {
                    for (int z = -Radius + 1; z < Radius; z++)
                    {
                        action(new IVector3(x, y, z));
                    }
                }
            }
        }

        private bool IsOutOfRadius(IVector3 position)
        {
            return
                Math.Abs(position.X) >= Radius ||
                Math.Abs(position.Y) >= Radius ||
                Math.Abs(position.Z) >= Radius;
        }

        private static CubeCell CreateCellNull(int radius) { return null; }

        private static TwoWayList<CubeCell> CreateCellZLine(int radius)
        {
            return MakeCoordList<CubeCell>(radius, CreateCellNull);
        }

        private static TwoWayList<TwoWayList<CubeCell>> CreateCellYZSurface(int radius)
        {
            return MakeCoordList<TwoWayList<CubeCell>>(radius, CreateCellZLine);
        }

        private static TwoWayList<TwoWayList<TwoWayList<CubeCell>>> CreateCellXYZCube(int radius)
        {
            return MakeCoordList<TwoWayList<TwoWayList<CubeCell>>>(radius, CreateCellYZSurface);
        }

        private static TwoWayList<T> MakeCoordList<T>(int radius, Func<int, T> createItem)
            where T : class
        {
            var list = new TwoWayList<T>();
            FillCoordList(list, radius, createItem);
            return list;
        }

        private static void FillCoordList<T>(TwoWayList<T> coordList, int radius, Func<int, T> createItem)
            where T : class
        {
            if (radius > 0)
            {
                coordList.AddForward(createItem(radius));
                for (int coord = 1; coord < radius; coord++)
                {
                    coordList.AddForward(createItem(radius));
                    coordList.AddBackward(createItem(radius));
                }
            }
        }

        public enum SizeChangeBehaviour
        {
            Ignore,
            Warning,
            Error,
            Exception
        }
    }
}
