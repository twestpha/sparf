using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContactComponent : MonoBehaviour {



    void Update(){
        // Keep track of last time contacting the ground
        // If too much time (2 seconds?) until last time contacted, fade out and game over sequence
        // Also stop counting if velocity is very very low
    }
}
