using UnityEngine;

public interface IDamageable
{
    public void ReceberDano(float _dano);
}

public interface ITouchable
{
    /// <param name="worldPoint">Ponto do mundo onde o toque atingiu o collider.</param>
    void OnTouched(Vector3 worldPoint);
}