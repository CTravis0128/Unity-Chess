/* *****************************************************************************
 * File: UIHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler for the UI object, which acts as a container for all 
 *              screenspace UI elements.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using SharpChess.Model;
using System;
using System.IO;
using UnityEngine;

public class UIHandler : MonoBehaviour {

    private const string BACKUP_FILENAME = "backup";

    public GameObject titleScreenOverlay;

    public GameObject titleMenuPanel;

    public GameObject mainMenuOverlay;

    public GameObject mainMenuPanel;

    public GameObject winTypeText;

    public GameObject winConditionText;

    /* 
     * Input related functions should check for this before proceeding.
     * The menu should not interrupt pawn promotions and vice versa. 
     */
    public static bool Busy { get; set; }


    /* 
     * There might be a better place for this, but I can't think of it. The UI object is as close to a global root 
     * object as it gets, and multiple objects will need access to the Game class, by which point this configuration
     * needs to have been done.
     */
    static UIHandler( ) {

        Game.BackupGamePath = Path.Combine( Environment.CurrentDirectory, BACKUP_FILENAME );
        Game.ClockMaxMoves = 1;
        Game.UseRandomOpeningMoves = true;

        // SharpChess assumes you will use all of these, leading to NullReferenceExceptions 
        Game.PlayerWhite.Brain.MoveConsideredEvent += BoardHandler.NoHandler;
        Game.PlayerBlack.Brain.MoveConsideredEvent += BoardHandler.NoHandler;
        Game.PlayerWhite.Brain.ThinkingBeginningEvent += BoardHandler.NoHandler;
        Game.PlayerBlack.Brain.ThinkingBeginningEvent += BoardHandler.NoHandler;
        Game.BoardPositionChanged += BoardHandler.NoHandler;
        Game.GamePaused += BoardHandler.NoHandler;
        Game.GameResumed += BoardHandler.NoHandler;
        Game.GameSaved += BoardHandler.NoHandler;
        Game.SettingsUpdated += BoardHandler.NoHandler;

    }

}
