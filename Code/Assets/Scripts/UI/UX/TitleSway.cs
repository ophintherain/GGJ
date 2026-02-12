using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TitleSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAngle = 5f;
    public float swayDuration = 2f;
    public Ease swayEase = Ease.InOutSine;
    
    [Header("Delay Settings")]
    public float initialDelay = 0f; 
    public bool randomStartOffset = true;
    
    private RectTransform rectTransform;
    private Sequence swaySequence;
    private Quaternion originalRotation;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return;
        }
        
        originalRotation = rectTransform.localRotation;
        
        if (randomStartOffset)
        {
            float randomOffset = Random.Range(0f, swayDuration);
            DOVirtual.DelayedCall(randomOffset, StartSway);
        }
        else
        {
            DOVirtual.DelayedCall(initialDelay, StartSway);
        }
    }
    
    void StartSway()
    {
        swaySequence = DOTween.Sequence();
        
        swaySequence.Append(rectTransform.DOLocalRotate(
            new Vector3(0, 0, swayAngle), 
            swayDuration / 2
        ).SetEase(swayEase));
        
        swaySequence.Append(rectTransform.DOLocalRotate(
            new Vector3(0, 0, -swayAngle), 
            swayDuration
        ).SetEase(swayEase));
        
        swaySequence.Append(rectTransform.DOLocalRotate(
            new Vector3(0, 0, 0), 
            swayDuration / 2
        ).SetEase(swayEase));
        
        swaySequence.SetLoops(-1, LoopType.Restart);
    }
    
    void OnDestroy()
    {
        if (swaySequence != null && swaySequence.IsActive())
        {
            swaySequence.Kill();
        }
        
        if (rectTransform != null)
        {
            rectTransform.localRotation = originalRotation;
        }
    }
}