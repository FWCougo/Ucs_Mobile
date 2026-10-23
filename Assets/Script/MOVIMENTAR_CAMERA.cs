using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public static bool canMoveCamera;


    // IDs dos dedos que começaram o toque em cima da UI (ficam bloqueados até soltar)
    private readonly HashSet<int> fingersBlockedByUI = new HashSet<int>();

    // Reaproveitados para não gerar lixo (GC) a cada toque
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
    private PointerEventData pointerData;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        canMoveCamera = true;
    }

    private void Update()
    {
        TrackTouchesStartedOverUI();

        if (!canMoveCamera) return;

        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (!fingersBlockedByUI.Contains(t.fingerId))
                HandlePan(t);
        }
        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            if (!fingersBlockedByUI.Contains(t0.fingerId) &&
                !fingersBlockedByUI.Contains(t1.fingerId))
                HandlePinchZoom(t0, t1);
        }
    }

    // ------------------------------------------------------------ UI CHECK
    /// <summary>
    /// Verifica TODOS os toques a cada frame: quando um dedo encosta na tela,
    /// decide na hora se ele começou sobre a UI e guarda o resultado até ele soltar.
    /// </summary>
    private void TrackTouchesStartedOverUI()
    {
        if (!ignoreTouchOverUI)
        {
            fingersBlockedByUI.Clear();
            return;
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (IsOverUI(t.position))
                        fingersBlockedByUI.Add(t.fingerId);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    fingersBlockedByUI.Remove(t.fingerId);
                    break;
            }
        }

        if (Input.touchCount == 0)
            fingersBlockedByUI.Clear();
    }

    /// <summary>
    /// Faz um raycast direto na UI usando a posição do toque.
    /// Considera APENAS resultados de GraphicRaycaster (Canvas/UI). Assim, objetos
    /// com collider detectados pelo PhysicsRaycaster / Physics2DRaycaster da câmera
    /// não bloqueiam o movimento.
    /// </summary>
    private bool IsOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        if (pointerData == null)
            pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = screenPosition;

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (raycastResults[i].module is GraphicRaycaster)
                return true;
        }

        return false;
    }

    // ---------------------------------------------------------------- PAN
    private void HandlePan(Touch touch)
    {
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