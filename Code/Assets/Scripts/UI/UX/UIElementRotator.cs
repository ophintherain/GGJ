using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIElementRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public RotateMode rotateMode = RotateMode.Fast;  // 旋转模式
    public float rotateSpeed = 50f;                  // 旋转速度
    public bool clockwise = true;                   // 顺时针旋转
    
    [Header("Heatmap Settings")]
    public bool randomStartRotation = true;         // 随机起始角度
    public bool usePulseEffect = false;             // 使用脉冲效果
    public float pulseScale = 0.2f;                // 脉冲缩放幅度
    public float pulseDuration = 1f;               // 脉冲周期
    
    [Header("3D Rotation Settings")]
    public bool rotateIn3D = false;                // 是否使用3D旋转
    public Vector3 rotationAxis = Vector3.forward; // 3D旋转轴
    
    private RectTransform rectTransform;
    private Transform objectTransform;
    private Tween rotateTween;
    private Tween pulseTween;
    private Sequence combinedSequence;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        objectTransform = transform;
        
        if (randomStartRotation)
        {
            float randomZ = Random.Range(0f, 360f);
            if (rectTransform != null)
            {
                rectTransform.localRotation = Quaternion.Euler(0, 0, randomZ);
            }
            else
            {
                objectTransform.localRotation = Quaternion.Euler(0, 0, randomZ);
            }
        }
        
        StartRotation();
        
        if (usePulseEffect)
        {
            StartPulse();
        }
    }
    
    void StartRotation()
    {
        float speed = clockwise ? rotateSpeed : -rotateSpeed;
        
        if (rotateIn3D)
        {
            if (rectTransform != null)
            {
                rotateTween = rectTransform.DORotate(
                    rotationAxis * 360f,
                    360f / Mathf.Abs(speed),
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear).SetLoops(-1);
            }
            else
            {
                rotateTween = objectTransform.DORotate(
                    rotationAxis * 360f,
                    360f / Mathf.Abs(speed),
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear).SetLoops(-1);
            }
        }
        else
        {
            if (rectTransform != null)
            {
                rotateTween = rectTransform.DORotate(
                    new Vector3(0, 0, 360f),
                    360f / Mathf.Abs(speed),
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear).SetLoops(-1);
            }
            else
            {
                rotateTween = objectTransform.DORotate(
                    new Vector3(0, 0, 360f),
                    360f / Mathf.Abs(speed),
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear).SetLoops(-1);
            }
        }
        
        if (!clockwise)
        {
            rotateTween = DOTween.To(() => 0f, x => {}, 0f, 0f); // 实际DOTween会自动处理负速度
        }
    }
    
    void StartPulse()
    {
        if (rectTransform == null) return;
        
        Vector3 originalScale = rectTransform.localScale;
        Vector3 pulseScaleVector = new Vector3(
            originalScale.x * (1 + pulseScale),
            originalScale.y * (1 + pulseScale),
            originalScale.z * (1 + pulseScale)
        );
        
        pulseTween = rectTransform.DOScale(pulseScaleVector, pulseDuration / 2)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => {
                rectTransform.DOScale(originalScale, pulseDuration / 2)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(StartPulse);
            });
    }
    
    void OnDestroy()
    {
        if (rotateTween != null && rotateTween.IsActive())
        {
            rotateTween.Kill();
        }
        
        if (pulseTween != null && pulseTween.IsActive())
        {
            pulseTween.Kill();
        }
        
        if (combinedSequence != null && combinedSequence.IsActive())
        {
            combinedSequence.Kill();
        }
    }
    
    void OnDisable()
    {
        if (rotateTween != null && rotateTween.IsActive())
        {
            rotateTween.Pause();
        }
        if (pulseTween != null && pulseTween.IsActive())
        {
            pulseTween.Pause();
        }
    }
    
    void OnEnable()
    {
        if (rotateTween != null && rotateTween.IsActive())
        {
            rotateTween.Play();
        }
        if (pulseTween != null && pulseTween.IsActive())
        {
            pulseTween.Play();
        }
    }
}