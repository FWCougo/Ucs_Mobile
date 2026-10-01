using UnityEngine;

public class CASA : RECEIVE_DMG
{
    public static Vector3 casaPosition;

    private void Awake()
    {
        casaPosition = transform.position;
    }


    protected override void Morrer()
    {
        MENU_MANAGER.Instance.OpenMenu("GAMEOVER_MENU");
        base.Morrer();
    }


}
