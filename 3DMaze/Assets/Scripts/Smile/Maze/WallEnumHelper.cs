namespace Assets.Scripts.Smile.Maze
{
    public static class WallEnumHelper
    {
        public static WallTypes Remove(this WallTypes origin, WallTypes wall)
        {
            return origin & ~wall;
        }
    }
}
