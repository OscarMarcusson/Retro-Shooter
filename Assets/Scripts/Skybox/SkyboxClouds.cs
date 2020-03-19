using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxClouds : MonoBehaviour
{
    float m_timer = 0f;

    public float m_speed = 1f;
    [Range(0.5f, 1f)] public float m_distance = 1f;
    public bool m_flip = false;
    Vector3 m_scale;




    void Start()
    {
        m_timer = Random.value;
        m_scale = new Vector3(m_flip ? -1f : 1f, 1f, 1f);
    }


    void LateUpdate()
    {
        m_timer += Time.deltaTime * m_speed;
        m_timer %= 360f;

        transform.eulerAngles = new Vector3(0f, m_timer, 0f);
        transform.localScale = m_scale * Camera.main.farClipPlane * 0.9f * m_distance;
        transform.position = Camera.main.transform.position;
    }
}
