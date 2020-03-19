using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// NOT YET IMPLEMENTED


public class WeaponPartAnimation
{
    readonly Transform m_transform;
    readonly WeaponData m_weaponData;
    List<AnimationInstance> m_animationInstances;

    public WeaponPartAnimation(Transform transform, WeaponData weaponData)
    {
        m_transform = transform;
        m_weaponData = weaponData;
        m_animationInstances = new List<AnimationInstance>();
    }


    /// <summary>
    /// Starts the animation.
    /// </summary>
    public void Play()
    {
        m_animationInstances.Add(new AnimationInstance(m_transform));
    }


    /// <summary>
    /// Renders the animation. Should be called once per frame. 
    /// </summary>
    public void Animate()
    {
        for(int i = 0; i < m_animationInstances.Count; i++)
        {
            m_animationInstances[i].Animate();
            if (m_animationInstances[i].finished)
            {
                m_animationInstances.RemoveAt(i);
                i--;
            }
        }
    }


    /// <summary>
    /// Simple class to call the animations for a part.
    /// </summary>
    private class AnimationInstance
    {
        readonly Transform m_transform;
        float m_timer;

        public AnimationInstance(Transform transform)
        {
            m_transform = transform;
            m_timer = 0f;
        }


        public void Animate()
        {
            // Implement
        }


        public bool finished { get { return m_timer >= 1f; } }
    }
}
