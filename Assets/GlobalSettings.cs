using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public ushort serverLevel;
    public List<Color> playerColors;
    public float playerSpeed;
    public int maxHp;
    public float worldSize;

    [Header("Dynamic Variables")]
    public bool modalOpen;

    public static float RealArctan(Vector2 vec)
    {
        if (vec.x == 0 && vec.y == 0) return 0f;
        else if (vec.x == 0 && vec.y > 0) return 90f;
        else if (vec.x == 0 && vec.y < 0) return -90f;
        else if (vec.x < 0) return Mathf.Atan(vec.y / vec.x) * Mathf.Rad2Deg + 180f;
        else return Mathf.Atan(vec.y / vec.x) * Mathf.Rad2Deg;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
