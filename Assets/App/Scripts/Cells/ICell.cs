using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICell
{
    void SetParent(Transform parent);
    void SetCellData(CellData cellType);
}