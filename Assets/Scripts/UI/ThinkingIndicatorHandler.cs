/* *****************************************************************************
 * File: ThinkingIndicatorHandler.cs
 * 
 * Author: Cory Travis
 * 
 * Date: 9/20/2015
 * 
 * Description: Controls the animated thinking indicator visible while the AI
 *              is determining its next move.
 * 
 * License: GPL v3 - https://www.gnu.org/licenses/gpl-3.0.txt
 * 
 * Copyright 2015 Cory Travis 
 * ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ThinkingIndicatorHandler : MonoBehaviour {

    private const float DEGREES_PER_SECOND = 180.0f;

    private const float ALPHA_PER_SECOND = 1.0f / 3.0f;

    private Vector3 originalRotation;

    private Image image;

    private Color originalColor;

    void Start( ) {
        originalRotation = transform.rotation.eulerAngles;
        image = gameObject.GetComponent<Image>( );
        originalColor = image.color;
    }

    void Update( ) {

        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = rotation.z - DEGREES_PER_SECOND * Time.deltaTime;
        transform.eulerAngles = rotation;

        if( image.color.a < Color.white.a ) {

            float   r   =   image.color.r,
                    g   =   image.color.g,
                    b   =   image.color.b,
                    a   =   image.color.a + ALPHA_PER_SECOND * Time.deltaTime;

            image.color = new Color( r, g, b, a );

        }
    }

    public void Reset( ) {
        transform.eulerAngles = originalRotation;
        image.color = originalColor;
    }

}
