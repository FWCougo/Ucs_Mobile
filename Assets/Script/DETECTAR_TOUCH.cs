using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Detecta TOQUES (tap) na tela e chama ITouchable.OnTouched no objeto atingido.
///
///  - Funciona com colliders 3D e/ou 2D.
///  - Ignora toques que começam sobre a UI (Canvas).
///  - Só conta como "toque" se o dedo não se moveu muito e foi rápido,
///    então arrastar (mover câmera) ou fazer pinça NÃO dispara toque.
///  - No Editor, o mouse (botão esquerdo) simula o toque para facilitar testes.
///
/// Coloque em qualquer objeto da cena (por exemplo, na própria câmera).
/// </summary>
public class DETECTAR_TOUCH : MonoBehaviour
{
    [Header("Detecção")]
    [Tooltip("Câmera usada para o raycast. Se vazio, usa a Camera.main.")]
    [SerializeField] private Camera targetCamera;

    [SerializeField] private LayerMask touchableLayers = ~0;
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private bool detect3D = true;
    [SerializeField] private bool detect2D = false;

    [Header("O que conta como toque (tap)")]
    [Tooltip("Se ligado, dispara ao SOLTAR o dedo (recomendado, evita conflito com arrastar). Desligado, dispara ao encostar.")]
    [SerializeField] private bool fireOnRelease = true;

    [Tooltip("Distância máxima (em pixels) que o dedo pode se mover para ainda ser um toque.")]
    [SerializeField] private float maxMovePixels = 20f;

    [Tooltip("Duração máxima (segundos) entre encostar e soltar para ser um toque.")]
    [SerializeField] private float maxTapDuration = 0.35f;

    [Header("UI")]
    [SerializeField] private bool ignoreTouchOverUI = true;

    // Estado do toque em andamento
    private bool tracking;
    private bool cancelled;
    private bool blockedByUI;
    private Vector2 startPosition;
    private float startTime;

    // Reaproveitados para não gerar lixo
    private readonly List<RaycastResult> uiResults = new List<RaycastResult>();
    private PointerEventData pointerData;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            HandleTouch();
        }
#if UNITY_EDITOR
        else
        {
            HandleMouseInEditor();
        }
#endif
    }

    // ---------------------------------------------------------- TOUCH
    private void HandleTouch()
    {
        // Mais de um dedo = pinça: cancela o toque em andamento
        if (Input.touchCount > 1)
        {
            cancelled = true;
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginPress(touch.position);
                if (tracking && !blockedByUI && !fireOnRelease)
                {
                    TryTouch(touch.position);
                    cancelled = true; // já disparou, não dispara de novo ao soltar
                }
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                UpdatePress(touch.position);
                break;

            case TouchPhase.Ended:
                EndPress(touch.position);
                break;

            case TouchPhase.Canceled:
                ResetState();
                break;
        }
    }

    // ---------------------------------------------------- MOUSE (EDITOR)
#if UNITY_EDITOR
    private void HandleMouseInEditor()
    {
        Vector2 pos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            BeginPress(pos);
            if (tracking && !blockedByUI && !fireOnRelease)
            {
                TryTouch(pos);
                cancelled = true;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            UpdatePress(pos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndPress(pos);
        }
    }
#endif

    // ------------------------------------------------------ LÓGICA COMUM
    private void BeginPress(Vector2 position)
    {
        tracking = true;
        cancelled = false;
        startPosition = position;
        startTime = Time.unscaledTime;
        blockedByUI = ignoreTouchOverUI && IsOverUI(position);
    }

    private void UpdatePress(Vector2 position)
    {
        if (!tracking) return;

        if ((position - startPosition).sqrMagnitude > maxMovePixels * maxMovePixels)
            cancelled = true; // virou arrasto
    }

    private void EndPress(Vector2 position)
    {
        if (tracking && !cancelled && !blockedByUI && fireOnRelease)
        {
            bool fastEnough = (Time.unscaledTime - startTime) <= maxTapDuration;
            bool stillClose = (position - startPosition).sqrMagnitude <= maxMovePixels * maxMovePixels;

            if (fastEnough && stillClose)
                TryTouch(position);
        }

        ResetState();
    }

    private void ResetState()
    {
        tracking = false;
        cancelled = false;
        blockedByUI = false;
    }

    // ------------------------------------------------------------ RAYCAST
    private void TryTouch(Vector2 screenPosition)
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        Ray ray = targetCamera.ScreenPointToRay(screenPosition);

        // 3D
        if (detect3D &&
            Physics.Raycast(ray, out RaycastHit hit3D, maxDistance, touchableLayers))
        {
            // Procura a interface no objeto atingido ou em algum pai
            ITouchable touchable = hit3D.collider.GetComponentInParent<ITouchable>();
            if (touchable != null)
            {
                touchable.OnTouched(hit3D.point);
                return;
            }
        }

        // 2D
        if (detect2D)
        {
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, maxDistance, touchableLayers);
            if (hit2D.collider != null)
            {
                ITouchable touchable = hit2D.collider.GetComponentInParent<ITouchable>();
                if (touchable != null)
                    touchable.OnTouched(hit2D.point);
            }
        }
    }

    // ----------------------------------------------------------- UI CHECK
    // Considera apenas GraphicRaycaster (Canvas), ignorando Physics Raycasters.
    private bool IsOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        if (pointerData == null)
            pointerData = new PointerEventData(EventSystem.current);

        pointerData.position = screenPosition;

        uiResults.Clear();
        EventSystem.current.RaycastAll(pointerData, uiResults);

        for (int i = 0; i < uiResults.Count; i++)
        {
            if (uiResults[i].module is GraphicRaycaster)
                return true;
        }

        return false;
    }
}