using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamController : MonoBehaviour
{
    public float maxTime = 1.5f;
    private float time = 0.0f;

    public LineRenderer lineRenderer;

    float startWitdh;

    private void Start()
    {
        startWitdh = lineRenderer.startWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (time < maxTime)
        {
            float alpha = Mathf.Lerp(1.0f, 0.0f, time / maxTime);

            lineRenderer.startWidth = startWitdh * alpha;
            lineRenderer.endWidth = startWitdh * alpha;


            time += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
