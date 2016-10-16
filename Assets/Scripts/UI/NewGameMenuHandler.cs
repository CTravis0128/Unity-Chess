/* *****************************************************************************
 * File: NewGameMenuHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for the new game menu, where the player can set the
 *              difficulty and choose a color to play as.
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

public class NewGameMenuHandler : MonoBehaviour {

    [Serializable]
    public class InvalidDifficultyException : Exception {

        private const string MESSAGE = "Invalid difficulty setting.";

        public readonly int Difficulty;

        public InvalidDifficultyException( int difficulty ) : base( MESSAGE ) {
            Difficulty = difficulty;
        }

    }

    private static UnityObject boardResource;

    private CameraHandler cameraHandler;

    private Slider difficultySlider;

    private Toggle playAsWhiteToggle;

    public GameObject titleMenuOverlay;

    public GameObject titleMenuPanel;

    public void StartNewGameButtonClicked( ) {

        SetupNewGame( );

        GameObject board = ( GameObject ) Instantiate( boardResource );
        board.name = "Board";
        board.SetActive( true );

        Reset( );

        gameObject.SetActive( false );
        titleMenuOverlay.SetActive( false );

        if( Game.PlayerWhite.Intellegence == Player.PlayerIntellegenceNames.Computer ) {
            Game.ResumePlay( );
        }

    }

    public void CancelButtonClicked( ) {

        Reset( );

        titleMenuPanel.SetActive( true );
        gameObject.SetActive( false );

    }

    void Start( ) {

        if( boardResource == null ) {
            boardResource = Resources.Load( "Board", typeof( GameObject ) );
        }

        cameraHandler = GameObject.Find( "Camera" ).GetComponent<CameraHandler>( );

        difficultySlider = transform.FindChild( "Difficulty Slider" ).GetComponent<Slider>( );
        playAsWhiteToggle = transform.FindChild( "Play As White Toggle" ).GetComponent<Toggle>( );

    }

    private void SetupNewGame( ) {

        if( playAsWhiteToggle.isOn ) {

            Game.PlayerWhite.Intellegence = Player.PlayerIntellegenceNames.Human;
            Game.PlayerBlack.Intellegence = Player.PlayerIntellegenceNames.Computer;

            cameraHandler.ShowWhite( );

        } else /* playAsBlackToggle.isOn */ {

            Game.PlayerWhite.Intellegence = Player.PlayerIntellegenceNames.Computer;
            Game.PlayerBlack.Intellegence = Player.PlayerIntellegenceNames.Human;

            cameraHandler.ShowBlack( );

        }

        SetDifficulty( );

        Game.New( );

    }

    private void SetDifficulty( ) {

        /* 
         * I would love to use SharpChess's built in 'Pondering' feature but the author made some design
         * decisions that I can't quite wrap my head around. Maybe in the future.
         */

        int difficulty = Convert.ToInt32( difficultySlider.value );

        Game.DifficultyLevel = difficulty;

        switch( difficulty ) {

            case 1:
                Game.MaximumSearchDepth = 1;
                Game.ClockTime = TimeSpan.FromMinutes( 1 );
                break;
            case 2:
                Game.MaximumSearchDepth = 2;
                Game.ClockTime = TimeSpan.FromMinutes( 1 );
                break;
            case 3:
                Game.MaximumSearchDepth = 3;
                Game.ClockTime = TimeSpan.FromMinutes( 1 );
                break;
            case 4:
                Game.MaximumSearchDepth = 4;
                Game.ClockTime = TimeSpan.FromMinutes( 2 );
                break;
            case 5:
                Game.MaximumSearchDepth = 5;
                Game.ClockTime = TimeSpan.FromMinutes( 2 );
                break;
            case 6:
                Game.MaximumSearchDepth = 6;
                Game.ClockTime = TimeSpan.FromMinutes( 2 );
                break;
            case 7:
                Game.MaximumSearchDepth = 7;
                Game.ClockTime = TimeSpan.FromMinutes( 3 );
                break;
            case 8:
                Game.MaximumSearchDepth = 8;
                Game.ClockTime = TimeSpan.FromMinutes( 4 );
                break;
            case 9:
                Game.MaximumSearchDepth = 16;
                Game.ClockTime = TimeSpan.FromMinutes( 5 );
                break;
            case 10:
                Game.MaximumSearchDepth = 32;
                Game.ClockTime = TimeSpan.FromMinutes( 10 );
                break;
            default:
                throw new InvalidDifficultyException( difficulty );

        }

    }

    private void Reset( ) {
        playAsWhiteToggle.isOn = true;
        difficultySlider.value = 1.0f;
    }

}
