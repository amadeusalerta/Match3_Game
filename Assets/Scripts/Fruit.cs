using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public enum FruitType { blue, green, red, yellow }

public class Fruit : MonoBehaviour
{
    [Header("Identity")]
    public Vector2Int posIndex;
    public FruitType fruitType;
    public int fruitPrefab;

    [Header("Features")]
    private bool moving;
    private bool mousePressed = false;
    public bool isMatched;
    private float swipeAngle = 0;
    private Vector2 firstTouchPosition, finalTouchPosition;
    private Vector2Int distance;

    [Header("Objects")]
    [HideInInspector] public GridManager grid;

    private void Awake()
    {
        grid = FindObjectOfType<GridManager>();
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, posIndex) > .01f)
            transform.position = Vector2.Lerp(transform.position, posIndex, grid.fruitSpeed * Time.deltaTime);

        else
        {
            if (moving)
            {
                transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
                moving = false;
                AllFruitsUpdate();
                grid.gameStatus = GameStatus.finished;
            }
        }

        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            moving = true;
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngel();
        }
    }

    public void SetupFruit(Vector2Int position, int prefabID)
    {
        posIndex = position;
        fruitPrefab = prefabID;
    }

    private void OnMouseDown()
    {
        if (grid.gameStatus == GameStatus.none)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
            grid.gameStatus = GameStatus.gaming;
        }
    }

    private void CalculateAngel()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
            MovePieces();
    }

    private void MovePieces()
    {
        Vector2 position2D = finalTouchPosition - firstTouchPosition;
        Vector2Int roundedPosition = new Vector2Int(Mathf.RoundToInt(Mathf.Abs(position2D.x)), Mathf.RoundToInt(Mathf.Abs(position2D.y)));
        distance = roundedPosition;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < grid.width - 1)
        {
            Debug.Log("Swipe Right Working...");
            grid.directionMov = DirectionMov.horizontal;
            grid.newFruits = new Transform[distance.x + grid.width];
            Transform fruit;

            for (int i = 0; i < grid.newFruits.Length; i++)
            {
                if (i < distance.x)
                {
                    Transform copiedFruit = grid.allFruits[grid.width - distance.x + i, posIndex.y];
                    fruit = EZ_PoolManager.Spawn(grid.fruits[copiedFruit.GetComponent<Fruit>().fruitPrefab], new Vector3(i - distance.x, posIndex.y, 0f), Quaternion.identity);

                    fruit.GetComponent<Fruit>().fruitPrefab = copiedFruit.GetComponent<Fruit>().fruitPrefab;
                }
                else
                {
                    fruit = grid.allFruits[i - distance.x, posIndex.y];
                }
                fruit.GetComponent<Fruit>().posIndex = new Vector2Int(i, posIndex.y);
                grid.newFruits[i] = fruit;
            }
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
        {
            Debug.Log("Swipe Left Working...");
            grid.directionMov = DirectionMov.horizontal;
            grid.newFruits = new Transform[distance.x + grid.width];
            Transform fruit;
            int x = 0, y = 0;

            for (int i = 0; i < grid.newFruits.Length; i++)
            {
                if (i < grid.width - distance.x)
                {
                    fruit = grid.allFruits[i + distance.x, posIndex.y];
                    fruit.GetComponent<Fruit>().posIndex = new Vector2Int(i, posIndex.y);
                }

                else if (i >= distance.x && i < grid.width)
                {
                    Transform copiedFruit = grid.allFruits[i - distance.x, posIndex.y];
                    fruit = EZ_PoolManager.Spawn(grid.fruits[copiedFruit.GetComponent<Fruit>().fruitPrefab], new Vector3(grid.width + y, posIndex.y, 0f), Quaternion.identity);
                    fruit.GetComponent<Fruit>().fruitPrefab = copiedFruit.GetComponent<Fruit>().fruitPrefab;
                    fruit.GetComponent<Fruit>().posIndex = new Vector2Int(i, posIndex.y);
                    y++;
                }
                else
                {
                    fruit = grid.allFruits[x, posIndex.y];
                    fruit.GetComponent<Fruit>().posIndex = new Vector2Int(x - distance.x, posIndex.y);
                    x++;
                }

                grid.newFruits[i] = fruit;
            }
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < grid.height - 1)    
        {
            Debug.Log("Swipe Up Working...");
            grid.directionMov = DirectionMov.vertical;
            grid.newFruits = new Transform[distance.y + grid.height];
            Transform fruit;

            for (int i = 0; i < grid.newFruits.Length; i++)
            {
                if (i < distance.y)
                {
                    Transform copiedFruit = grid.allFruits[posIndex.x, grid.height - distance.y + i];
                    fruit = EZ_PoolManager.Spawn(grid.fruits[copiedFruit.GetComponent<Fruit>().fruitPrefab], new Vector3(posIndex.x, i - distance.y, 0f), Quaternion.identity);

                    fruit.GetComponent<Fruit>().fruitPrefab = copiedFruit.GetComponent<Fruit>().fruitPrefab;
                }
                else
                {
                    fruit = grid.allFruits[posIndex.x, i - distance.y];
                }
                fruit.GetComponent<Fruit>().posIndex = new Vector2Int(posIndex.x, i);
                grid.newFruits[i] = fruit;
            }
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
        {
            Debug.Log("Swipe Down Working...");
            grid.directionMov = DirectionMov.vertical;
            grid.newFruits = new Transform[distance.y + grid.height];
            Transform fruit;
            int x = 0, y = 0;

            for (int i = 0; i < grid.newFruits.Length; i++)
            {
                if (i < grid.height - distance.y)
                {
                    fruit = grid.allFruits[posIndex.x, i + distance.y];
                    fruit.GetComponent<Fruit>().posIndex = new Vector2Int(posIndex.x, i);
                }

                else if (i >= distance.x && i < grid.height)
                {
                    Transform copiedFruit = grid.allFruits[posIndex.y, i - distance.y];
                    fruit = EZ_PoolManager.Spawn(grid.fruits[copiedFruit.GetComponent<Fruit>().fruitPrefab], new Vector3(posIndex.x, grid.height + y, 0f), Quaternion.identity);
                    fruit.GetComponent<Fruit>().fruitPrefab = copiedFruit.GetComponent<Fruit>().fruitPrefab;
                    fruit.GetComponent<Fruit>().posIndex = new Vector2Int(posIndex.x, i);
                    y++;
                }
                else
                {
                    fruit = grid.allFruits[posIndex.x, x];
                    fruit.GetComponent<Fruit>().posIndex = new Vector2Int(posIndex.x, x - distance.y);
                    x++;
                }

                grid.newFruits[i] = fruit;
            }
        }


    }

    private void AllFruitsUpdate()
    {
        if (grid.directionMov == DirectionMov.horizontal)
        {
            for (int i = 0; i < grid.newFruits.Length; i++)
            {
                if (i >= grid.width)
                {
                    EZ_PoolManager.Despawn(grid.newFruits[i]);
                }
                else
                {
                    grid.allFruits[i, posIndex.y] = grid.newFruits[i];
                }
            }
        }
        else if (grid.directionMov == DirectionMov.vertical)
        {
            for (int i = 0; i < grid.newFruits.Length; i++)
            {
                if (i >= grid.height)
                {
                    EZ_PoolManager.Despawn(grid.newFruits[i]);
                }
                else
                {
                    grid.allFruits[posIndex.x, i] = grid.newFruits[i];
                }
            }
        }
    }
}