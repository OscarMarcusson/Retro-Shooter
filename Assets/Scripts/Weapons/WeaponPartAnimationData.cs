using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NOT YET IMPLEMENTED


[System.Serializable]
public class WeaponPartAnimationData
{
    public string m_name = "Transform Name";
    [Space(5f)]
    public AnimationCurve m_positionX;
    public AnimationCurve m_positionY;
    public AnimationCurve m_positionZ;
    [Space(5f)]
    public AnimationCurve m_rotationX;
    public AnimationCurve m_rotationY;
    public AnimationCurve m_rotationZ;
}
