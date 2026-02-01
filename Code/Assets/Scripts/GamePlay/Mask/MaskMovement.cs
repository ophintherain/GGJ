using UnityEngine;

public class MaskMovement : MonoBehaviour
{
    private float moveSpeed;
    private Vector2 direction;

    private RectTransform self;
    private RectTransform bounds;  // ✅ 移动范围（你说的 BG）
    private Vector2 halfSize;

    private void Awake()
    {
        self = GetComponent<RectTransform>();
        halfSize = self.rect.size * 0.5f;
    }

    // ✅ 由外部（MaskController）注入边界和速度
    public void Setup(RectTransform b, float speed)
    {
        if (b == null)
        {
            Debug.LogError($"Setup: bounds is null for {name}");
            return;
        }

        bounds = b;
        moveSpeed = speed;

    }

    private void OnEnable()
    {
        // 每次激活给个新方向
        direction = Random.insideUnitCircle.normalized;
    }

    private void Update()
    {
        if (bounds == null)
        {
            Debug.LogError($"{name} bounds still NULL in Update");
            return;
        }

        // 添加调试信息
        if (Time.frameCount % 60 == 0) // 每秒打印一次
        {
            Debug.Log($"{name}: bounds = {bounds.name}, bounds.rect = {bounds.rect}, speed = {moveSpeed}");
            Debug.Log($"{name}: pos = {self.anchoredPosition}, direction = {direction}");
        }

        MoveAndBounce();
    }

    private void MoveAndBounce()
    {
        Vector2 pos = self.anchoredPosition;
        pos += direction * moveSpeed * Time.deltaTime;

        Rect r = bounds.rect;

        float left = r.xMin + halfSize.x;
        float right = r.xMax - halfSize.x;
        float bottom = r.yMin + halfSize.y;
        float top = r.yMax - halfSize.y;

        if (pos.x < left) { pos.x = left; direction.x *= -1; }
        if (pos.x > right) { pos.x = right; direction.x *= -1; }
        if (pos.y < bottom) { pos.y = bottom; direction.y *= -1; }
        if (pos.y > top) { pos.y = top; direction.y *= -1; }

        self.anchoredPosition = pos;
    }
}


