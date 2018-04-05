using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun Settings")]
    public bool useShotgunSpread;
    //How big the pellet spread area will be
    public float spreadSize = 2.0f;
    //How many pellets to shoot
    public int pellets = 30;
}
