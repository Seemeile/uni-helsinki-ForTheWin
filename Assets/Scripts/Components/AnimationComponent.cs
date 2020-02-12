using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct AnimationComponent : IComponentData
{
    public UnitAnimation animationType;
    public int currentFrame;
    public int frameCount;
    public float frameTimer;
    public float frameTimerMax;
}
