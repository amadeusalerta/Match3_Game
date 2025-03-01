using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public enum DirectionMov { none, vertical, horizontal }
public enum GameStatus { none, gaming, finished, over }

public class GridManager : MonoBehaviour
{
    [HideInInspector] public int width;
    [HideInInspector] public int height;

    [Header("Specs")]
    public Color colorFirst;
    public float fruitSpeed;
    public DirectionMov directionMov;
    public GameStatus gameStatus;

    [Header("Objects")]
    public GameObject bgTilePrefab;
    public Transform[] fruits;
    public Transform[,] allFruits;
    public Transform[] newFruits;
    private MatchChecker matchFind;

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchChecker>();
        allFruits = new Transform[width, height];
    }

    private void Update()
    {
        if (gameStatus == GameStatus.finished)
        {
            matchFind.FindAllMatches();
            gameStatus = GameStatus.none;
        }
    }
    List<Vector3> bgPositions = new();

    public void Initialize()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = EZ_PoolManager.Spawn(bgTilePrefab.transform, pos, Quaternion.identity).gameObject;
                SpriteRenderer imageComponent = bgTile.GetComponent<SpriteRenderer>();

                bgPositions.Add(pos);

                imageComponent.color = (x % 2 != y % 2) ? colorFirst : imageComponent.color;

                int fruitToUse = Random.Range(0, fruits.Length);
                ControlFruit(new Vector2Int(x, y), fruits[fruitToUse], fruitToUse);
            }
        }

        Vector3 cameraPos = GetCenter(bgPositions);
        cameraPos.z = Camera.main.transform.position.z;
        Camera.main.transform.position = cameraPos;
    }
    public static Vector3 GetCenter(List<Vector3> vectors)
    {
        if (vectors == null || vectors.Count == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;

        foreach (var vec in vectors)
        {
            sum += vec;
        }

        return sum / vectors.Count;
    }

    private void ControlFruit(Vector2Int pos, Transform fruitToSpawn, int prefabID)
    {
        FruitType currentFruitType = fruitToSpawn.GetComponent<Fruit>().fruitType;
        int sameFruitsX = 0, sameFruitsY = 0;

        for (int i = 0; i < 2 && pos.x - i - 1 >= 0; i++)
        {
            if (allFruits[pos.x - i - 1, pos.y].GetComponent<Fruit>().fruitType == currentFruitType)
                sameFruitsX++;
            else
                break;
        }

        for (int i = 0; i < 2 && pos.y - i - 1 >= 0; i++)
        {
            if (allFruits[pos.x, pos.y - i - 1].GetComponent<Fruit>().fruitType == currentFruitType)
                sameFruitsY++;
            else
                break;
        }

        if (sameFruitsX > 1 || sameFruitsY > 1)
        {
            int fruitToUse = Random.Range(0, fruits.Length);
            ControlFruit(pos, fruits[fruitToUse], fruitToUse);
        }
        else
        {
            SpawnFruit(pos, fruitToSpawn, prefabID);
        }
    }

    public void SpawnFruit(Vector2Int pos, Transform fruitToSpawn, int prefabID)
    {
        Transform fruit = EZ_PoolManager.Spawn(fruitToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        allFruits[pos.x, pos.y] = fruit;
        fruit.GetComponent<Fruit>().SetupFruit(pos, prefabID);
    }
}
