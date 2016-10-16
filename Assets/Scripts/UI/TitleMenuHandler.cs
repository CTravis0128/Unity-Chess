/* ***************************************************************************** 
 * File: TitleMenuHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Controls the flow of input on the title menu.
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

public class TitleMenuHandler : MonoBehaviour {

    private static UnityObject boardResource;

    private static UnityObject fileDialogResource;

    private static UnityObject loadFailedDialogResource;

    private GameObject loadDialog;

    private GameObject loadFailedDialog;

    private FileDialogHandler loadDialogHandler;

    private CameraHandler cameraHandler;

    private UIHandler uiHandler;

    public GameObject newGameMenu;

    public GameObject titleMenuOverlay;

    void Start( ) {

        if( boardResource == null ) {
            boardResource = Resources.Load( "Board", typeof( GameObject ) );
        }

        if( fileDialogResource == null ) {
            fileDialogResource = Resources.Load( "File Dialog Panel", typeof( GameObject ) );
        }

        if( loadFailedDialogResource == null ) {
            loadFailedDialogResource = Resources.Load( "Load Failed Dialog Panel", typeof( GameObject ) );
        }

        cameraHandler = GameObject.Find( "Camera" ).GetComponent<CameraHandler>( );
        uiHandler = GameObject.Find( "UI" ).GetComponent<UIHandler>( );

    }

    public void NewGame( ) {
        newGameMenu.SetActive( true );
        gameObject.SetActive( false );
    }

    public void LoadGame( ) {
        ShowLoadDialog( );
        gameObject.SetActive( false );
    }

    public void NewGameButtonClicked( ) {
        NewGame( );
    }

    public void LoadGameButtonClicked( ) {
        LoadGame( );
    }

    public void QuitButtonClicked( ) {
        Application.Quit( );
    }

    private void ShowLoadDialog( ) {

        loadDialog = ( GameObject ) Instantiate( fileDialogResource );
        loadDialog.name = "Load Dialog Panel";
        loadDialog.transform.SetParent( uiHandler.transform, false );

        loadDialogHandler = loadDialog.GetComponent<FileDialogHandler>( );

        loadDialogHandler.Configure( FileDialogHandler.Configuration.Load,
                                     LoadDialogConfirmButtonClickedHandler,
                                     LoadDialogCancelButtonClickedHandler );

        gameObject.SetActive( false );
        loadDialog.SetActive( true );

    }

    private void ShowLoadFailedDialog( ) {

        loadFailedDialog = ( GameObject ) Instantiate( loadFailedDialogResource );
        loadFailedDialog.name = "Load Failed Dialog Panel";
        loadFailedDialog.transform.SetParent( uiHandler.transform, false );

        Button okButton = loadFailedDialog.transform.FindChild( "OK Button" ).GetComponent<Button>( );
        okButton.onClick.AddListener( LoadFailedDialogOKButtonClickedHandler );

        loadDialog.SetActive( false );
        loadFailedDialog.SetActive( true );

    }

    private bool LoadGame( FileInfo file ) {

        bool success = false;

        try {
            success = Game.Load( file.FullName );
        } catch {
            success = false;
        }

        if( success ) {

            GameObject board = ( GameObject ) Instantiate( boardResource );
            board.name = "Board";
            board.SetActive( true );

            board.GetComponent<BoardHandler>( ).LoadGame( );

            if( Game.PlayerWhite.Intellegence == Player.PlayerIntellegenceNames.Human ) {
                cameraHandler.ShowWhite( );
            } else /* Game.PlayerBlack.Intelligence == Player.PlayerIntellegenceNames.Human */ {
                cameraHandler.ShowBlack( );
            }

        }

        return success;

    }

    private void DestroyLoadDialog( ) {
        Destroy( loadDialog );
        loadDialog = null;
        loadDialogHandler = null;
    }

    private void LoadDialogConfirmButtonClickedHandler( ) {

        FileInfo file = loadDialogHandler.GetSelectedFile( );

        if( file != null ) {
            if( LoadGame( file ) ) {
                DestroyLoadDialog( );
                titleMenuOverlay.SetActive( false );
            } else {
                ShowLoadFailedDialog( );
            }
        }

    }

    private void LoadDialogCancelButtonClickedHandler( ) {
        DestroyLoadDialog( );
        gameObject.SetActive( true );
    }

    private void LoadFailedDialogOKButtonClickedHandler( ) {
        Destroy( loadFailedDialog );
        loadFailedDialog = null;
        loadDialog.SetActive( true );
        gameObject.SetActive( true );
    }

}
