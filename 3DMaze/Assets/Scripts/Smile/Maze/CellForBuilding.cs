namespace Assets.Scripts.Smile.Maze
{
    public class CellForBuilding : Cell
    {
        public bool IsVisited = false;
        public bool IsCompleted = false;

        public CellForBuilding(Cell cell)
        {
            X = cell.X;
            Y = cell.Y;
            Z = cell.Z;
            Wall = cell.Wall;
        }
    }
}
