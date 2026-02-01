using UnityEngine;

public class MaskMovement : MonoBehaviour
{
    private float moveSpeed;
    private Vector2 direction;

    private RectTransform self;
    private RectTransform bounds;
    private Vector2 halfSize;

    private Rect worldBounds; // ✅ 添加世界坐标边界

    private void Awake()
    {
        self = GetComponent<RectTransform>();
        halfSize = self.rect.size * 0.5f;
    }

    public void Setup(RectTransform b, float speed)
    {
        if (b == null)
        {
            Debug.LogError($"Setup: bounds is null for {name}");
            return;
        }

        bounds = b;
        moveSpeed = speed;

        // ✅ 计算世界坐标边界
        CalculateWorldBounds();
    }

    private void CalculateWorldBounds()
    {
        if (bounds == null) return;

        // 获取bounds的四个角的世界坐标
        Vector3[] corners = new Vector3[4];
        bounds.GetWorldCorners(corners);

        // 计算世界坐标下的边界矩形
        float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);

        worldBounds = new Rect(minX, minY, maxX - minX, maxY - minY);

    }

    private void OnEnable()
    {
        // 每次激活给个新方向
        direction = Random.insideUnitCircle.normalized;

        // ✅ 确保边界已计算
        if (bounds != null && worldBounds.size == Vector2.zero)
        {
            CalculateWorldBounds();
        }
    }

    private void Update()
    {
        MoveAndBounce();
    }

    private void MoveAndBounce()
    {
        // ✅ 使用世界坐标进行碰撞检测
        Vector3 worldPos = self.position;
        Vector2 newWorldPos = worldPos + (Vector3)(direction * moveSpeed * Time.deltaTime);

        // ✅ 计算mask在世界坐标中的边界
        Vector2 maskWorldHalfSize = new Vector2(
            halfSize.x * self.lossyScale.x,
            halfSize.y * self.lossyScale.y
        );

        // ✅ 在世界坐标中进行边界检测
        float left = worldBounds.xMin + maskWorldHalfSize.x;
        float right = worldBounds.xMax - maskWorldHalfSize.x;
        float bottom = worldBounds.yMin + maskWorldHalfSize.y;
        float top = worldBounds.yMax - maskWorldHalfSize.y;

        bool bounced = false;

        if (newWorldPos.x < left)
        {
            newWorldPos.x = left;
            direction.x = Mathf.Abs(direction.x); // 确保方向朝右
            bounced = true;
        }
        if (newWorldPos.x > right)
        {
            newWorldPos.x = right;
            direction.x = -Mathf.Abs(direction.x); // 确保方向朝左
            bounced = true;
        }
        if (newWorldPos.y < bottom)
        {
            newWorldPos.y = bottom;
            direction.y = Mathf.Abs(direction.y); // 确保方向朝上
            bounced = true;
        }
        if (newWorldPos.y > top)
        {
            newWorldPos.y = top;
            direction.y = -Mathf.Abs(direction.y); // 确保方向朝下
            bounced = true;
        }

        // ✅ 更新世界位置
        self.position = newWorldPos;

        // ✅ 如果碰撞，可以稍微调整方向避免卡在角落
        if (bounced)
        {
            // 给一个小的随机偏移，避免卡在边界上
            direction = direction.normalized;
        }
    }

    // ✅ 可选：在编辑器中可视化边界
    private void OnDrawGizmosSelected()
    {
        if (bounds != null && worldBounds.size != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }
    }
}


