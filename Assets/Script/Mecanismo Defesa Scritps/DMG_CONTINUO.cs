using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMG_CONTINUO : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float dmgRadius = 0.3f;
    [SerializeField] private float dano = 0.5f;
    [SerializeField] private float dmgRate = 1.0f;
    [SerializeField] private bool drawGizmos = false;

    private readonly Dictionary<INIMIGO, Coroutine> _inimigosEmDano = new();
    private readonly HashSet<INIMIGO> _inimigosNoRaioAgora = new();

    public void Inicializar(float _dmgRadius, float _dano, float _dmgRate)
    {
        dmgRadius = _dmgRadius;
        dano = _dano;
        dmgRate = _dmgRate;
        enemyLayer = LayerMask.GetMask("Enemy"); ;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, dmgRadius);
        }
    }

    private void FixedUpdate()
    {
        print("feito");

        Collider[] cols = Physics.OverlapSphere(transform.position, dmgRadius, enemyLayer);

        _inimigosNoRaioAgora.Clear();

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].TryGetComponent(out INIMIGO _inimigo))
            {
                _inimigosNoRaioAgora.Add(_inimigo);

                if (!_inimigosEmDano.ContainsKey(_inimigo))
                {
                    Coroutine c = StartCoroutine(CausarDano(_inimigo));
                    _inimigosEmDano.Add(_inimigo, c);
                }
            }
        }

        if (_inimigosEmDano.Count > 0)
        {
            List<INIMIGO> _saiuDoRaio = null;

            foreach (var kvp in _inimigosEmDano)
            {
                if (!_inimigosNoRaioAgora.Contains(kvp.Key))
                {
                    (_saiuDoRaio ??= new List<INIMIGO>()).Add(kvp.Key);
                }
            }

            if (_saiuDoRaio != null)
            {
                foreach (var _inimigo in _saiuDoRaio)
                {
                    StopCoroutine(_inimigosEmDano[_inimigo]);
                    _inimigosEmDano.Remove(_inimigo);
                }
            }
        }
    }

    IEnumerator CausarDano(INIMIGO _inimigo)
    {
        IDamageable _damageable = _inimigo.GetComponent<IDamageable>();
        WaitForSeconds _wait = new WaitForSeconds(dmgRate);

        while (_inimigo != null && _inimigo.HP > 0)
        {
            _damageable?.ReceberDano(dano);
            yield return _wait;
        }

        _inimigosEmDano.Remove(_inimigo);
    }
}