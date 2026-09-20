using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class INIMIGO : RECEIVE_DMG
{
    [Header("NavMesh e Destino")]
    [SerializeField] private NavMeshAgent e_agent;
    [SerializeField] private Vector3 e_destino;

    [Header("Velocidades")]
    [SerializeField] private float e_velocidadeAtual;
    [SerializeField] private float e_velocidadeNormal;
    [SerializeField] private float e_velocidadeDevagar;

    [Header("Checagem")]
    public bool viuObstaculo=false;
    public bool inimigoViuObstaculo = false;

    [Header("Colisao")]
    [SerializeField] bool showCollisionGizmos = false;
    [SerializeField] private float e_radius = 0.2f;
    [SerializeField] private float e_radiusCauseDMG = 0.3f;
    [SerializeField] private float e_radiusEnemy = 0.025f;
    [SerializeField] private Transform checkObstacle;
    [SerializeField] private Vector3 checkEnemy;

    [Header("Layer de Obstaculos")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask walkableObstacleLayer;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Dano")]
    [SerializeField] private float dmgRate = 1.0f;
    [SerializeField] private bool causandoDano = false;
    [SerializeField] private float dano = 1;

    [Header("Coins")]
    [SerializeField] private int coins=10;

    protected override void Start()
    {
        base.Start();

        //Navegacao
        e_velocidadeAtual = e_velocidadeNormal;
        e_agent.speed = e_velocidadeAtual;
        e_agent.updateRotation = false;
        e_agent.updateUpAxis = false;
        e_agent.SetDestination(e_destino);
    }

    private void OnDrawGizmos()
    {
        if (showCollisionGizmos)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(checkObstacle.position, e_radius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkObstacle.position, e_radiusCauseDMG);

            Gizmos.color = Color.darkRed;
            Gizmos.DrawWireSphere(checkEnemy, e_radiusEnemy);
        }
    }

    public void AlterarVelocidade(float _speed)
    {
        e_velocidadeAtual = _speed;
        e_agent.speed = e_velocidadeAtual;
    }

    public void ReceberDestino()
    {
        e_destino = CASA.casaPosition;
    }

    void ChecarObstaculos()
    {
        bool obstacleFound = Physics.CheckSphere(checkObstacle.position, e_radius, obstacleLayer);
        bool walkableFound = Physics.CheckSphere(checkObstacle.position, e_radius, walkableObstacleLayer);

        if (obstacleFound)
        {
            viuObstaculo = true;
            AlterarVelocidade(0);
            return;
        }
        else if (walkableFound)
        {
            AlterarVelocidade(e_velocidadeDevagar);
        }
        else
        {
            if (e_velocidadeAtual == 0 || e_velocidadeAtual == e_velocidadeDevagar)
            {
                viuObstaculo=false;
                AlterarVelocidade(e_velocidadeNormal);
            }
        }
    }
    void ChecarInimigos()
    {
        Vector3 steeringDirection = e_agent.desiredVelocity.normalized;

        if (e_agent.desiredVelocity.sqrMagnitude > 0.1f)
        {
            checkEnemy = checkObstacle.position + Vector3.ClampMagnitude(steeringDirection, e_radiusEnemy * 1.5f);
        }      

        Collider[] cols = Physics.OverlapSphere(checkEnemy, e_radiusEnemy, enemyLayer);

        if(cols.Length > 0) { 
            for(int i=0; i<cols.Length; i++) {
                if (cols[i].TryGetComponent<INIMIGO>(out INIMIGO _inimigo) && cols[i].gameObject!=gameObject){
                    if ((_inimigo.viuObstaculo || _inimigo.inimigoViuObstaculo) && !inimigoViuObstaculo) {
                        print("Alterou Velocidade");
                        inimigoViuObstaculo = true;
                        AlterarVelocidade(0);
                        return;
                    }
                    else if(!_inimigo.viuObstaculo && !_inimigo.inimigoViuObstaculo)
                    {
                        inimigoViuObstaculo = false;
                        return;
                    }
                   
                }
            }                
        }

        inimigoViuObstaculo = false;
    }

    void TentarCausarDano()
    {
        if (causandoDano) return;

        print("Tentando causar dano");

        Collider[] cols = Physics.OverlapSphere(checkObstacle.position, e_radiusCauseDMG, obstacleLayer);

        if (cols.Length > 0)
        {
            if (cols[0].TryGetComponent(out IDamageable _damageable))
            {
                print("Encontrou IDamageable");

                StartCoroutine(CausarDano(_damageable));
            }

            return;
        }
    }

    IEnumerator CausarDano(IDamageable _damageable)
    {
        print("Causando dano");

        causandoDano = true;

        WaitForSeconds _wait = new WaitForSeconds(dmgRate);
        yield return _wait;
        if(_damageable != null) _damageable.ReceberDano(dano);

        causandoDano = false;
    }

    override protected void Morrer()
    {
        GAME_MANAGER.Instance.AddCoins(coins);

        base.Morrer();
    }

    private void FixedUpdate()
    {
        if (!inimigoViuObstaculo) ChecarObstaculos();
        if(!viuObstaculo) ChecarInimigos();
        if(viuObstaculo) TentarCausarDano();
    }
}
