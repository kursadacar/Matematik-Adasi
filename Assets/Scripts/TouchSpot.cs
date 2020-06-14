using UnityEngine;

public class TouchSpot : MonoBehaviour
{
    public enum SpotType
    {
        Shop,
        FishingSpot
    }

    public Shop target;

    public SpotType type;
}
