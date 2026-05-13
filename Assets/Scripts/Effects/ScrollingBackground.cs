using UnityEngine;
using UnityEngine.Tilemaps;

public class ScrollingBackground : MonoBehaviour
{
    public float velocidadX = 0.1f;
    public float velocidadY = 0.05f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<TilemapRenderer>().material;
    }

    void Update()
    {
        float offsetX = Time.time * velocidadX;
        float offsetY = Time.time * velocidadY;

        mat.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}