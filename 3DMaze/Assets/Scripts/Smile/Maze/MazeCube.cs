using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Smile.Maze
{
    public class MazeCube
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<Cell> Cells { get; set; } = new();

        public Cell this[int x, int y, int z]
        {
            get
            {
                return GetCell(x, y, z);
            }
            set
            {
                var oldCell = GetCell(x, y, z);
                if (oldCell is not null)
                {
                    Cells.Remove(oldCell);
                }
                Cells.Add(value);
            }
        }

        public Cell ReplaceCell(int x, int y, int z, WallTypes newWall)
        {
            var cell = new Cell { X = x, Y = y, Z = z, Wall = newWall };
            this[x, y, z] = cell;
            return cell;
        }

        private Cell GetCell(int x, int y, int z)
        {
            return Cells.FirstOrDefault(c => c.X == x && c.Y == y && c.Z == z);
        }
    }
}
