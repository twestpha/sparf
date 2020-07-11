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
    public Button[] selectionButtons;
    public Sprite[] selectionSprites;

    private float roundStartTime;
    private int[] buttonTrackIndex;
    private Vector3 rootStartPosition;
    private Quaternion rootStartRotation;

    void Start(){
        instance = this;
        currentTrack = trackRoot;

        rootStartPosition = trackRoot.transform.position;
        rootStartRotation = trackRoot.transform.rotation;

        roundStartTime = Time.time;

        buttonTrackIndex = new int[5];

        FillOutSelectionButton(0);
        FillOutSelectionButton(1);
        FillOutSelectionButton(2);
        FillOutSelectionButton(3);
        FillOutSelectionButton(4);
    }

    void FillOutSelectionButton(int index){
        int randomTrackIndex = Random.Range(0, trackPrefabs.Length);

        selectionButtons[index].transform.GetChild(0).GetComponent<Image>().sprite = selectionSprites[randomTrackIndex];
        buttonTrackIndex[index] = randomTrackIndex;
    }

    void Update(){
        timerText.text = (Time.time - roundStartTime).ToString();

        if(Input.GetKeyDown(KeyCode.R)){
            ResetGame();
        }
    }

    public void ButtonClicked(int index){
        GameObject newTrackPiece = GameObject.Instantiate(trackPrefabs[buttonTrackIndex[index]]);

        newTrackPiece.transform.position = currentTrack.endTransform.transform.position;
        newTrackPiece.transform.rotation = currentTrack.endTransform.transform.rotation;

        TrackComponent previousTrack = currentTrack;
        currentTrack = newTrackPiece.GetComponent<TrackComponent>();

        currentTrack.previousTrack = previousTrack;

        FillOutSelectionButton(index);
    }

    public void ResetGame(){
        roundStartTime = Time.time;

        // Kill all track pieces
        TrackComponent[] tracks = FindObjectsOfType<TrackComponent>();
        foreach(TrackComponent track in tracks){
            Destroy(track.gameObject);
        }

        // Teleport player to home
        GameObject playerBall = transform.GetChild(0).gameObject;
        playerBall.transform.localPosition = Vector3.zero;
        playerBall.transform.localRotation = Quaternion.identity;

        playerBall.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Create new root
        GameObject newTrackPiece = GameObject.Instantiate(trackPrefabs[0]);

        newTrackPiece.transform.position = rootStartPosition;
        newTrackPiece.transform.rotation = rootStartRotation;

        currentTrack = newTrackPiece.GetComponent<TrackComponent>();
    }
}
