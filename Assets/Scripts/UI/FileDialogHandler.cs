/* *****************************************************************************
 * File: FileDialogHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for file dialogs that can be configured for saving
 *              or loading.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using UnityObject = UnityEngine.Object;

public class FileDialogHandler : MonoBehaviour {

    public enum Configuration {
        Save,
        Load
    }

    private class Save {

        public GameObject Button { get; private set; }

        public FileInfo File { get; private set; }

        public string Name { get; private set; }

        public Save( GameObject button, FileInfo file ) {
            Button = button;
            File = file;
            Name = Path.GetFileNameWithoutExtension( file.Name );
        }

    }

    private const string SAVE_DIRECTORY = "Saves";

    private const string SAVE_EXTENSION = ".SharpChess";

    private const string SAVE_PATTERN = "*" + SAVE_EXTENSION;

    private const string SAVE_FILE_MESSAGE = "Choose an existing save to overwrite or enter a valid filename.";

    private const string LOAD_FILE_MESSAGE = "Choose a save to load.";

    private static DirectoryInfo saveDirectory;

    private static UnityObject fileButtonResource;

    private GameObject listPanel;

    private Scrollbar scrollbar;

    private InputField inputField;

    private Text saveOrLoadText;

    private Button confirmButton;

    private Button cancelButton;

    private Configuration configuration;

    private UnityAction confirmButtonClickedHandler;

    private UnityAction cancelButtonClickedHandler;

    private Save[ ] saves;

    private float buttonHeight;

    private Vector3 buttonOrigin;

    private int activeButtons;

    static FileDialogHandler( ) {

        string savePath = Path.Combine( Environment.CurrentDirectory, SAVE_DIRECTORY );
        saveDirectory = new DirectoryInfo( savePath );

        if( !saveDirectory.Exists ) {
            saveDirectory.Create( );
        }

    }

    void Start( ) {

        if( fileButtonResource == null ) {
            fileButtonResource = Resources.Load( "File Dialog File Button", typeof( GameObject ) );
        }

        listPanel = transform.FindChild( "File Dialog List Panel" ).gameObject;
        scrollbar = listPanel.transform.FindChild( "File Dialog List Panel Scrollbar" ).GetComponent<Scrollbar>( );
        inputField = transform.FindChild( "File Dialog Input" ).GetComponent<InputField>( );
        saveOrLoadText = transform.FindChild( "File Dialog Save Or Load Text" ).GetComponent<Text>( );
        confirmButton = transform.FindChild( "File Dialog Confirm Button" ).GetComponent<Button>( );
        cancelButton = transform.FindChild( "File Dialog Cancel Button" ).GetComponent<Button>( );

        Configure( );
        ListSaves( );
        SetupScrollbar( );

    }

    public void Configure( Configuration configuration,
                           UnityAction confirmButtonClickedHandler,
                           UnityAction cancelButtonClickedHandler ) {

        this.configuration = configuration;
        this.confirmButtonClickedHandler = confirmButtonClickedHandler;
        this.cancelButtonClickedHandler = cancelButtonClickedHandler;

    }

    public FileInfo GetSelectedFile( ) {

        if( String.IsNullOrEmpty( inputField.text ) ) {
            return null;
        }

        if( saves != null ) {
            for( int i = 0; i < saves.Length; i++ ) {
                Save save = saves[ i ];
                if( inputField.text == save.Name ) {
                    return save.File;
                }
            }
        }

        string path = Path.GetFileName( inputField.text ); // eliminate potential shenanigans
        path = Path.Combine( saveDirectory.FullName, path + SAVE_EXTENSION );

        return new FileInfo( path );

    }

    private void Configure( ) {

        if( configuration == Configuration.Load ) {
            saveOrLoadText.text = LOAD_FILE_MESSAGE;
            inputField.gameObject.SetActive( false );
        } else /* configuration == Configuration.Save */ {
            saveOrLoadText.text = SAVE_FILE_MESSAGE;
            inputField.gameObject.SetActive( true );
        }

        confirmButton.onClick.AddListener( confirmButtonClickedHandler );
        cancelButton.onClick.AddListener( cancelButtonClickedHandler );
    }

    private void ListSaves( ) {

        FileInfo[ ] saveFiles = saveDirectory.GetFiles( SAVE_PATTERN );

        if( saveFiles != null && saveFiles.Length > 0 ) {

            saves = new Save[ saveFiles.Length ];

            Vector2 listPanelSize = listPanel.GetComponent<RectTransform>( ).sizeDelta;

            for( int i = 0; i < saveFiles.Length; i++ ) {

                GameObject button = ( GameObject ) Instantiate( fileButtonResource );

                Save save = new Save( button, saveFiles[ i ] );

                button.name = save.Name + " Save File Button";

                Text buttonText = button.transform.FindChild( "Text" ).GetComponent<Text>( );
                buttonText.text = save.Name;

                FileDialogFileButtonHandler handler = button.GetComponent<FileDialogFileButtonHandler>( );
                handler.InputField = inputField;
                handler.FileName = save.Name;

                button.transform.SetParent( listPanel.transform, false );

                if( buttonHeight == default( float ) ) {
                    buttonHeight = button.GetComponent<RectTransform>( ).sizeDelta.y;
                    buttonOrigin = button.transform.position;
                }

                button.transform.position = new Vector3( buttonOrigin.x,
                                                         buttonOrigin.y - buttonHeight * i,
                                                         buttonOrigin.z );

                saves[ i ] = save;

                // if the file isn't going to fit on the panel...
                if( button.transform.position.y - buttonHeight < listPanel.transform.position.y - listPanelSize.y ) {

                    // ...hide it. the scrollbar can handle the rest
                    button.SetActive( false );

                }

            }

        }

    }

    private void SetupScrollbar( ) {

        // if there are existing saves...
        if( saves != null && saves.Length > 0 ) {

            // ...and enough to require a scrollbar...
            for( int i = 0; i < saves.Length; i++ ) {
                if( saves[ i ].Button.activeInHierarchy ) {
                    activeButtons++;
                }
            }

            // ...set up the scrollbar to handle it, otherwise don't bother
            if( activeButtons < saves.Length ) {
                scrollbar.size = ( float ) activeButtons / saves.Length;
                scrollbar.numberOfSteps = saves.Length - activeButtons;
                scrollbar.onValueChanged.AddListener( ScrollbarValueChangedHandler );
            }

        }

    }

    private void ScrollbarValueChangedHandler( float value ) {

        int top = ( int ) Math.Floor( value * ( saves.Length - activeButtons ) );

        for( int i = 0, position = 0; i < saves.Length; i++ ) {

            Save save = saves[ i ];

            if( i >= top && i < top + activeButtons ) {

                save.Button.transform.position = new Vector3( buttonOrigin.x,
                                                              buttonOrigin.y - buttonHeight * position++,
                                                              buttonOrigin.z );

                save.Button.SetActive( true );

            } else {
                save.Button.SetActive( false );
            }

        }

    }

}
