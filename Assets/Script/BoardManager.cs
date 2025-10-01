using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public int rows = 5;
    public int cols = 5;

    public RectTransform boardRoot;
    public RectTransform tilesParent;
    public Tile tilePrefab;

    private float cellSize;
    private bool[,] occ;
    [SerializeField] private Tile activeTile;
    [Header("Animation Settings")]
    public float moveDuration = 0.01f; // Thời gian di chuyển mỗi bước (giây)
    private bool isMoving = false; // Trạng thái đang di chuyển
    
    [Header("UI References")]
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    
    [Header("Swipe Settings")]
    public float swipeThreshold = 50f; // Ngưỡng tối thiểu để tính là swipe
    private Vector2 mouseDownPos;
    private bool isMouseDown = false;
    
    [Header("Merge Settings")]
    public float mergeAnimationDuration = 0.3f; // Thời gian animation merge
    public Ease mergeEase = Ease.OutBounce; // Loại easing cho merge
    public bool requireEmptySpaceForMerge = true; // Yêu cầu có khoảng trống để merge

    private void Start()
    {
        InitBoard();
        SpawnLevelExample();
    }
    private void InitBoard()
    {
        float boardWidth = boardRoot.rect.width;
        float boardHeight = boardRoot.rect.height;
        cellSize = Mathf.Min(boardWidth / cols, boardHeight / rows);
        occ = new bool[rows, cols];
    }
    void SpawnLevelExample()
    {
        // Ví dụ tạo 1 tile 2x2 màu xanh, active
        activeTile = Instantiate(tilePrefab, tilesParent);
        activeTile.Setup(0, 0, 2, 2, cellSize, "8");
        MarkOccupancy(activeTile, true);

        // Ví dụ tạo 1 tile 1x2 màu hồng, chướng ngại
        var blocker = Instantiate(tilePrefab, tilesParent);
        blocker.Setup(0, 2, 1, 2, cellSize, "4");
        MarkOccupancy(blocker, true);
        
        // Tạo thêm tile để test merge
        var tile2 = Instantiate(tilePrefab, tilesParent);
        tile2.Setup(2, 2, 1, 1, cellSize, "2");
        MarkOccupancy(tile2, true);
        
        var tile3 = Instantiate(tilePrefab, tilesParent);
        tile3.Setup(2, 1, 1, 1, cellSize, "2");
        MarkOccupancy(tile3, true);
    }
    public void MarkOccupancy(Tile t, bool value)
    {
        for (int r = t.row; r < t.row + t.height; r++)
            for (int c = t.col; c < t.col + t.width; c++)
                occ[r, c] = value;
    }
    private void Update()
    {
        if (isMoving) return;
        
        // Xử lý mouse down
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }
        
        // Xử lý mouse up
        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseUp();
        }
    }
    
    void HandleMouseDown()
    {
        mouseDownPos = Input.mousePosition;
        isMouseDown = true;
        
        // Kiểm tra xem có click vào tile không để chọn
        HandleMouseClick();
    }
    
    void HandleMouseUp()
    {
        if (!isMouseDown) return;
        
        Vector2 mouseUpPos = Input.mousePosition;
        Vector2 swipeVector = mouseUpPos - mouseDownPos;
        
        // Kiểm tra xem có phải swipe hay click ngắn
        if (swipeVector.magnitude >= swipeThreshold)
        {
            // Đây là swipe - di chuyển tile nếu có activeTile
            if (activeTile != null)
            {
                Vector2Int direction = GetSwipeDirection(swipeVector);
                if (direction != Vector2Int.zero)
                {
                    MoveActive(direction);
                }
            }
        }
        isMouseDown = false;
    }
    
    Vector2Int GetSwipeDirection(Vector2 swipeVector)
    {
        // Xác định hướng swipe dựa trên vector
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            // Swipe ngang
            Vector2Int direction = swipeVector.x > 0 ? Vector2Int.right : Vector2Int.left;
            return direction;
        }
        else
        {
            // Swipe dọc - Unity UI: Y tăng xuống dưới, nên phải đảo ngược
            Vector2Int direction = swipeVector.y > 0 ? Vector2Int.down : Vector2Int.up;
            return direction;
        }
    }
    
    void HandleMouseClick()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        bool tileClicked = false;
        foreach (RaycastResult result in results)
        {
            // Kiểm tra xem có click vào Tile không
            Tile clickedTile = result.gameObject.GetComponent<Tile>();
            if (clickedTile != null)
            {
                SelectTile(clickedTile);
                tileClicked = true;
                break;
            }
        }
        
        // Nếu click vào vùng trống, bỏ chọn tile hiện tại
        if (!tileClicked && activeTile != null)
        {
            DeselectCurrentTile();
        }
    }
    
    void DeselectCurrentTile()
    {
        if (activeTile != null)
        {
            SetTileVisual(activeTile, false);
            activeTile = null;
        }
    }
    
    void SelectTile(Tile tile)
    {
        // Bỏ chọn tile cũ nếu có
        if (activeTile != null)
        {
            SetTileVisual(activeTile, false);
        }
        
        // Chọn tile mới
        activeTile = tile;
        SetTileVisual(activeTile, true);
    }
    
    void SetTileVisual(Tile tile, bool isActive)
    {
        if (tile == null) return;
        tile.SetSelected(isActive);
    }
    void MoveActive(Vector2Int dir)
    {
        MoveActiveWithDOTween(dir);
    }
    
    void MoveActiveWithDOTween(Vector2Int dir)
    {
        if (isMoving) return; // Tránh spam input
        
        isMoving = true;
        
        // clear chỗ cũ
        MarkOccupancy(activeTile, false);

        int r = activeTile.row;
        int c = activeTile.col;
        int steps = 0;

        // Tính toán số bước có thể di chuyển
        while (true)
        {
            int rNext = r + dir.y;
            int cNext = c + dir.x;

            if (!InsideBoard(rNext, cNext, activeTile.width, activeTile.height)) break;
            if (!Free(rNext, cNext, activeTile.width, activeTile.height)) break;

            r = rNext;
            c = cNext;
            steps++;
        }

        if (steps == 0)
        {
            MarkOccupancy(activeTile, true);
            isMoving = false;
            return;
        }

        // Tạo sequence DOTween để di chuyển từng bước
        Sequence moveSequence = DOTween.Sequence();
        
        int startRow = activeTile.row;
        int startCol = activeTile.col;
        
        for (int i = 1; i <= steps; i++)
        {
            int newRow = startRow + (dir.y * i);
            int newCol = startCol + (dir.x * i);
            
            float newX = newCol * cellSize;
            float newY = -(newRow * cellSize);
            Vector2 newPosition = new Vector2(newX, newY);
            
            moveSequence.AppendCallback(() => {
                activeTile.row = newRow;
                activeTile.col = newCol;
            });
            
            moveSequence.Append(activeTile.rt.DOAnchorPos(newPosition, moveDuration).SetEase(Ease.OutQuad));
            
            if (i < steps)
            {
                moveSequence.AppendInterval(0);
            }
        }
        
        moveSequence.OnComplete(() => {
            MarkOccupancy(activeTile, true);
            
            CheckAndMergeTiles();
            
            isMoving = false;
        });
        
        moveSequence.Play();
    }
    
    public void StopCurrentAnimation()
    {
        if (isMoving)
        {
            DOTween.Kill(activeTile.rt);
            isMoving = false;
            MarkOccupancy(activeTile, true);
        }
    }
    bool InsideBoard(int row, int col, int w, int h)
    {
        return row >= 0 && col >= 0 && row + h <= rows && col + w <= cols;
    }

    bool Free(int row, int col, int w, int h)
    {
        for (int r = row; r < row + h; r++)
            for (int c = col; c < col + w; c++)
                if (occ[r, c]) return false;
        return true;
    }
    
    void CheckAndMergeTiles()
    {
        List<Tile> allTiles = GetAllTiles();
        
        for (int i = 0; i < allTiles.Count; i++)
        {
            for (int j = i + 1; j < allTiles.Count; j++)
            {
                Tile tile1 = allTiles[i];
                Tile tile2 = allTiles[j];
                
                if (CanMerge(tile1, tile2))
                {
                    // Kiểm tra điều kiện merge dựa trên setting
                    bool canMerge = requireEmptySpaceForMerge ? CanMergeSafely(tile1, tile2) : true;
                    
                    if (canMerge)
                    {
                        StartCoroutine(MergeTwoTiles(tile1, tile2));
                        return;
                    }
                }
            }
        }
    }
    bool CanMergeSafely(Tile tile1, Tile tile2)
    {
        // Kiểm tra xem có thể merge an toàn không (vị trí mới luôn ở tile2)
        Tile mainTile = tile2; // Vị trí mới sẽ ở tile2
        
        // Clear occupancy tạm thời để kiểm tra
        MarkOccupancy(tile1, false);
        MarkOccupancy(tile2, false);
        
        // Kiểm tra có thể đặt tile ở vị trí tile2 không
        bool canPlaceAtTile2 = CanPlaceTileAt(mainTile.row, mainTile.col, mainTile.width, mainTile.height);
        
        // Kiểm tra có vị trí trống khác để đặt tile không (nếu tile2 không được)
        bool hasEmptyPosition = HasEmptyPositionForTile(mainTile.width, mainTile.height);
        
        // Kiểm tra có đủ khoảng trống cho tile sau khi scale không
        bool hasSpaceForScale = HasEnoughSpaceForScaledTile(mainTile.width, mainTile.height);
        
        // Restore occupancy
        MarkOccupancy(tile1, true);
        MarkOccupancy(tile2, true);
        
        // Merge được phép nếu có thể đặt ở vị trí tile2 hoặc có vị trí trống khác
        bool canMerge = (canPlaceAtTile2 || hasEmptyPosition) && hasSpaceForScale;
        
        return canMerge;
    }
    
    bool HasEmptyPositionForTile(int width, int height)
    {
        for (int r = 0; r <= rows - height; r++)
        {
            for (int c = 0; c <= cols - width; c++)
            {
                if (CanPlaceTileAt(r, c, width, height))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    bool HasEnoughSpaceForScaledTile(int width, int height)
    {
        bool canScaleWidth = HasEmptyPositionForTile(width * 2, height);
        bool canScaleHeight = HasEmptyPositionForTile(width, height * 2);
        bool canScaleBoth = HasEmptyPositionForTile(width * 2, height * 2);
        return canScaleWidth || canScaleHeight || canScaleBoth;
    }
    
    public List<Tile> GetAllTiles()
    {
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < tilesParent.childCount; i++)
        {
            Tile tile = tilesParent.GetChild(i).GetComponent<Tile>();
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }
        return tiles;
    }
    
    bool CanMerge(Tile tile1, Tile tile2)
    {
        if (tile1.tileText.text != tile2.tileText.text) return false;
        return AreTilesTouching(tile1, tile2);
    }
    
    bool AreTilesTouching(Tile tile1, Tile tile2)
    {
        int left1 = tile1.col;
        int right1 = tile1.col + tile1.width - 1;
        int top1 = tile1.row;
        int bottom1 = tile1.row + tile1.height - 1;
        
        int left2 = tile2.col;
        int right2 = tile2.col + tile2.width - 1;
        int top2 = tile2.row;
        int bottom2 = tile2.row + tile2.height - 1;
        
        bool overlapX = !(right1 < left2 || right2 < left1);
        bool overlapY = !(bottom1 < top2 || bottom2 < top1);
        
        if (overlapX && overlapY) return true;
        
        bool adjacentX = (right1 + 1 == left2 || right2 + 1 == left1) && overlapY;
        bool adjacentY = (bottom1 + 1 == top2 || bottom2 + 1 == top1) && overlapX;
        
        return adjacentX || adjacentY;
    }
    IEnumerator MergeTwoTiles(Tile tile1, Tile tile2)
    {
        // Chọn tile2 làm tile chính (vị trí mới)
        Tile mainTile = tile2;
        Tile tileToRemove = tile1;
        
        // Tăng giá trị gấp đôi
        int currentValue = int.Parse(mainTile.tileText.text);
        int newValue = currentValue * 2;
        mainTile.tileText.text = newValue.ToString();
        
        // Clear occupancy của cả 2 tile trước khi merge
        MarkOccupancy(mainTile, false);
        MarkOccupancy(tileToRemove, false);
        
        // Animation merge
        Sequence mergeSequence = DOTween.Sequence();
        
        // Di chuyển tile chính đến vị trí mới (vị trí của tile2)
        mainTile.col = tile2.col;
        mainTile.row = tile2.row;
        float newX = mainTile.col * cellSize;
        float newY = -(mainTile.row * cellSize);
        Vector2 newPos = new Vector2(newX, newY);
        
        // Di chuyển tile1 đến vị trí tile2 trước khi merge
        if (tile1.row != tile2.row || tile1.col != tile2.col)
        {
            float oldX = tile1.col * cellSize;
            float oldY = -(tile1.row * cellSize);
            Vector2 oldPos = new Vector2(oldX, oldY);
            
            // Di chuyển tile1 đến vị trí tile2
            mergeSequence.Append(tile1.rt.DOAnchorPos(newPos, mergeAnimationDuration * 0.3f).SetEase(Ease.OutQuad));
        }
        
        // Scale up effect cho tile chính (tile2)
        mergeSequence.Append(mainTile.rt.DOScale(1.2f, mergeAnimationDuration * 0.5f).SetEase(mergeEase));
        mergeSequence.Append(mainTile.rt.DOScale(1f, mergeAnimationDuration * 0.5f).SetEase(mergeEase));
        
        // Animation shrink và fade out cho tile bị xóa (tile1)
        mergeSequence.Join(tileToRemove.rt.DOScale(0f, mergeAnimationDuration).SetEase(Ease.InBack));
        mergeSequence.Join(tileToRemove.backgroundImage.DOFade(0f, mergeAnimationDuration));
        
        mergeSequence.Play();
        yield return mergeSequence.WaitForCompletion();
        
        // Mark occupancy cho tile mới ở vị trí tile2
        MarkOccupancy(mainTile, true);
        
        // Xóa tile đã merge
        Destroy(tileToRemove.gameObject);

        // Cập nhật activeTile nếu cần
        if (activeTile == tileToRemove)
        {
            activeTile = mainTile;
            SetTileVisual(activeTile, true);
        }
        
        // Kiểm tra merge tiếp theo sau khi hoàn thành
        yield return new WaitForSeconds(0.1f);
        CheckAndMergeTiles();
    }
    bool CanPlaceTileAt(int row, int col, int width, int height)
    {
        if (!InsideBoard(row, col, width, height)) return false;
            
        for (int r = row; r < row + height; r++)
        {
            for (int c = col; c < col + width; c++)
            {
                if (occ[r, c])
                    return false;
            }
        }
        
        return true;
    }
    
}
