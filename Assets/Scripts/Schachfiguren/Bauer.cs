class Bauer : Schachfigur {
    public override string Title {get{return "Bauer";}}

    public override bool[,] PossibleMovements() {
        Schachfigur other;

        bool[,] moves = new bool[8,8];
        
        #region weiß
        if (isWhite) {
            #region diagonal left
            // wenn nicht am Rande des Spielfelds
            if (X != 0 && Z != 7) {
                other = GameState.Instance.Schachfiguren[X-1, Z+1];
                // wenn dort Gegner steht
                if (other != null && !other.isWhite) moves[X-1,Z+1] = true;
            }
            #endregion

            #region diagonal rechts
            // wenn nicht am Rande des Spielfelds
            if (X != 7 && Z != 7) {
                other = GameState.Instance.Schachfiguren[X+1, Z+1];
                // wenn dort Gegner steht
                if (other != null && !other.isWhite) moves[X+1,Z+1] = true;
            }
            #endregion

            #region vorne
            // wenn nicht am Rande des Spielfelds
            if (Z != 7) {
                other = GameState.Instance.Schachfiguren[X, Z+1];
                // wenn dort keine Figur steht
                if (other == null) moves[X,Z+1] = true;
            }
            #endregion

            #region vorne am Anfang
            // wenn nicht am Rande des Spielfelds
            if (Z == 1) {
                other = GameState.Instance.Schachfiguren[X, Z+1];
                Schachfigur other2 = GameState.Instance.Schachfiguren[X, Z+2];
                // wenn dort keine Figuren stehen
                if (other == null && other2 == null) moves[X,Z+2] = true;
            }
            #endregion
        }
        #endregion

        #region schwarz
        else {
            #region diagonal left
            // wenn nicht am Rande des Spielfelds
            if (X != 0 && Z != 0) {
                other = GameState.Instance.Schachfiguren[X-1, Z-1];
                // wenn dort Gegner steht
                if (other != null && other.isWhite) moves[X-1,Z-1] = true;
            }
            #endregion

            #region diagonal rechts
            // wenn nicht am Rande des Spielfelds
            if (X != 7 && Z != 0) {
                other = GameState.Instance.Schachfiguren[X+1, Z-1];
                // wenn dort Gegner steht
                if (other != null && other.isWhite) moves[X+1,Z-1] = true;
            }
            #endregion

            #region vorne
            // wenn nicht am Rande des Spielfelds
            if (Z != 0) {
                other = GameState.Instance.Schachfiguren[X, Z-1];
                // wenn dort keine Figur steht
                if (other == null) moves[X,Z-1] = true;
            }
            #endregion

            #region vorne am Anfang
            // wenn nicht am Rande des Spielfelds
            if (Z == 6) {
                other = GameState.Instance.Schachfiguren[X, Z-1];
                Schachfigur other2 = GameState.Instance.Schachfiguren[X, Z-2];
                // wenn dort keine Figuren stehen
                if (other == null && other2 == null) moves[X,Z-2] = true;
            }
            #endregion
        }
        #endregion

        return moves;
    }
}