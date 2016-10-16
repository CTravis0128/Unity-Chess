/* ***************************************************************************** 
 * File: MainMenuHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for the in-game main menu. Functions include saving,
 *              loading, and creating new games.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using SharpChess.Model;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using UnityObject = UnityEngine.Object;

public class MainMenuHandler : MonoBehaviour {

    private static UnityObject askSaveDialogResource;

    private static UnityObject fileDialogResource;

    private static UnityObject saveFailedDialogResource;

    private static UnityObject invalidFilenameDialogResource;

    private static UnityObject invalidDrawClaimDialogResource;

    private GameObject askSaveDialog;

    private GameObject saveDialog;

    private GameObject saveFailedDialog;

    private GameObject invalidFilenameDialog;

    private GameObject invalidDrawClaimDialog;

    private UIHandler uiHandler;

    private TitleMenuHandler titleMenuHandler;

    private FileDialogHandler saveDialogHandler;

    private bool quit;

    private bool newGame;

    private bool loadGame;

    public GameObject ui;

    public GameObject titleMenu;

    public void ClaimDrawButtonClicked( ) {

        string condition = null;

        /*
         * technically, there are edge cases where these can overlap. however, the outcome is effectively the same. 
         */

        if( Game.PlayerToPlay.CanClaimInsufficientMaterialDraw ) {
            condition = BoardHandler.GameOverConditions.DRAW_BY_INSUFFICIENT_MATERIAL;
        } else if( Game.PlayerToPlay.CanClaimThreeMoveRepetitionDraw ) {
            condition = BoardHandler.GameOverConditions.DRAW_BY_REPETITION;
        }

        if( condition != null ) {

            BoardHandler boardHandler = GameObject.Find( "Board" ).GetComponent<BoardHandler>( );
            boardHandler.ClaimDraw( condition );

            Resume( );

        } else {
            ShowInvalidDrawClaimDialog( );
        }

    }

    public void NewGameButtonClicked( ) {

        BoardHandler boardHandler = GameObject.Find( "Board" ).GetComponent<BoardHandler>( );

        if( boardHandler.UnsavedProgress ) {
            newGame = true;
            ShowAskSaveDialog( );
            gameObject.SetActive( false );
        } else {
            NewGame( );
        }

    }

    public void LoadGameButtonClicked( ) {

        BoardHandler boardHandler = GameObject.Find( "Board" ).GetComponent<BoardHandler>( );

        if( boardHandler.UnsavedProgress ) {
            loadGame = true;
            ShowAskSaveDialog( );
            gameObject.SetActive( false );
        } else {
            LoadGame( );
        }

    }

    public void SaveGameButtonClicked( ) {
        ShowSaveDialog( );
    }

    public void QuitToTitleButtonClicked( ) {

        BoardHandler boardHandler = GameObject.Find( "Board" ).GetComponent<BoardHandler>( );

        if( boardHandler.UnsavedProgress ) {
            quit = true;
            ShowAskSaveDialog( );
            gameObject.SetActive( false );
        } else {
            QuitToTitle( );
        }
    }

    void Start( ) {

        uiHandler = ui.GetComponent<UIHandler>( );
        titleMenuHandler = titleMenu.GetComponent<TitleMenuHandler>( );

        if( askSaveDialogResource == null ) {
            askSaveDialogResource = Resources.Load( "Ask Save Dialog Panel", typeof( GameObject ) );
        }

        if( fileDialogResource == null ) {
            fileDialogResource = Resources.Load( "File Dialog Panel", typeof( GameObject ) );
        }

        if( saveFailedDialogResource == null ) {
            saveFailedDialogResource = Resources.Load( "Save Failed Dialog Panel", typeof( GameObject ) );
        }

        if( invalidFilenameDialogResource == null ) {
            invalidFilenameDialogResource = Resources.Load( "Invalid Filename Dialog Panel", typeof( GameObject ) );
        }

        if( invalidDrawClaimDialogResource == null ) {
            invalidDrawClaimDialogResource = Resources.Load( "Invalid Draw Claim Dialog Panel", typeof( GameObject ) );
        }

    }

    void Update( ) {
        if( Input.GetKeyDown( KeyCode.Escape ) ) {
            Resume( );
        }
    }

    private void Resume( ) {
        uiHandler.mainMenuOverlay.SetActive( false );
        Game.ResumePlay( );
        UIHandler.Busy = false;
    }

    private void ShowAskSaveDialog( ) {

        askSaveDialog = ( GameObject ) Instantiate( askSaveDialogResource );
        askSaveDialog.name = "Ask Save Dialog Panel"; 
        askSaveDialog.transform.SetParent( uiHandler.transform, false );

        Button saveButton = askSaveDialog.transform.FindChild( "Save Button" ).GetComponent<Button>( );
        Button dontSaveButton = askSaveDialog.transform.FindChild( "Don't Save Button" ).GetComponent<Button>( );
        Button cancelButton = askSaveDialog.transform.FindChild( "Cancel Button" ).GetComponent<Button>( );

        saveButton.onClick.AddListener( AskSaveSaveButtonClickedHandler );
        dontSaveButton.onClick.AddListener( AskSaveDontSaveButtonClickedHandler );
        cancelButton.onClick.AddListener( AskSaveCancelButtonClickedHandler );

        askSaveDialog.SetActive( true );

    }

    private void ShowSaveDialog( ) {

        saveDialog = ( GameObject ) Instantiate( fileDialogResource );
        saveDialog.name = "Save Dialog Panel"; 
        saveDialog.transform.SetParent( uiHandler.transform, false );

        saveDialogHandler = saveDialog.GetComponent<FileDialogHandler>( );

        saveDialogHandler.Configure( FileDialogHandler.Configuration.Save,
                                     SaveDialogConfirmButtonClickedHandler,
                                     SaveDialogCancelButtonClickedHandler );

        gameObject.SetActive( false );
        saveDialog.SetActive( true );

    }

    private void ShowSaveFailedDialog( ) {

        saveFailedDialog = ( GameObject ) Instantiate( saveFailedDialogResource );
        saveFailedDialog.name = "Save Failed Dialog Panel"; 
        saveFailedDialog.transform.SetParent( uiHandler.transform, false );

        Button okButton = saveFailedDialog.transform.FindChild( "OK Button" ).GetComponent<Button>( );
        okButton.onClick.AddListener( SaveFailedDialogOKButtonClickedHandler );

        saveDialog.SetActive( false );
        gameObject.SetActive( false );
        saveFailedDialog.SetActive( true );

    }

    private void ShowInvalidFilenameDialog( ) {

        invalidFilenameDialog = ( GameObject ) Instantiate( invalidFilenameDialogResource );
        invalidFilenameDialog.name = "Invalid Filename Dialog Panel"; 
        invalidFilenameDialog.transform.SetParent( uiHandler.transform, false );

        Button okButton = invalidFilenameDialog.transform.FindChild( "OK Button" ).GetComponent<Button>( );
        okButton.onClick.AddListener( InvalidFileDialogOKButtonClickedHandler );

        saveDialog.SetActive( false );
        gameObject.SetActive( false );
        invalidFilenameDialog.SetActive( true );

    }

    private void ShowInvalidDrawClaimDialog( ) {

        invalidDrawClaimDialog = ( GameObject ) Instantiate( invalidDrawClaimDialogResource );
        invalidDrawClaimDialog.name = "Invalid Draw Claim Dialog Panel"; 
        invalidDrawClaimDialog.transform.SetParent( uiHandler.transform, false );

        Button okButton = invalidDrawClaimDialog.transform.FindChild( "OK Button" ).GetComponent<Button>( );
        okButton.onClick.AddListener( InvalidDrawClaimDialogOKButtonClickedHandler );

        gameObject.SetActive( false );
        invalidDrawClaimDialog.SetActive( true );
    }

    private void NewGame( ) {
        QuitToTitle( );
        titleMenuHandler.NewGame( );
    }

    private void LoadGame( ) {
        QuitToTitle( );
        titleMenuHandler.LoadGame( );
    }

    private void QuitToTitle( ) {

        Destroy( GameObject.Find( "Board" ) );

        uiHandler.mainMenuOverlay.SetActive( false );
        uiHandler.titleScreenOverlay.SetActive( true );
        uiHandler.titleMenuPanel.SetActive( true );

        if( uiHandler.winTypeText.activeInHierarchy ) {
            uiHandler.winTypeText.SetActive( false );
        }

        if( uiHandler.winConditionText.activeInHierarchy ) {
            uiHandler.winConditionText.SetActive( false );
        }

        newGame = false;
        loadGame = false;
        quit = false;

        UIHandler.Busy = false;
    }

    private void NewLoadQuit( ) {
        if( newGame ) {
            NewGame( );
            newGame = false;
        } else if( loadGame ) {
            LoadGame( );
            loadGame = false;
        } else if( quit ) {
            QuitToTitle( );
            quit = false;
        } else {
            gameObject.SetActive( true );
        }
    }

    private void SaveDialogConfirmButtonClickedHandler( ) {

        FileInfo saveFile = saveDialogHandler.GetSelectedFile( );

        if( saveFile != null ) {

            try {
                Game.Save( saveFile.FullName );
            } catch {
                ShowSaveFailedDialog( );
                return;
            }


            Destroy( saveDialog );
            saveDialog = null;

            NewLoadQuit( );

        } else {
            ShowInvalidFilenameDialog( );
        }

    }

    private void SaveDialogCancelButtonClickedHandler( ) {
        Destroy( saveDialog );
        saveDialog = null;
        newGame = false;
        loadGame = false;
        quit = false;
        gameObject.SetActive( true );
    }

    private void SaveFailedDialogOKButtonClickedHandler( ) {
        Destroy( saveFailedDialog );
        saveFailedDialog = null;
        saveDialog.SetActive( true );
    }

    private void AskSaveSaveButtonClickedHandler( ) {
        Destroy( askSaveDialog );
        askSaveDialog = null;
        ShowSaveDialog( );
    }

    private void AskSaveDontSaveButtonClickedHandler( ) {
        Destroy( askSaveDialog );
        askSaveDialog = null;
        NewLoadQuit( );
    }

    private void AskSaveCancelButtonClickedHandler( ) {
        Destroy( askSaveDialog );
        askSaveDialog = null;
        newGame = false;
        loadGame = false;
        quit = false;
        gameObject.SetActive( true );
    }

    private void InvalidFileDialogOKButtonClickedHandler( ) {
        Destroy( invalidFilenameDialog );
        invalidFilenameDialog = null;
        saveDialog.SetActive( true );
    }

    private void InvalidDrawClaimDialogOKButtonClickedHandler( ) {
        Destroy( invalidDrawClaimDialog );
        invalidDrawClaimDialog = null;
        gameObject.SetActive( true );
    }

}
