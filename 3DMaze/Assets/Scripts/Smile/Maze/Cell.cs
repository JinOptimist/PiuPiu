namespace Assets.Scripts.Smile.Maze
{
    public class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        
        public WallTypes Wall { get; set; }

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]: {Wall}";
        }
    }
}
