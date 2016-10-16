/* *****************************************************************************
 * File: BoardHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for the game board object. Queues and processes moves,
 *              and detects game over conditions.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using SharpChess.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

using UnityObject = UnityEngine.Object;

public class BoardHandler : MonoBehaviour {

    public static class GameOverConditions {

        public const string BLACK_WINS = "Black Wins";

        public const string CHECKMATE = "Checkmate";

        public const string DRAW = "Draw";

        public const string DRAW_BY_INSUFFICIENT_MATERIAL = "Insufficient Material";

        public const string DRAW_BY_REPETITION = "Repetition";

        public const string STALEMATE = "Stalemate";

        public const string WHITE_WINS = "White Wins";

    }

    [Serializable]
    public class InvalidPromotionException : Exception {

        private const string INVALID_PROMOTION_MESSAGE = "Invalid pawn promotion";

        public Move.MoveNames AttemptedPromotion { get; private set; }

        public InvalidPromotionException( Move.MoveNames attemptedPromotion ) : base( INVALID_PROMOTION_MESSAGE ) {
            AttemptedPromotion = attemptedPromotion;
        }
    }

    private static UnityObject thinkingIndicatorResource;

    private static UnityObject promotionSelectorResource;

    private Move moveToMake;

    private Move bishopPromotion;

    private Move knightPromotion;

    private Move queenPromotion;

    private Move rookPromotion;

    private bool moveToProcess;

    private AudioSource moveAudio;

    private Mesh bishopMesh;

    private Quaternion whiteBishopRotation;

    private Quaternion blackBishopRotation;

    private Mesh knightMesh;

    private Quaternion whiteKnightRotation;

    private Quaternion blackKnightRotation;

    private Mesh queenMesh;

    private Quaternion whiteQueenRotation;

    private Quaternion blackQueenRotation;

    private Mesh rookMesh;

    private Quaternion whiteRookRotation;

    private Quaternion blackRookRotation;

    private GameObject thinkingIndicator;

    private GameObject promotionSelector;

    private UIHandler uiHandler;

    private ThinkingIndicatorHandler thinkingIndicatorHandler;

    private Text winTypeText;

    private Text winConditionText;

    private bool unsavedProgress;

    private bool gameToLoad;

    private bool showThinkingIndicator;

    private bool hideThinkingIndicator;

    private bool gameOver;

    public bool UnsavedProgress {
        get {
            return unsavedProgress;
        }
    }

    public bool GameOver {
        get {
            return gameOver;
        }
    }

    public static void NoHandler( ) {
        // sharpchess assumes that all event handlers will be assigned to, hence the hack
    }

    public void LoadGame( ) {
        gameToLoad = true;
    }

    public bool TryMove( PieceHandler pieceHandler, GameObject destination ) {

        bool moveFound = false;
        bool promotionFound = false;

        Square squareTo = Board.GetSquare( destination.name );

        foreach( Move move in pieceHandler.LegalMoves ) {

            if( move.To == squareTo ) {

                moveFound = true;

                switch( move.Name ) {
                    case Move.MoveNames.PawnPromotionBishop:
                        bishopPromotion = move;
                        promotionFound = true;
                        break;
                    case Move.MoveNames.PawnPromotionKnight:
                        knightPromotion = move;
                        promotionFound = true;
                        break;
                    case Move.MoveNames.PawnPromotionQueen:
                        queenPromotion = move;
                        promotionFound = true;
                        break;
                    case Move.MoveNames.PawnPromotionRook:
                        rookPromotion = move;
                        promotionFound = true;
                        break;
                    default:
                        moveToMake = move;
                        break;
                }

            }

        }

        if( promotionFound ) {
            UIHandler.Busy = true;
            promotionSelector.SetActive( true );
        }

        return moveFound;
    }

    public void ClaimDraw( string condition ) {

        winTypeText.text = BoardHandler.GameOverConditions.DRAW;
        winConditionText.text = condition;

        winTypeText.gameObject.SetActive( true );
        winConditionText.gameObject.SetActive( true );

        gameOver = true;

    }

    void Start( ) {

        if( thinkingIndicatorResource == null ) {
            thinkingIndicatorResource = Resources.Load( "Thinking Indicator", typeof( GameObject ) );
        }

        if( promotionSelectorResource == null ) {
            promotionSelectorResource = Resources.Load( "Promotion Selector Overlay", typeof( GameObject ) );
        }

        moveAudio = gameObject.GetComponent<AudioSource>( );

        uiHandler = GameObject.Find( "UI" ).GetComponent<UIHandler>( );

        winTypeText = uiHandler.winTypeText.GetComponent<Text>( );
        winConditionText = uiHandler.winConditionText.GetComponent<Text>( );

        Game.BoardPositionChanged += BoardPositionChangedHandler;
        Game.BoardPositionChanged -= NoHandler;

        Game.GameSaved += GameSavedHandler;
        Game.GameSaved -= NoHandler;

        CreateThinkingIndicator( );
        CreatePromotionSelector( );
        SetupPawnPromotion( );

    }

    void Update( ) {

        if( !UIHandler.Busy ) {

            if( gameToLoad ) {
                for( int i = 0; i < Game.MoveHistory.Count; i++ ) {
                    ProcessMove( Game.MoveHistory[ i ] );
                }
                gameToLoad = false;
            }

            if( !gameOver ) {
                CheckGameOver( );
            }

            if( moveToMake != null ) {
                Game.MakeAMove( moveToMake.Name, moveToMake.Piece, moveToMake.To );
                moveToMake = null;
            }

            if( moveToProcess ) {
                ProcessMove( );
                moveAudio.Play( );
                moveToProcess = false;
            }

            if( showThinkingIndicator ) {
                ShowThinkingIndicator( );
                showThinkingIndicator = false;
            } else if( Input.GetKeyDown( KeyCode.Escape ) ) {
                uiHandler.mainMenuOverlay.SetActive( true );
                uiHandler.mainMenuPanel.SetActive( true );
                Game.PausePlay( );
                UIHandler.Busy = true;
            }

        } else if( hideThinkingIndicator ) {
            HideThinkingIndicator( );
            hideThinkingIndicator = false;
        }

    }

    void OnDestroy( ) {

        Game.BoardPositionChanged -= BoardPositionChangedHandler;
        Game.BoardPositionChanged += NoHandler;

        Game.GameSaved -= GameSavedHandler;
        Game.GameSaved += NoHandler;

        if( Game.PlayerWhite.Intellegence == Player.PlayerIntellegenceNames.Computer ) {
            Game.PlayerWhite.Brain.ThinkingBeginningEvent -= ComputerThinkingBeginningHandler;
            Game.PlayerWhite.Brain.ThinkingBeginningEvent += NoHandler;
        } else /* Game.PlayerBlack.Intellegence == Player.PlayerIntellegenceNames.Computer */ {
            Game.PlayerBlack.Brain.ThinkingBeginningEvent -= ComputerThinkingBeginningHandler;
            Game.PlayerBlack.Brain.ThinkingBeginningEvent += NoHandler;
        }

        Destroy( promotionSelector );
        Destroy( thinkingIndicator );

    }

    private void CreatePromotionSelector( ) {

        promotionSelector = ( GameObject ) Instantiate( promotionSelectorResource );
        promotionSelector.name = "Promotion Selector Overlay";
        promotionSelector.transform.SetParent( uiHandler.transform, false );

        Transform promotionSelectorPanelTransform = promotionSelector.transform.FindChild( "Promotion Selector Panel" );

        Button  bishopButton    =   promotionSelectorPanelTransform.FindChild( "Bishop Promotion Button" ).GetComponent<Button>( ),
                knightButton    =   promotionSelectorPanelTransform.FindChild( "Knight Promotion Button" ).GetComponent<Button>( ),
                queenButton     =   promotionSelectorPanelTransform.FindChild( "Queen Promotion Button" ).GetComponent<Button>( ),
                rookButton      =   promotionSelectorPanelTransform.FindChild( "Rook Promotion Button" ).GetComponent<Button>( );

        bishopButton.onClick.AddListener( PromoteToBishop );
        knightButton.onClick.AddListener( PromoteToKnight );
        queenButton.onClick.AddListener( PromoteToQueen );
        rookButton.onClick.AddListener( PromoteToRook );

    }

    private void CreateThinkingIndicator( ) {

        thinkingIndicator = ( GameObject ) Instantiate( thinkingIndicatorResource );
        thinkingIndicator.name = "Thinking Indicator";
        thinkingIndicator.transform.SetParent( uiHandler.transform, false );

        thinkingIndicatorHandler = thinkingIndicator.GetComponent<ThinkingIndicatorHandler>( );

        if( Game.PlayerWhite.Intellegence == Player.PlayerIntellegenceNames.Computer ) {
            Game.PlayerWhite.Brain.ThinkingBeginningEvent -= NoHandler;
            Game.PlayerWhite.Brain.ThinkingBeginningEvent += ComputerThinkingBeginningHandler;
        } else /* Game.PlayerBlack.Intellegence == Player.PlayerIntellegenceNames.Computer */ {
            Game.PlayerBlack.Brain.ThinkingBeginningEvent -= NoHandler;
            Game.PlayerBlack.Brain.ThinkingBeginningEvent += ComputerThinkingBeginningHandler;
        }

    }

    private void SetupPawnPromotion( ) {

        GameObject  whiteBishop =   GameObject.Find( "White Queens Bishop" ),
                    blackBishop =   GameObject.Find( "Black Queens Bishop" ),
                    whiteKnight =   GameObject.Find( "White Queens Knight" ),
                    blackKnight =   GameObject.Find( "Black Queens Knight" ),
                    whiteQueen  =   GameObject.Find( "White Queen" ),
                    blackQueen  =   GameObject.Find( "Black Queen" ),
                    whiteRook   =   GameObject.Find( "White Queens Rook" ),
                    blackRook   =   GameObject.Find( "Black Queens Rook" );

        bishopMesh = whiteBishop.GetComponent<MeshFilter>( ).mesh;
        whiteBishopRotation = whiteBishop.transform.rotation;
        blackBishopRotation = blackBishop.transform.rotation;

        knightMesh = whiteKnight.GetComponent<MeshFilter>( ).mesh;
        whiteKnightRotation = whiteKnight.transform.rotation;
        blackKnightRotation = blackKnight.transform.rotation;

        queenMesh = whiteQueen.GetComponent<MeshFilter>( ).mesh;
        whiteQueenRotation = whiteQueen.transform.rotation;
        blackQueenRotation = blackQueen.transform.rotation;

        rookMesh = whiteRook.GetComponent<MeshFilter>( ).mesh;
        whiteRookRotation = whiteRook.transform.rotation;
        blackRookRotation = blackRook.transform.rotation;

    }

    private void PromoteToBishop( ) {
        moveToMake = bishopPromotion;
        promotionSelector.SetActive( false );
        UIHandler.Busy = false;
    }

    private void PromoteToKnight( ) {
        moveToMake = knightPromotion;
        promotionSelector.SetActive( false );
        UIHandler.Busy = false;
    }

    private void PromoteToQueen( ) {
        moveToMake = queenPromotion;
        promotionSelector.SetActive( false );
        UIHandler.Busy = false;
    }

    private void PromoteToRook( ) {
        moveToMake = rookPromotion;
        promotionSelector.SetActive( false );
        UIHandler.Busy = false;
    }

    private void ProcessMove( Move move = null ) {

        move = move ?? Game.MoveHistory.Last;

        Square fromSquare = move.From;
        SquareHandler fromSquareHandler = GameObject.Find( fromSquare.Name ).GetComponent<SquareHandler>( );

        PieceHandler movedPieceHandler = fromSquareHandler.Piece.GetComponent<PieceHandler>( );

        Square toSquare = move.To;
        SquareHandler toSquareHandler = GameObject.Find( toSquare.Name ).GetComponent<SquareHandler>( );

        fromSquareHandler.Piece = null;

        if( toSquareHandler.Piece != null ) {
            GameObject.Destroy( toSquareHandler.Piece );
            toSquareHandler.Piece = null;
        }

        toSquareHandler.Piece = movedPieceHandler.gameObject;
        movedPieceHandler.Square = toSquareHandler.gameObject;

        movedPieceHandler.gameObject.transform.position = toSquareHandler.gameObject.transform.position;

        /* special cases */
        switch( move.Name ) {
            case Move.MoveNames.CastleQueenSide:
            case Move.MoveNames.CastleKingSide:
                ProcessCastle( move );
                break;
            case Move.MoveNames.PawnPromotionBishop:
            case Move.MoveNames.PawnPromotionKnight:
            case Move.MoveNames.PawnPromotionQueen:
            case Move.MoveNames.PawnPromotionRook:
                ProcessPromotion( move, movedPieceHandler.gameObject );
                break;
        }

    }

    private void ProcessCastle( Move move ) {

        PieceHandler rookHandler;
        SquareHandler toSquareHandler;

        if( move.Name == Move.MoveNames.CastleQueenSide ) {
            if( move.Piece.Player.Colour == Player.PlayerColourNames.White ) {
                rookHandler = GameObject.Find( "White Queens Rook" ).GetComponent<PieceHandler>( );
                toSquareHandler = GameObject.Find( "d1" ).GetComponent<SquareHandler>( );
            } else /* lastMove.Piece.Player.Colour == Player.PlayerColourNames.Black */ {
                rookHandler = GameObject.Find( "Black Queens Rook" ).GetComponent<PieceHandler>( );
                toSquareHandler = GameObject.Find( "d8" ).GetComponent<SquareHandler>( );
            }
        } else /* lastMove.Name == Move.MoveNames.CastleKingSide */ {
            if( move.Piece.Player.Colour == Player.PlayerColourNames.White ) {
                rookHandler = GameObject.Find( "White Kings Rook" ).GetComponent<PieceHandler>( );
                toSquareHandler = GameObject.Find( "f1" ).GetComponent<SquareHandler>( );
            } else /* lastMove.Piece.Player.Colour == Player.PlayerColourNames.Black */ {
                rookHandler = GameObject.Find( "Black Kings Rook" ).GetComponent<PieceHandler>( );
                toSquareHandler = GameObject.Find( "f8" ).GetComponent<SquareHandler>( );
            }
        }

        rookHandler.Square.GetComponent<SquareHandler>( ).Piece = null;
        rookHandler.Square = toSquareHandler.gameObject;
        toSquareHandler.Piece = rookHandler.gameObject;
        rookHandler.gameObject.transform.position = toSquareHandler.gameObject.transform.position;

    }

    private void ProcessPromotion( Move move, GameObject piece ) {

        Mesh mesh;
        Quaternion rotation;

        switch( move.Name ) {
            case Move.MoveNames.PawnPromotionBishop:
                mesh = bishopMesh;
                rotation = move.Piece.Player.Colour == Player.PlayerColourNames.White ? whiteBishopRotation : blackBishopRotation;
                break;
            case Move.MoveNames.PawnPromotionKnight:
                mesh = knightMesh;
                rotation = move.Piece.Player.Colour == Player.PlayerColourNames.White ? whiteKnightRotation : blackKnightRotation;
                break;
            case Move.MoveNames.PawnPromotionQueen:
                mesh = queenMesh;
                rotation = move.Piece.Player.Colour == Player.PlayerColourNames.White ? whiteQueenRotation : blackQueenRotation;
                break;
            case Move.MoveNames.PawnPromotionRook:
                mesh = rookMesh;
                rotation = move.Piece.Player.Colour == Player.PlayerColourNames.White ? whiteRookRotation : blackRookRotation;
                break;
            default:
                throw new InvalidPromotionException( move.Name );
        }

        piece.GetComponent<MeshFilter>( ).mesh = mesh;
        piece.GetComponent<MeshCollider>( ).sharedMesh = mesh;
        piece.transform.rotation = rotation;

    }

    private void CheckGameOver( ) {

        if( Game.PlayerToPlay.Status == Player.PlayerStatusNames.InCheckMate ) {

            if( Game.PlayerToPlay.OpposingPlayer.Colour == Player.PlayerColourNames.White ) {
                winTypeText.text = GameOverConditions.WHITE_WINS;
            } else /* Game.PlayerToPlay.OpposingPlayer.Colour == Player.PlayerColourNames.Black */ {
                winTypeText.text = GameOverConditions.BLACK_WINS;
            }

            winConditionText.text = GameOverConditions.CHECKMATE;

            gameOver = true;

        } else if( Game.PlayerToPlay.OpposingPlayer.Status == Player.PlayerStatusNames.InCheckMate ) {

            if( Game.PlayerToPlay.Colour == Player.PlayerColourNames.White ) {
                winTypeText.text = GameOverConditions.WHITE_WINS;
            } else /* Game.PlayerToPlayer.Colour == Player.PlayerColourNames.Black */ {
                winTypeText.text = GameOverConditions.BLACK_WINS;
            }

            winConditionText.text = GameOverConditions.CHECKMATE;

            gameOver = true;

        } else if( Game.PlayerToPlay.Status == Player.PlayerStatusNames.InStalemate ) {

            winTypeText.text = GameOverConditions.DRAW;
            winConditionText.text = GameOverConditions.STALEMATE;

            gameOver = true;

        }

        if( gameOver ) {
            winTypeText.gameObject.SetActive( true );
            winConditionText.gameObject.SetActive( true );
        }

    }

    private void ShowThinkingIndicator( ) {
        thinkingIndicator.SetActive( true );
        Game.BoardPositionChanged += ComputerThinkingEndingHandler;
        UIHandler.Busy = true;
    }

    private void HideThinkingIndicator( ) {
        thinkingIndicator.SetActive( false );
        thinkingIndicatorHandler.Reset( );
        Game.BoardPositionChanged -= ComputerThinkingEndingHandler;
        UIHandler.Busy = false;
    }

    private void BoardPositionChangedHandler( ) {
        moveToProcess = true;
        unsavedProgress = true;
    }

    private void GameSavedHandler( ) {
        unsavedProgress = false;
    }

    private void ComputerThinkingBeginningHandler( ) {
        showThinkingIndicator = true;
    }

    private void ComputerThinkingEndingHandler( ) {
        hideThinkingIndicator = true;
    }

}
