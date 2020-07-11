using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackComponent : MonoBehaviour {
    private static int previousMaterialIndex = -1;

    public GameObject endTransform;

    private Vector3 spawnedPosition;
    private Timer spawnTimer;

    private Collider coll;
    private PlayerTrackSpawnComponent spawner;

    public TrackComponent previousTrack;

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

        // Lazy shuffle
        int newMaterialIndex = previousMaterialIndex;
        while(newMaterialIndex == previousMaterialIndex){
            newMaterialIndex = Random.Range(0, spawner.trackMaterials.Length);
        }

        GetComponent<Renderer>().material = spawner.trackMaterials[newMaterialIndex];
        previousMaterialIndex = newMaterialIndex;
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

    void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "Player"){
            if(previousTrack != null){
                // Disable two tracks' ago collision
                if(previousTrack.previousTrack != null){
                    previousTrack.previousTrack.GetComponent<Collider>().enabled = false;
                }

                // Destroy 5 tracks ago
                try {
                    Destroy(previousTrack.previousTrack.previousTrack.previousTrack.previousTrack.gameObject);
                } catch {
                    // Fuck
                }
            }
        }
    }
}
