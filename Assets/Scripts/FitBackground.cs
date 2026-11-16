using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FitBackground : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Fit();
    }

    // Update is called once per frame
    void Fit()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;
        Vector2 size = sr.sprite.bounds.size;
        float scale = Mathf.Max(worldW / size.x, worldH / size.y);
        transform.localScale = new Vector3(scale, scale, 1f);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);
    }
}
