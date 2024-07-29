using Assets.Scripts.Smile;
using MazeGenerator;
using MazeGenerator.Models.GenerationModels;
using MazeGenerator.Models.MazeModels;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using WallType = MazeGenerator.Models.MazeModels.Wall;

public class MazeBuilder : MonoBehaviour
{
    public GameObject MazeParent;

    public GameObject WallTemplate;
    public GameObject StairTemplate;
    public GameObject ExitTemplate;
    public GameObject ExitFromChunkTemplate;
    public GameObject RoomTrigger;

    public GameObject Player;

    public int Length;
    public int Width;
    public int Height;

    private string materialBasePath = "Materials/WallByLevels";

    private const int WALL_SIZE = 4;
    private const int HALF_WALL_SIZE = WALL_SIZE / 2;

    // will be seted on Start
    private int zMargin;
    private int fullMazeLevelCount;

    private void Awake()
    {
        MovePlaeyerToStartPoint();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Length"))
        {
            Length = PlayerPrefs.GetInt("Length");
        }
        if (PlayerPrefs.HasKey("Width"))
        {
            Width = PlayerPrefs.GetInt("Width");
        }
        if (PlayerPrefs.HasKey("Height"))
        {
            Height = PlayerPrefs.GetInt("Height");
        }
        int? seed = PlayerPrefs.HasKey("Seed")
            ? PlayerPrefs.GetInt("Seed")
            : null;
        var generationWeightsType = PlayerPrefs.HasKey("GenerationWeightsType")
            ? (GenerationWeightsType)PlayerPrefs.GetInt("GenerationWeightsType")
            : GenerationWeightsType.GenericBuilding;
        var generationWeights = GetGenerationWeights(generationWeightsType);

        var generator = new Generator();
        var maze = generator.Generate(Length, Width, Height,
            startPoint: new System.Numerics.Vector2(0, 0),
            weights: generationWeights,
            seed: seed);

        PlayerPrefs.SetInt("Seed", maze.Seed);
        Debug.Log($"Seed: {maze.Seed}");

        zMargin = -1 * (maze.MaxWidth + 1) * WALL_SIZE;
        fullMazeLevelCount = maze.Chunks.Sum(x => x.Height);

        BuildMaze(maze);

        PlayerPathStore.Path = new System.Collections.Generic.List<GameObject>();
        MovePlaeyerToStartPoint();
    }

    private GenerationWeights GetGenerationWeights(GenerationWeightsType generationWeightsType)
    {
        switch (generationWeightsType)
        {
            case GenerationWeightsType.GenericBuilding:
                return GenerationWeights.GenericBuilding();
            case GenerationWeightsType.FullRandom:
                return GenerationWeights.FullRandom();
            case GenerationWeightsType.StairsEveryWhere:
                return GenerationWeights.StairsEveryWhere();
            default:
                throw new NotImplementedException();
        }
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
        var drawedLevels = 0;
        for (int chunkIndex = maze.Chunks.Count - 1; chunkIndex >= 0; chunkIndex--)
        {
            var chunk = maze.Chunks[chunkIndex];
            BuildChunk(chunk, drawedLevels, chunkIndex);
            drawedLevels += chunk.Height;
        }
    }

    private void BuildChunk(Chunk chunk, int drawedLevels, int chunkIndex)
    {
        for (int z = 0; z < chunk.Height; z++)
        {
            var fullLevel = drawedLevels + z;
            var level = new GameObject($"Level {z} ({fullLevel})");
            level.transform.position = new Vector3(0, DefaultYPosition(fullLevel), 0);
            for (int y = 0; y < chunk.Width; y++)
            {
                for (int x = 0; x < chunk.Length; x++)
                {
                    var room = BuildRoom(chunk[x, y, z], drawedLevels, chunkIndex);
                    room.transform.SetParent(level.transform, false);
                }
            }
            level.transform.SetParent(MazeParent.transform, false);
        }
    }

