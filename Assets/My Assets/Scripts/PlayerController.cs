using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public AudioManager audiomanager;
    public GridManager gridManager;
    public Vector2Int playerPos;
    public LevelLoader levelLoader;
    public Sprite[] characterViews;
    private SpriteRenderer spriteRenderer;
    private Stack<Tile[,]> undoStack = new Stack<Tile[,]>();
    private bool playerCanMove = true;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (gridManager == null || gridManager.grid == null) return;
        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            dir = Vector2Int.up;
            spriteRenderer.sprite = characterViews[0];
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dir = Vector2Int.down;
            spriteRenderer.sprite = characterViews[1];
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = Vector2Int.left;
            spriteRenderer.sprite = characterViews[2];
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = Vector2Int.right;
            spriteRenderer.sprite = characterViews[3];
        }

        if (dir != Vector2Int.zero)
            TryMove(dir);

        if (Input.GetKeyDown(KeyCode.U))
        {
            UndoMove();
            audiomanager.PlayUndoSfx();
        }
        if (Input.GetKeyDown(KeyCode.R)) RestartLevel();
    }

    void TryMove(Vector2Int dir)
    {
        //Debug.Log("Trying to move: " + dir);
        Vector2Int targetPos = playerPos + dir;
        if (!gridManager.IsInsideGrid(targetPos)) return;

        Tile targetTile = gridManager.GetTile(targetPos);
        Tile currentTile = gridManager.GetTile(playerPos);

        if (targetTile.Type == TileType.Wall) return;

        if (targetTile.Occupier != null && targetTile.Occupier.CompareTag("Box") && playerCanMove)
        {
            // Try to push box
            Vector2Int boxTarget = targetPos + dir;
            if (!gridManager.IsInsideGrid(boxTarget)) return;

            Tile boxTargetTile = gridManager.GetTile(boxTarget);
            if (boxTargetTile.Type != TileType.Wall && boxTargetTile.Occupier == null)
            {
                SaveUndoState();
                // Move box
                boxTargetTile.Occupier = targetTile.Occupier;
                targetTile.Occupier.transform.position = gridManager.GridToWorld(boxTarget);
                targetTile.Occupier = null;
                gridManager.SetTile(boxTarget, boxTargetTile);
                gridManager.SetTile(targetPos, targetTile);

                //sfx
                if (boxTargetTile.Type == TileType.Goal)
                {
                    audiomanager.PlayGoalSfx();
                }
                else
                {
                    audiomanager.PlayBoxMoveSfx();
                }
                // Move player
                currentTile.Occupier = null;
                gridManager.SetTile(playerPos, currentTile);
                playerPos = targetPos;
                transform.position = gridManager.GridToWorld(playerPos);
                Tile newPlayerTile = gridManager.GetTile(playerPos);
                newPlayerTile.Occupier = gameObject;
                gridManager.SetTile(playerPos, newPlayerTile);

                CheckWin();
            }
            return;
        }

        if (targetTile.Occupier == null && playerCanMove)
        {
            SaveUndoState();
            currentTile.Occupier = null;
            gridManager.SetTile(playerPos, currentTile);
            playerPos = targetPos;
            transform.position = gridManager.GridToWorld(playerPos);
            Tile newPlayerTile = gridManager.GetTile(playerPos);
            newPlayerTile.Occupier = gameObject;
            gridManager.SetTile(playerPos, newPlayerTile);
            audiomanager.PlayPlayerMovementSfx();

            CheckWin();
        }
    }



    void SaveUndoState()
    {
        Tile[,] snapshot = gridManager.grid.Clone() as Tile[,];
        undoStack.Push(snapshot);
    }

    void UndoMove()
    {
        if (undoStack.Count == 0) return;
        gridManager.grid = undoStack.Pop();
        ResyncSceneFromGrid();
    }

    void RestartLevel()
    {
        // Optionally reload level from LevelLoader
    }

    void CheckWin()
    {
        if (gridManager.AreAllGoalsCovered())
        {
            StartCoroutine(WaitAndMoveToNextLevel());
        }
    }

    private IEnumerator WaitAndMoveToNextLevel()
    {
        playerCanMove = false;
        yield return new WaitForSeconds(0.5f);  
        audiomanager.StopBackgroundMusic();
        //GameManager.instance.loadingImage.SetActive(true);
        levelLoader.NextLevel();
        yield return new WaitForSeconds(GameManager.instance.levelWaitTime);
        playerCanMove = true;
        //GameManager.instance.loadingImage.SetActive(false);
    }

    void ResyncSceneFromGrid()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Tile t = gridManager.GetTile(pos);
                if (t.Occupier == null) continue;
                if (t.Occupier.CompareTag("Player"))
                {
                    playerPos = pos;
                    transform.position = gridManager.GridToWorld(playerPos);
                }
                else
                {
                    t.Occupier.transform.position = gridManager.GridToWorld(pos);
                }
            }
        }
    }
}
