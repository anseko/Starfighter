using System.Linq;
using TMPro;
using UnityEngine;

public class GridFiller : MonoBehaviour
{
    private GameObject gridCell;
    [SerializeField] public GameObject grid;
    [SerializeField] private GameObject gridCellPrefab;

    private char[] letters = Enumerable.Range('A', 'T' - 'A' + 1).Select(c => (char) c).ToArray();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var columnIndex in letters)
        {
            for (int rowIndex = 1; rowIndex <= 20; rowIndex++)
            {
                gridCell = Instantiate(gridCellPrefab,grid.transform);
                gridCell.GetComponent<TextMeshPro>().SetText($"{columnIndex}{rowIndex}");
                //gridCell.GetComponent<CellFiller>().Init(transform.position);
            }
        }
    }
}
