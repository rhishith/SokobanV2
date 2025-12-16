using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Sprite[] characterViews; // 0: Up, 1: Down, 2: Left, 3: Right
    
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    
    private AudioManager audioManager;
    
    private GridManager gridManager;
    private Vector2Int playerPos;
    
    private SpriteRenderer spriteRenderer;
    private InputActionMap playerMap;
    private InputAction moveAction;
    private InputAction undoAction; // If you want to add undo later
    private InputAction restartAction; // "

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (inputActionAsset != null)
        {
            playerMap = inputActionAsset.FindActionMap("Player");
            if (playerMap != null)
            {
                moveAction = playerMap.FindAction("Move");
            }
        }
    }

    public void Initialize(GridManager grid, AudioManager audioMgr, Vector2Int startPos)
    {
        gridManager = grid;
        audioManager = audioMgr;
        playerPos = startPos;
        transform.position = gridManager.GridToWorld(playerPos); // Set initial visual position
    }

    private void OnEnable()
    {
        if (playerMap != null) playerMap.Enable();
        if (moveAction != null) moveAction.performed += OnMove;
    }

    private void OnDisable()
    {
        if (playerMap != null) playerMap.Disable();
        if (moveAction != null) moveAction.performed -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (gridManager == null || gridManager.Grid == null) return;
        if (!GameManager.Instance.IsPlayerInputEnabled) return;

        Vector2 input = context.ReadValue<Vector2>();

        // Filter small values (deadzone/noise)
        if (input.sqrMagnitude > 0.5f)
        {
            Vector2Int dir = GetDirectionFromInput(input);
            if (dir != Vector2Int.zero)
            {
                TryMove(dir);
            }
        }
    }

    private Vector2Int GetDirectionFromInput(Vector2 input)
    {
        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            // Vertical movement
            if (input.y > 0)
            {
                SetSprite(0);
                return Vector2Int.up;
            }
            else
            {
                SetSprite(1);
                return Vector2Int.down;
            }
        }
        else
        {
            // Horizontal movement
            if (input.x < 0)
            {
                SetSprite(2);
                return Vector2Int.left;
            }
            else
            {
                SetSprite(3);
                return Vector2Int.right;
            }
        }
    }

    private void SetSprite(int index)
    {
        if (characterViews != null && index >= 0 && index < characterViews.Length)
        {
            spriteRenderer.sprite = characterViews[index];
        }
    }

    private void TryMove(Vector2Int dir)
    {
        Vector2Int targetPos = playerPos + dir;
        
        if (!gridManager.IsInsideGrid(targetPos)) return;

        Tile targetTile = gridManager.GetTile(targetPos);
        Tile currentTile = gridManager.GetTile(playerPos);

        if (targetTile.Type == TileType.Wall) return;

        // Check for box interaction
        if (targetTile.Occupier != null && targetTile.Occupier.CompareTag("Box"))
        {
            if (TryPushBox(targetPos, dir))
            {
                MovePlayer(targetPos, currentTile);
                CheckWin();
            }
        }
        else if (targetTile.Occupier == null)
        {
            MovePlayer(targetPos, currentTile);
            if (audioManager != null) audioManager.PlayPlayerMovementSfx();
            CheckWin();
        }
    }

    private bool TryPushBox(Vector2Int boxPos, Vector2Int dir)
    {
        Vector2Int boxTargetPos = boxPos + dir;

        if (!gridManager.IsInsideGrid(boxTargetPos)) return false;

        Tile boxTargetTile = gridManager.GetTile(boxTargetPos);

        // Cannot push into wall or another object
        if (boxTargetTile.Type == TileType.Wall || boxTargetTile.Occupier != null) 
            return false;

        // Perform Box Move
        Tile currentBoxTile = gridManager.GetTile(boxPos);
        GameObject boxObj = currentBoxTile.Occupier;

        // Update Box Logical Position
        boxTargetTile.Occupier = boxObj;
        gridManager.SetTile(boxTargetPos, boxTargetTile);

        currentBoxTile.Occupier = null; // Clear old tile (will be overwritten by player anyway, but good for safety)
        gridManager.SetTile(boxPos, currentBoxTile);

        // Update Box Visual Position
        boxObj.transform.position = gridManager.GridToWorld(boxTargetPos);

        // SFX
        if (audioManager != null)
        {
            if (boxTargetTile.Type == TileType.Goal)
            {
                audioManager.PlayGoalSfx();
            }
            else
            {
                audioManager.PlayBoxMoveSfx();
            }
        }

        return true;
    }

    private void MovePlayer(Vector2Int targetPos, Tile currentTile)
    {
        // Clear old tile
        currentTile.Occupier = null;
        gridManager.SetTile(playerPos, currentTile);

        // Update Logic
        playerPos = targetPos;
        Tile newPlayerTile = gridManager.GetTile(playerPos);
        newPlayerTile.Occupier = gameObject;
        gridManager.SetTile(playerPos, newPlayerTile);

        // Update Visual
        transform.position = gridManager.GridToWorld(playerPos);
    }

    private void CheckWin()
    {
        if (gridManager.AreAllGoalsCovered())
        {
            GameManager.Instance.LevelComplete();
        }
    }
}