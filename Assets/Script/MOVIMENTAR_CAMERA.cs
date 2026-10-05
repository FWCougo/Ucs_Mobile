using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controle de câmera por touch:
///  - 1 dedo arrastando  = move a câmera (pan)
///  - 2 dedos (pinça)    = zoom in / zoom out
///
/// Funciona com câmera Ortográfica (2D) e Perspectiva (3D).
/// Use o Input Manager antigo (Project Settings > Player > Active Input Handling = "Input Manager" ou "Both").
/// </summary>
[RequireComponent(typeof(Camera))]
public class MOVIMENTAR_CAMERA : MonoBehaviour
{
    [Header("Pan (1 dedo)")]
    [Tooltip("Velocidade do arrasto. 1 = a cena acompanha o dedo.")]
    [SerializeField] private float panSpeed = 1f;

    [Tooltip("Se ligado, move no plano XZ (jogos 3D vistos de cima). Desligado, move no plano da câmera (2D).")]
    [SerializeField] private bool moveOnXZPlane = false;

    [Tooltip("Distância usada para converter pixels em unidades no modo Perspectiva.")]
    [SerializeField] private float perspectivePanDistance = 10f;

    [Header("Zoom (2 dedos)")]
    [SerializeField] private float zoomSpeed = 0.01f;
    [SerializeField] private float minZoom = 2f;   // Ortho: orthographicSize | Perspectiva: FOV
    [SerializeField] private float maxZoom = 20f;  // Para perspectiva, use por ex. min 20 e max 80

    [Header("Limites da câmera (opcional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPosition = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 maxPosition = new Vector2(50f, 50f);

    [Header("UI")]
    [Tooltip("Ignora toques que começam sobre elementos de UI (botões, painéis...).")]
    [SerializeField] private bool ignoreTouchOverUI = true;

    private Camera cam;
    private bool panBlockedByUI;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.touchCount == 1)
        {
            HandlePan(Input.GetTouch(0));
        }
        else if (Input.touchCount == 2)
        {
            HandlePinchZoom(Input.GetTouch(0), Input.GetTouch(1));
        }
    }

    // ---------------------------------------------------------------- PAN
    private void HandlePan(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            panBlockedByUI = ignoreTouchOverUI &&
                             EventSystem.current != null &&
                             EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }

        //if (panBlockedByUI || touch.phase != TouchPhase.Moved)
        if (panBlockedByUI)
        return;

        // Quantas unidades do mundo equivalem a 1 pixel da tela
        float worldPerPixel;
        if (cam.orthographic)
        {
            worldPerPixel = (cam.orthographicSize * 2f) / Screen.height;
        }
        else
        {
            worldPerPixel = (2f * perspectivePanDistance *
                             Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad)) / Screen.height;
        }

        Vector2 delta = touch.deltaPosition * worldPerPixel * panSpeed;

        // Arrastar para a direita move a câmera para a esquerda (a cena "segue" o dedo)
        Vector3 move;
        if (moveOnXZPlane)
        {
            Vector3 right = transform.right;
            Vector3 forward = transform.forward;
            right.y = 0f;
            forward.y = 0f;
            right.Normalize();
            forward.Normalize();

            // Câmera olhando de cima: transform.up aponta "para frente" no plano
            Vector3 up = Vector3.Cross(Vector3.up, right).normalized;
            if (Vector3.Dot(up, transform.up) < 0f) up = -up;
            if (up.sqrMagnitude < 0.001f) up = forward;

            move = -(right * delta.x + up * delta.y);
        }
        else
        {
            move = -(transform.right * delta.x + transform.up * delta.y);
        }

        transform.position += move;
        ClampPosition();
    }

    // --------------------------------------------------------------- ZOOM
    private void HandlePinchZoom(Touch t0, Touch t1)
    {
        // Posições dos dedos no frame anterior
        Vector2 prev0 = t0.position - t0.deltaPosition;
        Vector2 prev1 = t1.position - t1.deltaPosition;

        float prevDistance = (prev0 - prev1).magnitude;
        float currentDistance = (t0.position - t1.position).magnitude;

        // Positivo = dedos se afastando (zoom in) | Negativo = dedos se aproximando (zoom out)
        float difference = currentDistance - prevDistance;

        if (cam.orthographic)
        {
            // Ortho: tamanho MENOR = mais zoom
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - difference * zoomSpeed,
                minZoom, maxZoom);
        }
        else
        {
            // Perspectiva: FOV MENOR = mais zoom
            cam.fieldOfView = Mathf.Clamp(
                cam.fieldOfView - difference * zoomSpeed,
                minZoom, maxZoom);
        }

        ClampPosition();
    }

    // ------------------------------------------------------------- LIMITES
    private void ClampPosition()
    {
        if (!useBounds) return;

        Vector3 pos = transform.position;

        if (moveOnXZPlane)
        {
            pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
            pos.z = Mathf.Clamp(pos.z, minPosition.y, maxPosition.y);
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
        }

        transform.position = pos;
    }
}