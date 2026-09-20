using UnityEngine;

public class TORRETA : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform canoTorreta;
    [SerializeField] private Transform pontoDisparo;

    [Header("Deteccao")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float alcance = 5f;
    [SerializeField] private float taxaBusca = 0.2f;

    [Header("Rotacao")]
    [SerializeField] private float velRotacao = 180f;
    [SerializeField] private float anguloTolerancia = 5f;

    [Header("Disparo")]
    [SerializeField] private float dano = 0.1f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;

    private INIMIGO inimigoAtual;
    private Quaternion rotInicial;
    private bool miraAlinhada;
    private float proximoTiro;
    private float proximaBusca;

    private readonly Collider[] _bufferCols = new Collider[16];

    /// <summary>
    /// Configura a torreta com os valores vindos do MEC_DEF_SO.
    /// Deve ser chamado logo apos o AddComponent.
    /// </summary>
    public void Configurar(float _dano, float _alcance, float _fireRate, float _velRotacao, LayerMask _enemyLayer)
    {
        dano = _dano;
        alcance = _alcance;
        fireRate = _fireRate;
        velRotacao = _velRotacao;
        enemyLayer = _enemyLayer;
    }

    /// <summary>
    /// Define as referencias de Transform caso a torreta seja criada via codigo.
    /// </summary>
    public void DefinirReferencias(Transform _cano, Transform _pontoDisparo)
    {
        canoTorreta = _cano;
        pontoDisparo = _pontoDisparo;
        rotInicial = canoTorreta.rotation;
    }

    private void Start()
    {
        if (canoTorreta == null)
        {
            Debug.LogError($"[{nameof(TORRETA)}] canoTorreta nao foi atribuido em {name}.", this);
            enabled = false;
            return;
        }

        rotInicial = canoTorreta.rotation;
    }

    private void Update()
    {
        if (Time.time >= proximaBusca)
        {
            proximaBusca = Time.time + taxaBusca;
            BuscarAlvo();
        }

        RotacionarCano();

        if (inimigoAtual != null && miraAlinhada && Time.time >= proximoTiro)
        {
            proximoTiro = Time.time + fireRate;
            Atirar();
        }
    }

    /// <summary>
    /// Procura o inimigo mais proximo dentro do alcance. Mantem o alvo atual
    /// enquanto ele continuar vivo e dentro do raio.
    /// </summary>
    private void BuscarAlvo()
    {
        if (AlvoValido(inimigoAtual)) return;

        inimigoAtual = null;

        int _qtd = Physics.OverlapSphereNonAlloc(transform.position, alcance, _bufferCols, enemyLayer);
        float _menorDist = float.MaxValue;

        for (int i = 0; i < _qtd; i++)
        {
            if (!_bufferCols[i].TryGetComponent(out INIMIGO _inimigo)) continue;
            if (_inimigo.HP <= 0) continue;

            float _dist = (_inimigo.transform.position - transform.position).sqrMagnitude;

            if (_dist < _menorDist)
            {
                _menorDist = _dist;
                inimigoAtual = _inimigo;
            }
        }
    }

    private bool AlvoValido(INIMIGO _inimigo)
    {
        if (_inimigo == null) return false;
        if (!_inimigo.gameObject.activeInHierarchy) return false;
        if (_inimigo.HP <= 0) return false;

        float _dist = (_inimigo.transform.position - transform.position).sqrMagnitude;
        return _dist <= alcance * alcance;
    }

    /// <summary>
    /// Gira o cano suavemente em direcao ao alvo, ou de volta a rotacao inicial
    /// quando nao ha alvo. Atualiza o estado de mira alinhada.
    /// </summary>
    private void RotacionarCano()
    {
        Quaternion _rotAlvo = rotInicial;

        if (inimigoAtual != null)
        {
            Vector3 _lookDir = inimigoAtual.transform.position - transform.position;
            _lookDir.y = 0f;

            if (_lookDir.sqrMagnitude > 0.0001f)
            {
                _rotAlvo = Quaternion.LookRotation(_lookDir);
            }
        }

        canoTorreta.rotation = Quaternion.RotateTowards(
            canoTorreta.rotation,
            _rotAlvo,
            velRotacao * Time.deltaTime
        );

        miraAlinhada = Quaternion.Angle(canoTorreta.rotation, _rotAlvo) <= anguloTolerancia;
    }

    private void Atirar()
    {
        if (inimigoAtual.TryGetComponent(out IDamageable _damageable))
        {
            _damageable.ReceberDano(dano);
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, alcance);

        if (inimigoAtual != null)
        {
            Gizmos.color = miraAlinhada ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, inimigoAtual.transform.position);
        }
    }

#endif
}