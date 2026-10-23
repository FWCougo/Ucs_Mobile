using UnityEngine;

public class CASA : RECEIVE_DMG, ITouchable
{
    public static Vector3 casaPosition;

    [Header("CANVAS")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField] private bool canvaAtivo = false;

    private void Awake()
    {
        casaPosition = transform.position;
    }

    protected override void Morrer()
    {
        MENU_MANAGER.Instance.OpenMenu("GAMEOVER_MENU");
        base.Morrer();
    }

    public void AtivarCanva(bool value)
    {
        canvas.gameObject.SetActive(value);
    }

    public void OnTouched(Vector3 worldPoint)
    {
        canvaAtivo = !canvaAtivo;
        AtivarCanva(canvaAtivo);
        
    }
}
