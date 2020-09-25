using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour {
    #region fields and properties
    // Empty, das alle Felder beinhaltet
    public GameObject spielfeld;
    
    // 2D-Schachfiguren-Array, in dem Positionen der Figuren gespeichert werden
    // sowie dazugehörige Property
    Schachfigur[,] Schachfiguren {get; set;} 

    // Für bessere Beschriftung
    string[] columns = {"A","B","C","D","E","F","G","H"};

    // Beinhaltet alle 12 Prefabs (6 Figuren in je 2 Farben)
    public List<GameObject> prefabs;

    // Liste der lebenden Schachfiguren
    List<GameObject> schachfiguren;

    Schachfigur selectedPiece;
    bool isWhiteTurn = true;
    #endregion

    #region methods
    void Start() {
        CreateChessBoard();
        CreateChessPieces();
    }

    void Update() {
        // Mauskoordinaten in Pixel
        Vector3 mouse = Input.mousePosition;
        // Mauskoordinaten umgewandelt in einen Ray
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        // Referenz auf das getroffene Objekt
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity)) {
            if (Input.GetMouseButtonDown(0)) {
                Vector3 position = hit.transform.position;
                print((int)position.x/2 + " " + (int)position.z/2);
                SelectPiece((int)position.x/2, (int)position.z/2);
            }
        }       
    }

    void SelectPiece(int x, int z) {
        if (Schachfiguren[x,z] == null) return;
        else if (Schachfiguren[x,z].isWhite != isWhiteTurn) return; 
        else selectedPiece = Schachfiguren[x,z];
        print(selectedPiece);
    }

    void CreateChessBoard() {
        Color color;
        for (int row = 0; row < 8; row++) {
            for (int column = 0; column < 8; column++) {
                if (row % 2 == 0 && column % 2 == 0 || row % 2 == 1 && column % 2 == 1) color = Color.black;
                else color = Color.white;
                CreateSingleChessField(row, column, color);
            }
        }
    }

    // TODO: hier Meshes statt Planes benutzen
    void CreateSingleChessField(int row, int column, Color color) {
        GameObject feld = GameObject.CreatePrimitive(PrimitiveType.Plane);
        feld.GetComponent<Renderer>().material.color = color;
        int number = row + 1;
        feld.name = columns[column] + number;
        feld.transform.Translate(2*column, 0, 2*row);
        feld.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        feld.transform.parent = spielfeld.transform;
    }
    
    void CreateChessPieces() {
        schachfiguren = new List<GameObject>();
        Schachfiguren = new Schachfigur[8,8];
        
        // (PrefabNr, x, y)
        #region Weiß
        // Türme
        CreateSingleChessPiece(0,0,0);
        CreateSingleChessPiece(0,7,0);
        // Springer
        CreateSingleChessPiece(1,1,0);
        CreateSingleChessPiece(1,6,0);
        // Läufer
        CreateSingleChessPiece(2,2,0);
        CreateSingleChessPiece(2,5,0);
        // Dame
        CreateSingleChessPiece(3,3,0);
        // König
        CreateSingleChessPiece(4,4,0);
        // Bauern
        for (int i = 0; i < 8; i++) {
            CreateSingleChessPiece(5,1*i,1);
        }
        #endregion

        #region Schwarz
        // Türme
        CreateSingleChessPiece(6,0,7);
        CreateSingleChessPiece(6,7,7);
        // Springer
        CreateSingleChessPiece(7,1,7);
        CreateSingleChessPiece(7,6,7);
        // Läufer
        CreateSingleChessPiece(8,2,7);
        CreateSingleChessPiece(8,5,7);
        // Dame
        CreateSingleChessPiece(9,3,7);
        // König
        CreateSingleChessPiece(10,4,7);
        // Bauern
        for (int i = 0; i < 8; i++) {
            CreateSingleChessPiece(11,1*i,6);
        }
        #endregion
    }

    void CreateSingleChessPiece(int index, int x, int z) {
        GameObject figur;
        figur = Instantiate(prefabs[index], new Vector3(x*2, 0, z*2), Quaternion.identity) as GameObject;
        schachfiguren.Add(figur);
        // Setzt Schachfiguren im imaginären Schachbrett auf Initialposition
        Schachfiguren[x,z] = figur.GetComponent<Schachfigur>();
        Schachfiguren[x,z].Move(x,z);
        //print(figur.GetComponent<Schachfigur>());
    }
    #endregion
}
