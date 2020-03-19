using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Recoil
{
    WeaponData m_weaponData;
    float m_timer = 0f;
    float m_duration;

    public Recoil(WeaponData weaponData)
    {
        m_weaponData = weaponData;

        for(int i = 0; i < weaponData.m_recoilCurve.length; i++)
            m_duration = Mathf.Max(m_duration, weaponData.m_recoilCurve.keys[i].time);
    }


    public float getRecoil
    {
        get
        {
            m_timer += Time.deltaTime;
            return m_weaponData.m_recoilCurve.Evaluate(m_timer);
        }
    }


    public bool finished { get { return m_timer >= m_duration; } }
}
