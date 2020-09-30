using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour {
    #region fields and properties
    public static GameState Instance {get; set;}
    private bool[,] allowedMoves {get; set;}

    // Empty, das alle Felder beinhaltet
    public GameObject spielfeld;
    
    // 2D-Schachfiguren-Array, in dem Positionen der Figuren gespeichert werden
    // sowie dazugehörige Property
    public Schachfigur[,] Schachfiguren {get; set;} 

    // Für bessere Beschriftung
    string[] columns = {"A","B","C","D","E","F","G","H"};

    // Beinhaltet alle 12 Prefabs (6 Figuren in je 2 Farben)
    public List<GameObject> prefabs;

    // Liste der lebenden Schachfiguren
    List<GameObject> schachfiguren;

    Schachfigur selectedPiece;
    public bool isWhiteTurn = true;
    public bool changePerspective = false;

    private Material previousMat;
    public Material selectedMat;

    public int[] EnPassantMove {get; set;}
    #endregion

    #region methods
    void Start() {
        Instance = this;
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
            // Auswahl nur möglich wenn keine Kamerafahrt stattfindet
            if (Input.GetMouseButtonDown(0) && changePerspective == false) {
                Vector3 position = hit.transform.position;
                int x = (int)position.x/2;
                int z = (int)position.z/2;

                // Erstauswahl
                if (selectedPiece == null) SelectPiece(x,z);
                // Bewegung
                else {
                    if (allowedMoves[x,z]) {
                        Schachfigur enemy = Schachfiguren[x,z];
                        // wenn Figur auf Gegner stoßen wird
                        if (enemy != null && enemy.isWhite != isWhiteTurn) {
                            // wenn Gegner König ist, ist das Spiel vorbei
                            // TODO: Schach + Schachmatt implementieren
                            if (enemy.Title == "König") return;

                            // Gegner entfernen
                            schachfiguren.Remove(enemy.gameObject);
                            Destroy(enemy.gameObject);
                        } 

                        #region en passant Bewegung des Bauern
                        // wenn en passant Bewegung ausgeführt wird
                        if (x == EnPassantMove[0] && z == EnPassantMove[1]) {
                            // an der schwarzen Front
                            if (isWhiteTurn) enemy = Schachfiguren[x,z-1];
                            // an der weißen Front
                            else enemy = Schachfiguren[x,z+1];
                            // Gegner entfernen
                            schachfiguren.Remove(enemy.gameObject);
                            Destroy(enemy.gameObject);
                        }
                        EnPassantMove[0] = -1;
                        EnPassantMove[1] = -1;
                        if (selectedPiece.Title == "Bauer") {
                            // wenn weißer Bauer noch auf Startposition ist und zwei Schritte vor geht
                            if (selectedPiece.Z == 1 && z == 3) {
                                EnPassantMove[0] = x;
                                EnPassantMove[1] = z-1;
                            }
                            // wenn schwarzer Bauer noch auf Startposition ist und zwei Schritte vor geht
                            else if (selectedPiece.Z == 6 && z == 4) {
                                EnPassantMove[0] = x;
                                EnPassantMove[1] = z+1;
                            }
                        #endregion

                        #region Umwandlung des Bauern
                            // wenn ein Bauer das jeweilige Ende des Spielfelds erreicht hat
                            if (z == 7 || z == 0) {
                                schachfiguren.Remove(selectedPiece.gameObject);
                                Destroy(selectedPiece.gameObject);
                                // TODO: hier Auswahl-Interface für Dame, Turm, Läufer und Springer anzeigen
                                if (z == 7) CreateSingleChessPiece(3,x,z);
                                else if (z == 0) CreateSingleChessPiece(9,x,z);
                                selectedPiece = Schachfiguren[x,z];
                            }
                        }
                        #endregion

                        #region Rochade
                        if (selectedPiece.Title == "König") {
                            Schachfigur turm;
                            #region kurze Rochade
                            if (x == selectedPiece.X+2) {
                                turm = Schachfiguren[x+1, z];
                                // an der bisherigen Position des Turms steht dieser gleich nicht mehr
                                Schachfiguren[x+1, z] = null;
                                // physische Bewegung des Turms links vom König
                                turm.transform.position = new Vector3((x-1)*2, 0, z*2);
                                // speichern der neuen Position des Turms
                                Schachfiguren[x-1,z] = turm;
                                turm.Move(x-1,z);
                                // Bewegung speichern (für Rochade wichtig)
                                turm.hasMoved = true;
                            }
                            #endregion

                            #region lange Rochade
                            else if (x == selectedPiece.X-2) {
                                turm = Schachfiguren[x-2, z];
                                // an der bisherigen Position des Turms steht dieser gleich nicht mehr
                                Schachfiguren[x-2, z] = null;
                                // physische Bewegung des Turms rechts vom König
                                turm.transform.position = new Vector3((x+1)*2, 0, z*2);
                                // speichern der neuen Position des Turms
                                Schachfiguren[x+1,z] = turm;
                                turm.Move(x+1,z);
                                // Bewegung speichern (für Rochade wichtig)
                                turm.hasMoved = true;
                            }
                            #endregion
                        }
                        #endregion

                        // an der bisherigen Position der Figur steht gleich keine mehr
                        Schachfiguren[selectedPiece.X, selectedPiece.Z] = null;
                        // physische Bewegung der Figur
                        selectedPiece.transform.position = new Vector3(x*2, 0, z*2);
                        // speichern der neuen Position der Figur
                        Schachfiguren[x,z] = selectedPiece;
                        selectedPiece.Move(x,z);
                        // Bewegung speichern (für Rochade wichtig)
                        selectedPiece.hasMoved = true;
                        // Wechsel
                        isWhiteTurn = !isWhiteTurn;
                        changePerspective = true;
                        CameraController.Instance.xAligned = false;
                        CameraController.Instance.yAligned = false;
                    }
                    // Auswahl aufheben
                    selectedPiece.GetComponent<MeshRenderer>().material = previousMat;
                    selectedPiece = null;
                    Highlights.Instance.HideHighlights();
                }
                
            }
        }       
    }

    void SelectPiece(int x, int z) {
        // wenn dort keine Figur steht, wird nichts ausgewählt
        if (Schachfiguren[x,z] == null) return;
        // wenn die Figur dem Gegner gehört, wird sie nicht ausgewählt
        else if (Schachfiguren[x,z].isWhite != isWhiteTurn) return; 
        else {
            selectedPiece = Schachfiguren[x,z];
            // Hervorhebung der Figur
            previousMat = selectedPiece.GetComponent<MeshRenderer>().material;
            selectedMat.mainTexture = previousMat.mainTexture;
            selectedPiece.GetComponent<MeshRenderer>().material = selectedMat;
            // Highlighting der Züge
            allowedMoves = selectedPiece.PossibleMovements();
            Highlights.Instance.HighlightAllowedMoves(allowedMoves);
            //print(selectedPiece);
        }
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

    /*
    void CreateSingleChessField(int row, int column, Color color) {
        GameObject feld = GameObject.CreatePrimitive(PrimitiveType.Plane);
        feld.GetComponent<Renderer>().material.color = color;
        int number = row + 1;
        feld.name = columns[column] + number;
        feld.transform.Translate(2*column, 0, 2*row);
        feld.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        feld.transform.parent = spielfeld.transform;
    }
    */

    public void CreateSquare(int row, int column, Color color) {
        GameObject feld = new GameObject();
        int number = row + 1;
        feld.name = columns[column] + number;
        feld.transform.parent = spielfeld.transform;
        CreateTriangle(row, column, color, true).transform.parent = feld.transform;
        CreateTriangle(row, column, color, false).transform.parent = feld.transform;
    }

    GameObject CreateTriangle(int row, int column, Color color, bool isLowerTriangle) {
        GameObject triangle = new GameObject();
        triangle.AddComponent<MeshFilter>();
        triangle.AddComponent<MeshRenderer>();
        Mesh mesh = triangle.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        if (isLowerTriangle) {
            mesh.vertices = new Vector3[] {
                new Vector3(-1 + row*2, 0, -1 + column*2), 
                new Vector3(-1 + row*2, 0,  1 + column*2), 
                new Vector3( 1 + row*2, 0, -1 + column*2)
            };
        }
        else {
            mesh.vertices = new Vector3[] {
                new Vector3(-1 + row*2, 0,  1 + column*2), 
                new Vector3( 1 + row*2, 0,  1 + column*2), 
                new Vector3( 1 + row*2, 0, -1 + column*2)
            };
        }
        mesh.uv = new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,1)};
        mesh.triangles = new int[] {0,1,2};
        Renderer rend = triangle.GetComponent<Renderer>();
        rend.material = feldMat;
        mesh.colors = new Color[]{color, color, color};
        return triangle;
    }
    
    void CreateChessPieces() {
        schachfiguren = new List<GameObject>();
        Schachfiguren = new Schachfigur[8,8];
        EnPassantMove = new int[2]{-1,-1};

        // (PrefabNr, x, y)
        #region weiß
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

        #region schwarz
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
    }
    #endregion
}