    private GameObject BuildRoom(Cell cell, int drawedLevels, int chunkIndex)
    {
        var zMargin = drawedLevels + cell.Z;
        var room = new GameObject($"Room[{cell.X}, {cell.Y}, {cell.Z} ({zMargin})]");
        room.transform.position = new Vector3(
            DefaultXPosition(cell.X),
            0, //DefaultYPosition(cell.Z),//DefaultYPosition(zMargin),
            DefaultZPosition(cell.Y));

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
                case InnerPart.ExitFromChunk:
                    var exitFromChunk = BuildExitFromChunk(cell.X, cell.Y, cell.Z);
                    exitFromChunk.transform.SetParent(room.transform, false);
                    break;
                case InnerPart.Start:
                    // Do nothing, just want to remember where I started
                    break;
                default:
                    Debug.LogWarning($"Uknown InnetPart {cell.InnerPart}");
                    break;
            }
        }

        var wallType = cell.Wall;
        if (wallType.HasFlag(WallType.North))
        {
            var wall = BuildWallNorth(cell.X, cell.Y, cell.Z, chunkIndex);
            wall.transform.SetParent(room.transform, false);
        }
        if (wallType.HasFlag(WallType.East))
        {
            var wall = BuildWallEast(cell.X, cell.Y, cell.Z, chunkIndex);
            wall.transform.SetParent(room.transform, false);
        }
        if (wallType.HasFlag(WallType.South))
        {
            var wall = BuildWallSouth(cell.X, cell.Y, cell.Z, chunkIndex);
            wall.transform.SetParent(room.transform, false);
        }
        if (wallType.HasFlag(WallType.West))
        {
            var wall = BuildWallWest(cell.X, cell.Y, cell.Z, chunkIndex);
            wall.transform.SetParent(room.transform, false);
        }

        if (wallType.HasFlag(WallType.Roof))
        {
            var roof = BuildRoof(cell.X, cell.Y, cell.Z, chunkIndex);
            roof.transform.SetParent(room.transform, false);
        }

        //WriteTextToRoom(room, cell);
        var trigger = BuildRoomTrigger(cell.X, cell.Y, cell.Z);
        trigger.transform.SetParent(room.transform, false);

        return room;
    }

    private GameObject BuildRoomTrigger(int x, int y, int z)
    {
        var roomTrigger = Instantiate(RoomTrigger);

        var script = roomTrigger.GetComponent<RoomTrigger>();
        script.roomCoordinate = $"[{x}, {y}, {z}]";

        roomTrigger.transform.position = new Vector3(
            0,
            0,
            0);

        return roomTrigger;
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

    private GameObject BuildWallWest(int x, int y, int z, int chunkIndex)
    {
        var wall = CreateBaseWall(x, y, z, chunkIndex);
        wall.transform.Rotate(0, 180, 0);
        wall.transform.position = new Vector3(
            0 - HALF_WALL_SIZE, //x * WALL_SIZE,
            0,
            0);

        return wall;
    }

    private GameObject BuildWallEast(int x, int y, int z, int chunkIndex)
    {
        var wall = CreateBaseWall(x, y, z, chunkIndex);
        wall.transform.Rotate(0, 180, 0);
        wall.transform.position = new Vector3(
            WALL_SIZE - HALF_WALL_SIZE, //x * WALL_SIZE,
            0,
            0);

        return wall;
    }

    private GameObject BuildWallSouth(int x, int y, int z, int chunkIndex)
    {
        var wall = CreateBaseWall(x, y, z, chunkIndex);
        wall.transform.Rotate(0, 90, 0);

        wall.transform.position = new Vector3(
            0,
            0,
            0 - HALF_WALL_SIZE);
        return wall;
    }

    private GameObject BuildWallNorth(int x, int y, int z, int chunkIndex)
    {
        var wall = CreateBaseWall(x, y, z, chunkIndex);
        wall.transform.Rotate(0, 90, 0);

        wall.transform.position = new Vector3(
            0,
            0,
            WALL_SIZE - HALF_WALL_SIZE);
        return wall;
    }

    private GameObject BuildRoof(int x, int y, int z, int chunkIndex)
    {
        var roof = CreateBaseWall(x, y, z, chunkIndex);

        roof.transform.Rotate(0, 0, 90);

        roof.transform.position = new Vector3(
            0,
            0 + HALF_WALL_SIZE,
            0);

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
            0,
            0,
            0);

        return stair;
    }

    private GameObject BuildExit(int x, int y, int z)
    {
        var exit = Instantiate(ExitTemplate);

        exit.transform.position = new Vector3(0, 0, 0);
        return exit;
    }

    private GameObject BuildExitFromChunk(int x, int y, int z)
    {
        var exitFromChunk = Instantiate(ExitFromChunkTemplate);

        exitFromChunk.transform.position = new Vector3(
            0 - HALF_WALL_SIZE,
            0 - HALF_WALL_SIZE,
            0);
        return exitFromChunk;
    }

    private GameObject CreateBaseWall(int x, int y, int z, int chunkIndex)
    {
        var wall = Instantiate(WallTemplate);

        //var baseTextObject = wall
        //    .transform.Find("Canvas")
        //    .transform.Find("Text");
        //var textMeshPro = baseTextObject.GetComponent<TextMeshProUGUI>();
        //textMeshPro.text = $"[{x}, {y}, {z}]";

        var material = GetMaterial(chunkIndex);
        wall.GetComponent<Renderer>().material = material;

        return wall;
    }

    private Material GetMaterial(int chunkIndex)
    {
        if (chunkIndex > 4)
        {
            chunkIndex = 4;
        }

        var materialPath = materialBasePath + $"/Chunk{chunkIndex}";
        Material loadedMaterial = Resources.Load<Material>(materialPath);
        return loadedMaterial;
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
