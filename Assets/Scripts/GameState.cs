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

    GameObject[,] Felder {get; set;}

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
    public Material feldMat;

    public int[] EnPassantMove {get; set;}
    bool schach = false;
    public bool[,] whiteMoves;
    public bool[,] blackMoves; 
    #endregion

    #region methods
    void Start() {
        Felder = new GameObject[8,8];
        Instance = this;
        CreateChessBoard();
        CreateChessPieces();
    }

    void Update() {
        #region Auswahl
        // Mauskoordinaten in Pixel
        Vector3 mouse = Input.mousePosition;
        // Mauskoordinaten umgewandelt in einen Ray
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        // Referenz auf das getroffene Objekt
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity)) {
            // Auswahl nur möglich wenn keine Kamerafahrt stattfindet
            if (Input.GetMouseButtonDown(0) && changePerspective == false) {
                if (hit.transform.tag == "Figur" || hit.transform.tag == "Feld") {
                    Vector3 position = hit.transform.position;
                    int x = (int)position.x/2;
                    int z = (int)position.z/2;
                    if (selectedPiece == null) SelectPiece(x,z);
                    else MovePiece(x,z);
                }
            }
        }
        #endregion
    }

    void MovePiece(int x, int z) {
        if (allowedMoves[x,z]) {
            Schachfigur enemy = Schachfiguren[x,z];
            // wenn Figur auf Gegner stoßen wird
            if (enemy != null && enemy.isWhite != isWhiteTurn) {
                // Gegner entfernen
                schachfiguren.Remove(enemy.gameObject);
                Destroy(enemy.gameObject);
                // wenn Gegner König ist, ist das Spiel vorbei
                if (enemy.Title == "König") {
                    print(isWhiteTurn ? "Schwarz" : "Weiß" + " ist Schachmatt");
                    return;
                }
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

            #region Schachmatt
            foreach (var item in schachfiguren) {
                Schachfigur figur = item.GetComponent<Schachfigur>();
                // nur mögliche Bewegungen der Figuren anschauen, deren Besitzer am Zug ist
                if (isWhiteTurn == figur.isWhite) {
                    bool[,] moves = figur.PossibleMovements();
                    for (int i = 0; i < 8; i++) {
                        for (int j = 0; j < 8; j++) {
                            // wenn an einer der möglichen Stellen ein König der anderen Farbe steht
                            // Schachfiguren[i,j].isWhite != isWhiteTurn --> unnötig, weil Figur gar nicht auf Feld könnte, wo Figur der selben Farbe steht
                            if (moves[i,j] && Schachfiguren[i,j] && Schachfiguren[i,j].isWhite != isWhiteTurn) {
                                print("Gegnerische/r " + Schachfiguren[i,j]);
                                if (Schachfiguren[i,j].Title == "König") {
                                    schach = true;
                                    print(isWhiteTurn ? "Schwarz" : "Weiß" + " steht im Schach wegen " + figur);
                                    // 1. kann das Schach geblockt werden, indem sich eine eigene Figur zwischen den König und die gegnerische Figur stellt?
                                    schach = canNotBlock(figur, Schachfiguren[i,j]);
                                    // 2. kann die gegnerische Figur, wegen der der König im Schach steht, geschlagen werden?
                                    if (schach) schach = canNotKill(figur, Schachfiguren[i,j]);
                                    // 3. kann der König entkommen, indem er auf eines seiner Nachbarfelder zieht?
                                    if (schach) schach = canNotMove(Schachfiguren[i,j]);
                                    if (schach) {
                                        print("Schachmatt!");
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            // Wechsel
            isWhiteTurn = !isWhiteTurn;
            changePerspective = true;
            CameraController.Instance.xAligned = false;
            CameraController.Instance.yAligned = false;
        }
        // Auswahl aufheben
        selectedPiece.GetComponent<MeshRenderer>().material = previousMat;
        selectedPiece = null;
        CreateChessBoard();
    }

    bool canNotBlock(Schachfigur angreifer, Schachfigur koenig) {
        whiteMoves = new bool[8,8];
        blackMoves = new bool[8,8];
        foreach (var item in schachfiguren) {
            Schachfigur figur = item.GetComponent<Schachfigur>();
            if (figur.Title != "König") {
                bool[,] moves = figur.PossibleMovements();
                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        if (moves[i,j]) {
                            if (figur.isWhite) whiteMoves[i,j] = true;
                            else blackMoves[i,j] = true;
                        }        
                    }
                }
            }
        }

        List<Feld> weg = new List<Feld>();
        int ax = angreifer.X;
        int az = angreifer.Z;
        int distance;
        int distanceOnX = Mathf.Abs(angreifer.X - koenig.X);
        int distanceOnZ = Mathf.Abs(angreifer.Z - koenig.Z);
        if (distanceOnX == 0) distance = distanceOnZ;
        else distance = distanceOnX;

        for (int i = 1; i < distance; i++) {
                 if (angreifer.X  > koenig.X && angreifer.Z  > koenig.Z) weg.Add(new Feld(--ax, --az));
            else if (angreifer.X  > koenig.X && angreifer.Z  < koenig.Z) weg.Add(new Feld(--ax, ++az));
            else if (angreifer.X  < koenig.X && angreifer.Z  < koenig.Z) weg.Add(new Feld(++ax, ++az));
            else if (angreifer.X  < koenig.X && angreifer.Z  > koenig.Z) weg.Add(new Feld(++ax, --az));
            else if (angreifer.X  > koenig.X && angreifer.Z == koenig.Z) weg.Add(new Feld(--ax, az));
            else if (angreifer.X == koenig.X && angreifer.Z  < koenig.Z) weg.Add(new Feld(ax, ++az));
            else if (angreifer.X  < koenig.X && angreifer.Z == koenig.Z) weg.Add(new Feld(++ax, az));
            else if (angreifer.X == koenig.X && angreifer.Z  > koenig.Z) weg.Add(new Feld(ax, --az));
        }
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                foreach (Feld feld in weg) {
                    print(feld);
                    if (i == feld.x && j == feld.z) {
                        if (!isWhiteTurn && whiteMoves[i,j] || isWhiteTurn && blackMoves[i,j]) {
                            print(angreifer + " kann auf " + feld + " geblockt werden");
                            return false;
                        }
                    }
                }                
            }
        }
        return true;
    }

    bool canNotKill(Schachfigur angreifer, Schachfigur koenig) {
        bool[,] moves = koenig.PossibleMovements();
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (moves[i,j]) {
                    if (koenig.isWhite) whiteMoves[i,j] = true;
                    else blackMoves[i,j] = true;
                }            
            }
        }
        if (!isWhiteTurn) {
            if (whiteMoves[angreifer.X, angreifer.Z]) {
                print("Weiß kann den Angreifer killen");
                return false;
            }
        }
        else {
            if (blackMoves[angreifer.X, angreifer.Z]) {
                print("Schwarz kann den Angreifer killen");
                return false;
            }
        }
        return true;
    }

    bool canNotMove(Schachfigur koenig) {
        foreach (var item in schachfiguren) {
            Schachfigur figur = item.GetComponent<Schachfigur>();
            // diagonale Angriffsbewegung der Bauern
            if (figur.Title == "Bauer") {
                int x = figur.X;
                int z = figur.Z;
                if (figur.isWhite) {
                    // links vorne
                    if (x-1 >= 0 && z+1 <= 7) whiteMoves[figur.X-1, figur.Z+1] = true;
                    // rechts vorne
                    if (x+1 <= 7 && z+1 <= 7) whiteMoves[figur.X+1, figur.Z+1] = true;
                }
                else {
                    // links vorne
                    if (x+1 <= 7 && z-1 >= 0) blackMoves[figur.X+1, figur.Z-1] = true;
                    // rechts vorne
                    if (x-1 >= 0 && z-1 >= 0) blackMoves[figur.X-1, figur.Z-1] = true;
                }
            }
        }
        
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (isWhiteTurn && koenig.PossibleMovements()[i,j] && !whiteMoves[i,j] 
                || !isWhiteTurn && koenig.PossibleMovements()[i,j] && !blackMoves[i,j]) {
                    print(koenig + " kann ausweichen");
                    return false;
                } 
            }
        }
        return true;
    }

    void fillEnemyMoves(bool whiteKing) {
        whiteMoves = new bool[8,8];
        blackMoves = new bool[8,8];
        foreach (var item in schachfiguren) {
            Schachfigur figur = item.GetComponent<Schachfigur>();
            if (figur.isWhite != whiteKing) {
               bool[,] moves = figur.PossibleMovements();
                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        if (moves[i,j]) {
                            if (figur.isWhite) whiteMoves[i,j] = true;
                            else blackMoves[i,j] = true;
                        }        
                    }
                }
                // diagonale Angriffsbewegung der Bauern
                if (figur.Title == "Bauer") {
                    int x = figur.X;
                    int z = figur.Z;
                    if (figur.isWhite) {
                        // links vorne
                        if (x-1 >= 0 && z+1 <= 7) whiteMoves[figur.X-1, figur.Z+1] = true;
                        // rechts vorne
                        if (x+1 <= 7 && z+1 <= 7) whiteMoves[figur.X+1, figur.Z+1] = true;
                    }
                    else {
                        // links vorne
                        if (x+1 <= 7 && z-1 >= 0) blackMoves[figur.X+1, figur.Z-1] = true;
                        // rechts vorne
                        if (x-1 >= 0 && z-1 >= 0) blackMoves[figur.X-1, figur.Z-1] = true;
                    }
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
            // Highlighting der erlaubten Züge
            fillEnemyMoves(selectedPiece.isWhite);
            allowedMoves = selectedPiece.PossibleMovements();
            HighlightAllowedMoves();
            //print(selectedPiece);
        }
    }

    void CreateChessBoard() {
        foreach (GameObject Feld in Felder) Destroy(Feld);
        Color color;
        for (int row = 0; row < 8; row++) {
            for (int column = 0; column < 8; column++) {
                if (row % 2 == 0 && column % 2 == 0 || row % 2 == 1 && column % 2 == 1) color = Color.black;
                else color = Color.white;
                //CreateSingleChessField(row, column, color);
                Felder[row,column] = CreateSquare(row, column, color);
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

    public GameObject CreateSquare(int row, int column, Color color) {
        GameObject feld = new GameObject();
        int number = row + 1;
        feld.name = columns[column] + number;
        feld.transform.parent = spielfeld.transform;
        feld.transform.position = new Vector3(row*2, 0, column*2);
        feld.tag = "Feld";
        CreateTriangle(row, column, color, true).transform.parent = feld.transform;
        CreateTriangle(row, column, color, false).transform.parent = feld.transform;
        feld.AddComponent<BoxCollider>();
        feld.GetComponent<BoxCollider>().isTrigger = true;
        feld.GetComponent<BoxCollider>().size = new Vector3(2,0,2);
        return feld;
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
        figur.tag = "Figur";
        schachfiguren.Add(figur);
        // Setzt Schachfiguren im imaginären Schachbrett auf Initialposition
        Schachfiguren[x,z] = figur.GetComponent<Schachfigur>();
        Schachfiguren[x,z].Move(x,z);
    }
    #endregion

    void HighlightAllowedMoves() {
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (allowedMoves[i,j]) {
                    Color prevColor;
                    if (i % 2 == 0 && j % 2 == 0 || i % 2 == 1 && j % 2 == 1) 
                    prevColor = Color.black;
                    else prevColor = Color.white;
                    Color highlightColor = Color.yellow;
                    if (Schachfiguren[i,j] && Schachfiguren[i,j].isWhite != isWhiteTurn)
                    highlightColor = Color.red;

                    Color color = Color.Lerp(prevColor, highlightColor, .5f);
                    Destroy(Felder[i,j]);
                    Felder[i,j] = CreateSquare(i,j,color);
                }
            }
        }
    }
}

public class Feld {
    public int x;
    public int z;

    public Feld(int x, int z) {
        this.x = x;
        this.z = z;
    }

    public override string ToString() {
        string[] columns = {"A","B","C","D","E","F","G","H"};
        string spalte = columns[x];
        int reihe = z+1;
        return spalte + reihe;
  }
}