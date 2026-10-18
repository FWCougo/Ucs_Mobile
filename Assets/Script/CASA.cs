using UnityEngine;

public class CASA : RECEIVE_DMG, ITouchable
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

    public void OnTouched(Vector3 worldPoint)
    {
        print($"Tocou na casa na posicao {worldPoint}");
    }
}
