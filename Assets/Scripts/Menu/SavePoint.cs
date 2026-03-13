
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Tooltip("Si se deja vacío, se guardara la posicion del mismo GameObject del checkpoint.")]
    public Transform spawnTransform;

    void Reset()
    {
        if (GetComponent<Collider>() == null && GetComponent<Collider2D>() == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        TrySave(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TrySave(other.gameObject);
    }

    void TrySave(GameObject other)
    {
        if (!other.CompareTag("Player")) return;
        Vector3 pos = spawnTransform != null ? spawnTransform.position : transform.position;
        SaveSystem.SavePlayerPosition(pos);
        Debug.Log("SavePoint: Juego guardado.");
    }
}