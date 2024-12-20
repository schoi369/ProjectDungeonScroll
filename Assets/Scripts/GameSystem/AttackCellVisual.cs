using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCellVisual : MonoBehaviour
{
    bool Initiated { get; set; } = false;
    float InitTime { get; set; }
    public void Init()
    {
        Initiated = true;
        InitTime = Time.time;
    }

    public void Update()
    {
        if (Initiated)
        {            
            if (Time.time > InitTime + .5f)
            {
                ResetVisual();
            }
        }
    }

    void ResetVisual()
    {
        Initiated = false;
        AttackCellVisualPool.Instance.RemoveVisual(this);
    }
}
