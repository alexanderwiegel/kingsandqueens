using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlights : MonoBehaviour
{
    #region fields and properties
    public static Highlights Instance{get; set;}

    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    #endregion
    
    void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
    }

    private GameObject GetHighlightObject() 
    {
        // Findet das erste GameObject in der Liste der Highlights, das nicht aktiv ist
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null) {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }

        return go;
    }

    public void HighlightAllowedMoves(bool[,] moves) {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (moves[i,j]) {
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i*2,0,j*2);
                }
            }
        }
    }

    public void HideHighlights() {
        foreach (GameObject go in highlights) {
            go.SetActive(false);
        }
    }
}
