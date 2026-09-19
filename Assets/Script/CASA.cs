using UnityEngine;

public class CASA : RECEIVE_DMG
{
    public static Vector3 casaPosition;

    private void Awake()
    {
        casaPosition = transform.position;
    }

}
