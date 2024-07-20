using MazeGenerator;
using MazeGenerator.Models.MazeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using WallType = MazeGenerator.Models.MazeModels.Wall;

public class MazeBuilder : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Stair;

    public GameObject Player;

    public int Length;
    public int Width;
    public int Height;

    private const int WALL_SIZE = 4;
    private const int HALF_WALL_SIZE = WALL_SIZE / 2;

    // will set on Start
    private int zMargin = -1 * WALL_SIZE;

    // Start is called before the first frame update
    void Start()
    {
        var generator = new Generator();
        var maze = generator.Generate(Length, Width, Height,
            startPoint: new System.Numerics.Vector3(0, 0, Height - 1),
            seed: 42
            );
        zMargin = -1 * (maze.Width + 1) * WALL_SIZE;
        BuildMaze(maze);
        MovePlaeyerToStartPoint();
    }

    private void MovePlaeyerToStartPoint()
    {
        Player.transform.position = new Vector3(
            HALF_WALL_SIZE,
            Width * WALL_SIZE + WALL_SIZE,
            zMargin);

    }

    private void BuildMaze(Maze maze)
    {
        for (int z = 0; z < maze.Height; z++)
        {
            for (int y = 0; y < maze.Width; y++)
            {
                for (int x = 0; x < maze.Length; x++)
                {
                    BuildRoom(maze[x, y, z]);
                }
            }
        }

        // Build only first floor
        //for (int y = 0; y < maze.Width; y++)
        //{
        //    for (int x = 0; x < maze.Length; x++)
        //    {
        //        BuildRoom(maze[x, y, 0]);
        //    }
        //}
    }

    private void BuildRoom(Cell cell)
    {
        if (cell.InnerPart != InnerPart.None)
        {
            BuildStair(cell.X, cell.Y, cell.Z, cell.InnerPart);
        }

        var wall = cell.Wall;
        if (wall.HasFlag(WallType.North))
        {
            BuildWallNorthSouth(cell.X, cell.Y + 1, cell.Z);
        }
        if (wall.HasFlag(WallType.East))
        {
            BuildWallEastWest(cell.X + 1, cell.Y, cell.Z);
        }
        if (wall.HasFlag(WallType.South))
        {
            BuildWallNorthSouth(cell.X, cell.Y, cell.Z);
        }
        if (wall.HasFlag(WallType.West))
        {
            BuildWallEastWest(cell.X, cell.Y, cell.Z);
        }

        // TODO Create an Enter to the maze
        // For now just a HACK IT ^_^
        // Do not build one of the roof.
        // For now it will enter to the maze
        if (cell.X == 0 && cell.Y == 0 && cell.Z == Height - 1)
        {
            return;
        }
        if (wall.HasFlag(WallType.Roof))
        {
            BuildRoof(cell.X, cell.Y, cell.Z);
        }
    }

    private void BuildWallEastWest(int x, int y, int z)
    {
        var wall = Instantiate(Wall);
        wall.transform.position = new Vector3(
            x * WALL_SIZE,
            z * WALL_SIZE + WALL_SIZE / 2,
            y * WALL_SIZE + zMargin);
    }

    private void BuildWallNorthSouth(int x, int y, int z)
    {
        var wall = Instantiate(Wall);
        wall.transform.Rotate(0, 90, 0);

        wall.transform.position = new Vector3(
            x * WALL_SIZE + HALF_WALL_SIZE,
            z * WALL_SIZE + WALL_SIZE / 2,
            (y - 1) * WALL_SIZE + HALF_WALL_SIZE + zMargin);
    }

    private void BuildRoof(int x, int y, int z)
    {
        var wall = Instantiate(Wall);
        wall.transform.Rotate(0, 0, 90);

        wall.transform.position = new Vector3(
            x * WALL_SIZE + HALF_WALL_SIZE,
            z * WALL_SIZE + WALL_SIZE,
            y * WALL_SIZE + zMargin);
    }

    private void BuildStair(int x, int y, int z, InnerPart stairType)
    {
        var stair = Instantiate(Stair);

        switch (stairType)
        {
            case InnerPart.StairFromSouthToNorth:
                stair.transform.Rotate(0, 0, 0);
                break;
            case InnerPart.StairFromNorthToSouth:
                stair.transform.Rotate(0, 180, 0);
                break;
            case InnerPart.StairFromWestToEast:
                stair.transform.Rotate(0, 90, 0);
                break;
            case InnerPart.StairFromEastToWest:
                stair.transform.Rotate(0, -90, 0);
                break;
        }

        stair.transform.position = new Vector3(
            x * WALL_SIZE + (WALL_SIZE / 2),
            z * WALL_SIZE + WALL_SIZE / 2,
            y * WALL_SIZE + zMargin);
    }
}
