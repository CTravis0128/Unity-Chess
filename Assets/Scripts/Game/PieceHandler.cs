/* *****************************************************************************
 * File: PieceHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for the individual pieces on the chess board. Stores a
 *              reference to the square it is positioned on, and handles input.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using SharpChess.Model;
using UnityEngine;

public class PieceHandler : MonoBehaviour {

    private const float MOVE_HIGHLIGHT_ALPHA = 0.5f;

    private static readonly Vector3 highlightHeight = new Vector3( 0.0f, 5.0f, 0.0f );

    private static GameObject selected;

    private BoardHandler boardHandler;

    private Moves legalMoves = new Moves( );

    public static GameObject Selected {
        get {
            return selected;
        }
    }

    public GameObject Square { get; set; }

    public Player.PlayerColourNames Colour { get; private set; }

    public Moves LegalMoves {
        get {
            return legalMoves;
        }
    }

    // may be called by a square on click
    public void Select( ) {
        gameObject.transform.position += highlightHeight;
        selected = gameObject;
        HighlightSquares( );
    }

    // also may be called by a square on click
    public void Deselect( ) {
        gameObject.transform.position -= highlightHeight;
        selected = null;
        DehighlightSquares( );
    }

    // also may be called by a square on click
    public bool TryMoveTo( GameObject square ) {
        return boardHandler.TryMove( this, square );
    }

    void Start( ) {

        boardHandler = GameObject.Find( "Board" ).GetComponent<BoardHandler>( );

        if( gameObject.name.StartsWith( "White" ) ) {
            Colour = Player.PlayerColourNames.White;
        } else /* gameObject.name.StartsWith( "Black" ) */ {
            Colour = Player.PlayerColourNames.Black;
        }

    }

    void OnMouseDown( ) {

        if( UIHandler.Busy ) {
            return;
        }

        if( boardHandler.GameOver ) {
            return;
        }

        if( Game.PlayerToPlay.Intellegence == Player.PlayerIntellegenceNames.Computer ) {
            return;
        }

        if( Colour == Game.PlayerToPlay.Colour ) {

            if( selected != null ) {
                PieceHandler selectedHandler = selected.GetComponent<PieceHandler>( );
                selectedHandler.Deselect( );
            }

            Select( );

        } else if( selected != null ) {

            PieceHandler selectedHandler = selected.GetComponent<PieceHandler>( );

            if( selectedHandler.TryMoveTo( Square ) ) {
                selectedHandler.Deselect( );
            }

        }

    }

    private void HighlightSquares( ) {

        Square.GetComponent<SquareHandler>( ).Highlight( );

        Square square = Board.GetSquare( Square.name );
        square.Piece.GenerateLegalMoves( legalMoves );

        foreach( Move move in legalMoves ) {
            SquareHandler squareHandler = GameObject.Find( move.To.Name ).GetComponent<SquareHandler>( );
            if( !squareHandler.Highlighted ) /* possible overlap due to pawn promotion */ {
                squareHandler.Highlight( MOVE_HIGHLIGHT_ALPHA );
            }
        }

    }

    private void DehighlightSquares( ) {

        Square.GetComponent<SquareHandler>( ).DeHighlight( );

        foreach( Move move in legalMoves ) {
            SquareHandler squareHandler = GameObject.Find( move.To.Name ).GetComponent<SquareHandler>( );
            if( squareHandler.Highlighted ) /* possible overlap due to pawn promotion */ {
                squareHandler.DeHighlight( );
            }
        }

        legalMoves.Clear( );

    }

}