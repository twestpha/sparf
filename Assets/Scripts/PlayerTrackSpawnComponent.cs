using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackSpawnComponent : MonoBehaviour {
    public static PlayerTrackSpawnComponent instance;

    public TrackComponent trackRoot;
    private TrackComponent currentTrack;

    public GameObject[] trackPrefabs;

    public Material[] trackMaterials;
    public AnimationCurve trackSpawnHeightCurve;
    public AnimationCurve trackSpawnScaleCurve;

    private float time;

    void Start(){
        instance = this;
        currentTrack = trackRoot;
    }

    void Update(){
        time += Time.deltaTime;

        if(time > 1.0f){
            time = 0.0f;

            GameObject newTrackPiece = GameObject.Instantiate(trackPrefabs[Random.Range(0, trackPrefabs.Length)]);

            newTrackPiece.transform.position = currentTrack.endTransform.transform.position;
            newTrackPiece.transform.rotation = currentTrack.endTransform.transform.rotation;

            currentTrack = newTrackPiece.GetComponent<TrackComponent>();
        }
    }
}
