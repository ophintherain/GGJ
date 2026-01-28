using System;
using UnityEngine;
using System.Collections;

public abstract class DialogBoxBase : MonoBehaviour
{
    public float spawnTime;
    public bool isActive;

    // ⭐ 新增
    protected bool canBeJudged = false;

    public virtual void Spawn(float spawnTime)
    {
        this.spawnTime = spawnTime;
        isActive = true;
        canBeJudged = false; // 默认不能被消除
    }

    public void EnableJudge()
    {
        canBeJudged = true;
    }

    public abstract void HandlePlayerInput();
}


