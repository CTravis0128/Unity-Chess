/* *****************************************************************************
 * File: SquareHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for the individual squares on the chess board. Stores
 *              a reference to the piece positioned on it, and handles input.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using SharpChess.Model;
using UnityEngine;
using UnityEngine.UI;

using UnityObject = UnityEngine.Object;

public class SquareHandler : MonoBehaviour {

    private const float DEFAULT_ALPHA = 0.67f;

    private UnityObject highlightImageResource;

    private GameObject highlightCanvas;

    private Image highlightImage;

    private Color defaultColor;

    private BoardHandler boardHandler;

    // represents the piece that is positioned on the square (if any)
    public GameObject Piece { get; set; }

    public bool Highlighted { get; private set; }

    public void Highlight( float alpha ) {
        highlightImage.color = new Color( defaultColor.r, defaultColor.g, defaultColor.b, alpha );
        highlightImage.gameObject.SetActive( true );
        Highlighted = true;
    }

    public void Highlight( ) {
        highlightImage.color = defaultColor;
        highlightImage.gameObject.SetActive( true );
        Highlighted = true;
    }

    public void DeHighlight( ) {
        highlightImage.color = defaultColor;
        highlightImage.gameObject.SetActive( false );
        Highlighted = false;
    }

    void Start( ) {

        if( highlightImageResource == null ) {
            highlightImageResource = Resources.Load( "Square Highlight Image", typeof( GameObject ) );
        }

        highlightCanvas = GameObject.Find( "Highlight Canvas" );

        boardHandler = GameObject.Find( "Board" ).GetComponent<BoardHandler>( );

        CreateHighlightImage( );

    }

    void Awake( ) {

        switch( gameObject.name ) {

            #region White Pieces

            case "a1":
                Piece = GameObject.Find( "White Queens Rook" );
                break;
            case "b1":
                Piece = GameObject.Find( "White Queens Knight" );
                break;
            case "c1":
                Piece = GameObject.Find( "White Queens Bishop" );
                break;
            case "d1":
                Piece = GameObject.Find( "White Queen" );
                break;
            case "e1":
                Piece = GameObject.Find( "White King" );
                break;
            case "f1":
                Piece = GameObject.Find( "White Kings Bishop" );
                break;
            case "g1":
                Piece = GameObject.Find( "White Kings Knight" );
                break;
            case "h1":
                Piece = GameObject.Find( "White Kings Rook" );
                break;
            case "a2":
                Piece = GameObject.Find( "White Pawn A" );
                break;
            case "b2":
                Piece = GameObject.Find( "White Pawn B" );
                break;
            case "c2":
                Piece = GameObject.Find( "White Pawn C" );
                break;
            case "d2":
                Piece = GameObject.Find( "White Pawn D" );
                break;
            case "e2":
                Piece = GameObject.Find( "White Pawn E" );
                break;
            case "f2":
                Piece = GameObject.Find( "White Pawn F" );
                break;
            case "g2":
                Piece = GameObject.Find( "White Pawn G" );
                break;
            case "h2":
                Piece = GameObject.Find( "White Pawn H" );
                break;

            #endregion

            #region Black Pieces

            case "a8":
                Piece = GameObject.Find( "Black Queens Rook" );
                break;
            case "b8":
                Piece = GameObject.Find( "Black Queens Knight" );
                break;
            case "c8":
                Piece = GameObject.Find( "Black Queens Bishop" );
                break;
            case "d8":
                Piece = GameObject.Find( "Black Queen" );
                break;
            case "e8":
                Piece = GameObject.Find( "Black King" );
                break;
            case "f8":
                Piece = GameObject.Find( "Black Kings Bishop" );
                break;
            case "g8":
                Piece = GameObject.Find( "Black Kings Knight" );
                break;
            case "h8":
                Piece = GameObject.Find( "Black Kings Rook" );
                break;
            case "a7":
                Piece = GameObject.Find( "Black Pawn A" );
                break;
            case "b7":
                Piece = GameObject.Find( "Black Pawn B" );
                break;
            case "c7":
                Piece = GameObject.Find( "Black Pawn C" );
                break;
            case "d7":
                Piece = GameObject.Find( "Black Pawn D" );
                break;
            case "e7":
                Piece = GameObject.Find( "Black Pawn E" );
                break;
            case "f7":
                Piece = GameObject.Find( "Black Pawn F" );
                break;
            case "g7":
                Piece = GameObject.Find( "Black Pawn G" );
                break;
            case "h7":
                Piece = GameObject.Find( "Black Pawn H" );
                break;

            #endregion

        }

        if( Piece != null ) {
            Piece.GetComponent<PieceHandler>( ).Square = gameObject;
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

        if( PieceHandler.Selected != null ) {
            PieceHandler selectedHandler = PieceHandler.Selected.GetComponent<PieceHandler>( );
            if( selectedHandler.TryMoveTo( gameObject ) ) {
                selectedHandler.Deselect( );
            } else if( Piece != null ) {
                PieceHandler pieceHandler = Piece.GetComponent<PieceHandler>( );
                if( pieceHandler.Colour == Game.PlayerToPlay.Colour ) {
                    selectedHandler.Deselect( );
                    pieceHandler.Select( );
                }
            }
        } else if( Piece != null ) {
            PieceHandler pieceHandler = Piece.GetComponent<PieceHandler>( );
            if( pieceHandler.Colour == Game.PlayerToPlay.Colour ) {
                pieceHandler.Select( );
            }
        }

    }

    private void CreateHighlightImage( ) {

        GameObject highlightImageObject = ( GameObject ) Instantiate( highlightImageResource );
        highlightImageObject.name = gameObject.name + " Highlight Image";
        highlightImageObject.transform.SetParent( highlightCanvas.transform );

        float   positionX   =   transform.position.x,
                positionY   =   highlightImageObject.transform.position.y,
                positionZ   =   transform.position.z;

        highlightImageObject.transform.position = new Vector3( positionX, positionY, positionZ );

        highlightImage = highlightImageObject.GetComponent<Image>( );

        float   defaultR    =   highlightImage.color.r,
                defaultG    =   highlightImage.color.g,
                defaultB    =   highlightImage.color.b;

        defaultColor = new Color( defaultR, defaultG, defaultB, DEFAULT_ALPHA );

        highlightImage.color = defaultColor;

    }

}
