/* *****************************************************************************
 * File: DifficultyTextHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Used to display the selected difficulty level on the new game
 *              menu.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class DifficultyTextHandler : MonoBehaviour {

    private string baseText;

    private Text text;

    public Slider difficultySlider;

    public void UpdateText( ) {
        text.text = baseText + difficultySlider.value.ToString( );
    }

    void Start( ) {
        text = GetComponent<Text>( );
        baseText = text.text;
        UpdateText( );
    }

}
