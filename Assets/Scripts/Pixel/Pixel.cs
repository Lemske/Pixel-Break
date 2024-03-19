using UnityEngine;

public class Pixel
{
    public GameObject pixel;
    public int x;
    public int y;

    public Pixel(int x, int y, GameObject pixel)
    {
        this.x = x;
        this.y = y;
        this.pixel = pixel;
    }
}
