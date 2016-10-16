/* *****************************************************************************
 * File: CameraHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Handler used to position the camera behind either players side
 *              on the chess board.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using UnityEngine;

public class CameraHandler : MonoBehaviour {

    private static readonly Vector3 whitePosition = new Vector3( 52.5f, 130.0f, -21.7f );

    private static readonly Vector3 whiteRotation = new Vector3( 62.30002f, 0.0f, 0.0f );

    private static readonly Vector3 blackPosition = new Vector3( 52.5f, 130.0f, 126.7f );

    private static readonly Vector3 blackRotation = new Vector3( 62.30002f, 180.0f, 0.0f );

    private bool state;

    public void Flip( ) {

        if( !state ) {
            gameObject.transform.position = blackPosition;
            gameObject.transform.rotation = Quaternion.Euler( blackRotation );
        } else {
            gameObject.transform.position = whitePosition;
            gameObject.transform.rotation = Quaternion.Euler( whiteRotation );
        }

        state = !state;
    }

    public void ShowWhite( ) {
        if( state ) {
            Flip( );
        }
    }

    public void ShowBlack( ) {
        if( !state ) {
            Flip( );
        }
    }

}
