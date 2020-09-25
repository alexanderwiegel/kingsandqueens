using UnityEngine;
abstract class Schachfigur : MonoBehaviour {
  #region fields and properties
  // Farbe (weiß oder schwarz).
  public bool isWhite = false;

  // Name der Figur. Kann nur gelesen, nicht verändert werden.
  public abstract string Title {get;}

  // Position auf x-Achse (zwischen -7 und 7, steht für A-H).
  public int X {get; private set;}
  
  // Position auf z-Achse (zwischen -7 und 7, steht für 1-8).
  public int Z {get; private set;}

  // Für bessere Beschriftung
  string[] columns = {"A","B","C","D","E","F","G","H"};
  #endregion

  #region methods
  /* Zeigt die möglichen Bewegungen einer Schachfigur.
  *  Wird aufgerufen, sobald eine eigene Schachfigur ausgewählt wurde.
  */
  public abstract void ShowPossibleMovements();

  /* Individuelle Bewegung einer Schachfigur.
  *  Muss von jeder Figur überschrieben werden.
  */
  public void Move(int x, int z) {
    X = x;
    Z = z;
  }

  /* Überschriebene ToString()-Methode.
  *  Gibt Name, Nummer, Farbe und Position einer Schachfigur aus.
  */
  public override string ToString() {
    string color;
    if (isWhite) color = "weiß"; 
    else color = "schwarz";
    string spalte = columns[X];
    int reihe = Z+1;
    return Title + ", " + color + ", Position: " + spalte + reihe;
  }
  #endregion
}