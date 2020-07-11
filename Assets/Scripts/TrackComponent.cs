using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackComponent : MonoBehaviour {

    public GameObject endTransform;

    private Vector3 spawnedPosition;
    private Timer spawnTimer;

    private Collider coll;
    private PlayerTrackSpawnComponent spawner;

    void Start(){
        // Don't execute for track root
        spawner = PlayerTrackSpawnComponent.instance;
        if(spawner.trackRoot == this){
            enabled = false;
            return;
        }

        spawnedPosition = transform.position;
        spawnTimer = new Timer(0.25f);
        spawnTimer.Start();

        coll = GetComponent<Collider>();
        coll.enabled = false;

        transform.position += (Vector3.up * spawner.trackSpawnHeightCurve.Evaluate(0.0f));

        float scale = spawner.trackSpawnScaleCurve.Evaluate(0.0f);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void Update(){
        if(!coll.enabled){
            float t = spawnTimer.Parameterized();

            transform.position = spawnedPosition + (Vector3.up * spawner.trackSpawnHeightCurve.Evaluate(t));

            float scale = spawner.trackSpawnScaleCurve.Evaluate(t);
            transform.localScale = new Vector3(scale, scale, scale);

            if(spawnTimer.Finished()){
                coll.enabled = true;
                enabled = false;
            }
        }
    }
}
