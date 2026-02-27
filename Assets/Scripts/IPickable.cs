using UnityEngine;

public interface IPickable
{
    bool IsPicked { get; }
    void OnPickUp(GameObject picker);
    void OnDrop();
}