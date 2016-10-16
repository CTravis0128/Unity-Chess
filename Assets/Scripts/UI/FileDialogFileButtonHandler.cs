/* ***************************************************************************** 
 * File: FileDialogFileButtonHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handles storage of the selected file in the parent file dialog
 *              for later retrieval.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class FileDialogFileButtonHandler : MonoBehaviour {

    public InputField InputField { get; set; }

    public string FileName { get; set; }

    void Start( ) {
        gameObject.GetComponent<Button>( ).onClick.AddListener( OnClickHandler );
    }

    private void OnClickHandler( ) {
        InputField.text = FileName;
    }
}
