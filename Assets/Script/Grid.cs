using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public int gridSizeX, gridSizeY;
    public Vector2 offset;
    public Vector2 startPos;

    public GameObject tilePrefab;
    public GameObject[] candies;
    public GameObject[,] tiles;


    void Start() {
        tiles = new GameObject[gridSizeX, gridSizeY];
        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        startPos = transform.position + (Vector3.left * (offset.x * gridSizeX / 2)) + (Vector3.down * (offset.y * gridSizeY / 3));
        CreateGrid();
    }

    void CreateGrid() {
        Vector2 offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        Vector2 startPos = transform.position + (Vector3.left * (offset.x * gridSizeX / 2)) + (Vector3.down * (offset.y * gridSizeY / 3));

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 pos = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));

                GameObject backgroundTile = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + x + "," + y + ")";

                int index = Random.Range(0, candies.Length);

                int MAX_ITERATION = 0;
                while (MatchesAt(x, y, candies[index]) && MAX_ITERATION < 100) {
                    index = Random.Range(0, candies.Length);
                    MAX_ITERATION++;
                }
                MAX_ITERATION = 0;

               GameObject candy = ObjectPooler.Instance.SpawnFromPool(index.ToString(), pos, Quaternion.identity);

                candy.name = "(" + x + "," + y + ")";
                tiles[x, y] = candy;

            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) {
        if (column > 1 && row > 1) {
            if (tiles[column - 1, row].tag == piece.tag && tiles[column - 2, row].tag == piece.tag) {
                return true;
            }
            if (tiles[column, row - 1].tag == piece.tag && tiles[column, row - 2].tag == piece.tag) {
                return true;
            }
        } else if (column <= 1 || row <= 1) {
            if (row > 1) {
                if (tiles[column, row - 1].tag == piece.tag && tiles[column, row - 2].tag == piece.tag) {
                    return true;
                }
            }
            if (column > 1) {
                if (tiles[column - 1, row].tag == piece.tag && tiles[column - 2, row].tag == piece.tag) {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row) {
        if (tiles[column, row].GetComponent<Tile>().isMatched) {
            GameManager.instance.GetScore(10);

            GameObject gm = tiles[column, row];
            gm.SetActive(false);
            tiles[column, row] = null;
        }
    }

    public void DestroyMatches() {
        for (int i = 0; i < gridSizeX; i++) {
            for (int j = 0; j < gridSizeY; j++) {
                if (tiles[i, j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow() {
        int nullCount = 0;
        for (int i = 0; i < gridSizeX; i++) {
            for (int j = 0; j < gridSizeY; j++) {
                if (tiles[i, j] == null) {
                    nullCount++;
                } else if (nullCount > 0) {
                    tiles[i, j].GetComponent<Tile>().row -= nullCount;
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private void RefillBoard() {
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                if (tiles[x, y] == null) {
                    Vector2 tempPosition = new Vector3(startPos.x + (x * offset.x), startPos.y + (y * offset.y));
                    int candyToUse = Random.Range(0, candies.Length);

                   GameObject tileToRefill = ObjectPooler.Instance.SpawnFromPool(candyToUse.ToString(), tempPosition, Quaternion.identity);

                    tiles[x, y] = tileToRefill;
                }
            }
        }
    }

    private bool MatchesOnBoard() {
        for (int i = 0; i < gridSizeX; i++) {
            for (int j = 0; j < gridSizeY; j++) {
                if (tiles[i, j] != null) {
                    if (tiles[i, j].GetComponent<Tile>().isMatched) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard() {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard()) {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }
}