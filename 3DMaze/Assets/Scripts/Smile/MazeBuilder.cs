using Assets.GameData;
using MazeGenerator;
using MazeGenerator.Models.GenerationModels;
using MazeGenerator.Models.MazeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using WallType = MazeGenerator.Models.MazeModels.Wall;

public class MazeBuilder : MonoBehaviour
{
    public GameObject MazeParent;

    public GameObject WallTemplate;
    public GameObject StairTemplate;
    public GameObject ExitTemplate;

    public GameObject Player;

    public int Length;
    public int Width;
    public int Height;

    private const int WALL_SIZE = 4;
    private const int HALF_WALL_SIZE = WALL_SIZE / 2;

    // will be seted on Start
    private int zMargin;

    // Start is called before the first frame update
    void Start()
    {
        Length = MazeParameter.Length;
        Width = MazeParameter.Width;
        Height = MazeParameter.Height;

        var generator = new Generator();
        var maze = generator.Generate(Length, Width, Height,
            startPoint: new System.Numerics.Vector3(0, 0, Height - 1),
            weights: MazeParameter.GenerationWeights,
            endPoint: MazeParameter.ExitLocation,
            seed: MazeParameter.Seed
            );
        zMargin = -1 * (maze.Width + 1) * WALL_SIZE;
        BuildMaze(maze);
        MovePlaeyerToStartPoint();
    }

    private void MovePlaeyerToStartPoint()
    {
        Player.transform.position = new Vector3(
            HALF_WALL_SIZE,
            Height * WALL_SIZE + WALL_SIZE,
            zMargin);

    }

    private void BuildMaze(Maze maze)
    {
        for (int z = 0; z < maze.Height; z++)
        {
            var level = new GameObject($"Level {z}");
            for (int y = 0; y < maze.Width; y++)
            {
                for (int x = 0; x < maze.Length; x++)
                {
                    var room = BuildRoom(maze[x, y, z]);
                    room.transform.SetParent(level.transform, false);
                }
            }
            level.transform.SetParent(MazeParent.transform, false);
        }
    }

    private GameObject BuildRoom(Cell cell)
    {
        var room = new GameObject($"Room[{cell.X}, {cell.Y}, {cell.Z}]");
        if (cell.InnerPart != InnerPart.None)
        {
            switch (cell.InnerPart)
            {
                case InnerPart.StairFromSouthToNorth:
                case InnerPart.StairFromNorthToSouth:
                case InnerPart.StairFromWestToEast:
                case InnerPart.StairFromEastToWest:
                    var stair = BuildStair(cell.X, cell.Y, cell.Z, cell.InnerPart);
                    stair.transform.SetParent(room.transform, false);
                    break;
                case InnerPart.Exit:
                    var exit = BuildExit(cell.X, cell.Y, cell.Z);
                    exit.transform.SetParent(room.transform, false);
                    break;
                default:
                    Debug.LogWarning($"Uknown InnetPart {cell.InnerPart}");
                    break;
            }
            
        }

        var wallType = cell.Wall;
        if (wallType.HasFlag(WallType.North))
        {
            var wall = BuildWallNorthSouth(cell.X, cell.Y + 1, cell.Z);
            wall.transform.SetParent(room.transform, false);
        }
        if (wallType.HasFlag(WallType.East))
        {
            var wall = BuildWallEastWest(cell.X + 1, cell.Y, cell.Z);
            wall.transform.SetParent(room.transform, false);
        }
        if (wallType.HasFlag(WallType.South))
        {
            var wall = BuildWallNorthSouth(cell.X, cell.Y, cell.Z);
            wall.transform.SetParent(room.transform, false);
        }
        if (wallType.HasFlag(WallType.West))
        {
            var wall = BuildWallEastWest(cell.X, cell.Y, cell.Z);
            wall.transform.SetParent(room.transform, false);
        }

        // TODO Create an Enter to the maze
        // For now just a HACK IT ^_^
        // Do not build one of the roof.
        // For now it will enter to the maze
        if (cell.X == 0 && cell.Y == 0 && cell.Z == Height - 1)
        {
            // do nothing to haven't roof in one point
        }
        else
        {
            if (wallType.HasFlag(WallType.Roof))
            {
                var roof = BuildRoof(cell.X, cell.Y, cell.Z);
                roof.transform.SetParent(room.transform, false);
            }
        }

        //WriteTextToRoom(room, cell);

        return room;
    }

    private void WriteTextToRoom(GameObject room, Cell cell)
    {
        //foreach (var wall in room
        //    .GetComponentsInChildren<Transform>()
        //    .Where(x=>x.name == "Wall(Clone)"))
        //{
        //    var baseTextObject = wall
        //        .transform.Find("Canvas")
        //        .transform.Find("Text");
        //    var textMeshPro = baseTextObject.GetComponent<TextMeshProUGUI>();
        //    if (cell.X == 0 && cell.Y == 0 && cell.Z == 0)
        //    {
        //        textMeshPro.text = $"Exit!";
        //    }
        //    else
        //    {
        //        textMeshPro.text = $"[{cell.X}, {cell.Y}, {cell.Z}]";
        //    }
        //}
    }

    private GameObject BuildWallEastWest(int x, int y, int z)
    {
        var wall = CreateBaseWall(x, y, z);
        wall.transform.Rotate(0, 180, 0);
        wall.transform.position = new Vector3(
            DefaultXPosition(x) - HALF_WALL_SIZE, //x * WALL_SIZE,
            DefaultYPosition(z),
            DefaultZPosition(y));

        return wall;
    }

    private GameObject BuildWallNorthSouth(int x, int y, int z)
    {
        var wall = CreateBaseWall(x, y, z);
        wall.transform.Rotate(0, 90, 0);

        wall.transform.position = new Vector3(
            DefaultXPosition(x),
            DefaultYPosition(z),
            DefaultZPosition(y) - HALF_WALL_SIZE);
        return wall;
    }

    private GameObject BuildRoof(int x, int y, int z)
    {
        var roof = CreateBaseWall(x, y, z);

        roof.transform.Rotate(0, 0, 90);

        roof.transform.position = new Vector3(
            DefaultXPosition(x),
            DefaultYPosition(z) + HALF_WALL_SIZE,
            DefaultZPosition(y));

        return roof;
    }

    private GameObject BuildStair(int x, int y, int z, InnerPart stairType)
    {
        var stair = Instantiate(StairTemplate);

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
            DefaultXPosition(x),
            DefaultYPosition(z),
            DefaultZPosition(y));

        return stair;
    }

    private GameObject BuildExit(int x, int y, int z)
    {
        var exit = Instantiate(ExitTemplate);

        exit.transform.position = new Vector3(
            DefaultXPosition(x),
            DefaultYPosition(z),
            DefaultZPosition(y));
        return exit;
    }

    private GameObject CreateBaseWall(int x, int y, int z)
    {
        var wall = Instantiate(WallTemplate);
        var baseTextObject = wall
            .transform.Find("Canvas")
            .transform.Find("Text");
        var textMeshPro = baseTextObject.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"[{x}, {y}, {z}]";
        return wall;
    }

    private float DefaultXPosition(int x)
    {
        return x * WALL_SIZE + HALF_WALL_SIZE;
    }
    
    private float DefaultYPosition(int z)
    {
        return z * WALL_SIZE + HALF_WALL_SIZE;
    }
    
    private float DefaultZPosition(int y)
    {
        return y * WALL_SIZE + zMargin;
    }
}
