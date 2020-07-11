using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTrackSpawnComponent : MonoBehaviour {
    public static PlayerTrackSpawnComponent instance;

    [Header("Track Generation")]
    public TrackComponent trackRoot;
    private TrackComponent currentTrack;

    public GameObject[] trackPrefabs;

    public Material[] trackMaterials;
    public AnimationCurve trackSpawnHeightCurve;
    public AnimationCurve trackSpawnScaleCurve;
    public AnimationCurve trackSpawnRotationCurve;

    [Header("UI")]
    public Text timerText;

    private float roundStartTime;

    private float tempTime;

    void Start(){
        instance = this;
        currentTrack = trackRoot;

        roundStartTime = Time.time;
    }

    void Update(){
        tempTime += Time.deltaTime;

        if(tempTime > 1.0f){
            tempTime = 0.0f;

            GameObject newTrackPiece = GameObject.Instantiate(trackPrefabs[Random.Range(0, trackPrefabs.Length)]);

            newTrackPiece.transform.position = currentTrack.endTransform.transform.position;
            newTrackPiece.transform.rotation = currentTrack.endTransform.transform.rotation;

            TrackComponent previousTrack = currentTrack;
            currentTrack = newTrackPiece.GetComponent<TrackComponent>();

            currentTrack.previousTrack = previousTrack;
        }

        timerText.text = (Time.time - roundStartTime).ToString();
    }
}
