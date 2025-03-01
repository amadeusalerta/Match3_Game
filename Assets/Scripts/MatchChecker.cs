using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using System.Linq;

public class MatchChecker : MonoBehaviour
{
    public GridManager grid;
    public CanvasController canvas;
    private bool isMatched;

    public void FindAllMatches()
    {
        isMatched = false;

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Transform currentFruit = grid.allFruits[x, y];
                if (currentFruit == null) continue;

                Fruit currentFruitComponent = currentFruit.GetComponent<Fruit>();

                if (CheckMatch(x, y, currentFruit, currentFruitComponent, true) ||
                    CheckMatch(x, y, currentFruit, currentFruitComponent, false))
                {
                    isMatched = true;
                }
            }
        }

        if (isMatched)
        {
            StartCoroutine(DestroyFruits(0.5f));
        }
        else
        {
            grid.gameStatus = GameStatus.none;
        }
    }

    private bool CheckMatch(int x, int y, Transform currentFruit, Fruit currentFruitComponent, bool isHorizontal)
    {
        int dx = isHorizontal ? 1 : 0;
        int dy = isHorizontal ? 0 : 1;

        if (x - dx >= 0 && x + dx < grid.width && y - dy >= 0 && y + dy < grid.height)
        {
            Transform first = grid.allFruits[x - dx, y - dy];
            Transform second = grid.allFruits[x + dx, y + dy];

            if (first != null && second != null)
            {
                Fruit firstComponent = first.GetComponent<Fruit>();
                Fruit secondComponent = second.GetComponent<Fruit>();

                if (firstComponent.fruitType == currentFruitComponent.fruitType &&
                    secondComponent.fruitType == currentFruitComponent.fruitType)
                {
                    MarkAsMatched(first, firstComponent);
                    MarkAsMatched(second, secondComponent);
                    MarkAsMatched(currentFruit, currentFruitComponent);

                    canvas.UpdateTask(currentFruitComponent.fruitPrefab);
                    return true;
                }
            }
        }
        return false;
    }

    private void MarkAsMatched(Transform fruit, Fruit fruitComponent)
    {
        fruitComponent.isMatched = true;
        fruit.tag = "isMatched";
    }

    private IEnumerator DestroyFruits(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("isMatched"))
        {
            item.tag = "Untagged";
            grid.allFruits[item.GetComponent<Fruit>().posIndex.x, item.GetComponent<Fruit>().posIndex.y] = null;
            EZ_PoolManager.Despawn(item.transform);
        }

        StartCoroutine(DecreaseRowCo(0.5f));
    }

    private IEnumerator DecreaseRowCo(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMatched = false;

        for (int x = 0; x < grid.width; x++)
        {
            int nullCounter = 0;

            for (int y = 0; y < grid.height; y++)
            {
                if (grid.allFruits[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    grid.allFruits[x, y - nullCounter] = grid.allFruits[x, y];
                    grid.allFruits[x, y].GetComponent<Fruit>().posIndex.y -= nullCounter;
                    grid.allFruits[x, y] = null;
                }
            }
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(0.5f);
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        FindAllMatches();
    }

    private void RefillBoard()
    {
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                if (grid.allFruits[x, y] == null)
                {
                    int fruitIndex = Random.Range(0, grid.fruits.Length);
                    grid.SpawnFruit(new Vector2Int(x, y), grid.fruits[fruitIndex], fruitIndex);
                }
            }
        }

        RemoveMisplacedFruits();
    }

    private void RemoveMisplacedFruits()
    {
        HashSet<Transform> foundFruits = new HashSet<Transform>(FindObjectsOfType<Fruit>().Select(f => f.transform));

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                foundFruits.Remove(grid.allFruits[x, y]);
            }
        }

        foreach (Transform fruit in foundFruits)
        {
            Destroy(fruit.gameObject);
        }
    }
}