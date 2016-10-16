/* *****************************************************************************
 * File: FileDialogInputHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Simply fixes the placement of the caret in the input field
 *              of the file dialog. 
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using UnityEngine;

public class FileDialogInputHandler : MonoBehaviour {

    private bool caretFixed;

    void Update( ) {

        if( !caretFixed ) {

            /* 
             * not my proudest moment, but Unity caused this shit, not me 
             */

            Transform caretTransform = transform.FindChild( name + " Input Caret" );

            /* 
             * may our children forgive us
             */

            if( caretTransform != null ) {

                RectTransform caretRectTransform = caretTransform.GetComponent<RectTransform>( );

                caretRectTransform.offsetMin = new Vector2( caretRectTransform.offsetMin.x, -5.0f );
                caretRectTransform.offsetMax = new Vector2( caretRectTransform.offsetMax.x, 0.0f );

                /*
                 * why can't i just do this in the fucking editor
                 */

                caretFixed = true;

            }
        }
    }

}
