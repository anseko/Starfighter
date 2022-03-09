using TMPro;
using UnityEngine;

namespace Client
{
    public class GridFiller : MonoBehaviour
    {
        [SerializeField] private GameObject grid;
        [SerializeField] private GameObject gridCellPrefab;


        private void Start()
        {
            for (var columnIndex = 'A'; columnIndex <= 'T'; columnIndex++)
            {
                for (var rowIndex = 1; rowIndex <= 20; rowIndex++)
                {
                    var gridCell = Instantiate(gridCellPrefab, grid.transform);
                    gridCell.GetComponentInChildren<TextMeshPro>().SetText($"{columnIndex}{rowIndex}");
                }
            }
        }
    }
}
