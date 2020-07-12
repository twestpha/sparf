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

    private int trackDeckNextCardIndex;
    public int[] trackDeck;

    [Header("UI")]
    public Text timerText;
    public Button[] selectionButtons;
    public Sprite[] selectionSprites;
    public Text hiScoreTimerText;
    public Text hiScoreExcitementText;
    public Image fadeLayer;

    private float roundStartTime;
    private float hiScore;
    private int[] buttonTrackIndex;
    private Vector3 rootStartPosition;
    private Quaternion rootStartRotation;

    private bool runningTimer = true;

    [Header("Physics")]
    public PhysicMaterial physicsMaterial;

    void Start(){
        instance = this;
        currentTrack = trackRoot;

        hiScoreTimerText.enabled = false;
        hiScoreExcitementText.enabled = false;

        rootStartPosition = trackRoot.transform.position;
        rootStartRotation = trackRoot.transform.rotation;

        roundStartTime = Time.time;

        ShuffleTrackDeck();

        buttonTrackIndex = new int[5];

        FillOutSelectionButton(0);
        FillOutSelectionButton(1);
        FillOutSelectionButton(2);
        FillOutSelectionButton(3);
        FillOutSelectionButton(4);
    }

    private void ShuffleTrackDeck(){
        trackDeckNextCardIndex = 0;

        // My boy fisher and my bro Yates
        for(int i = 0; i < trackDeck.Length; ++i){
            int swapIndex = Random.Range(i, trackDeck.Length);

            int temp = trackDeck[i];
            trackDeck[i] = trackDeck[swapIndex];
            trackDeck[swapIndex] = temp;
        }
    }

    private int GetNextCard(){
        int nextTrackCard = trackDeck[trackDeckNextCardIndex];

        trackDeckNextCardIndex++;
        if(trackDeckNextCardIndex >= trackDeck.Length){
            ShuffleTrackDeck();
        }

        return nextTrackCard;
    }

    void FillOutSelectionButton(int index){
        int nextCard = GetNextCard();

        selectionButtons[index].transform.GetChild(0).GetComponent<Image>().sprite = selectionSprites[nextCard];
        buttonTrackIndex[index] = nextCard;
    }

    void Update(){
        float elapsed = Time.time - roundStartTime;

        if(runningTimer){
            timerText.text = elapsed.ToString("0.00");
        }

        if(Input.GetKeyDown(KeyCode.R)){
            ResetGame();
        }

        // Difficulty curve
        physicsMaterial.dynamicFriction = Mathf.Lerp(0.09f, 0.03f, elapsed / 60.0f);
    }

    void OnDestroy(){
        physicsMaterial.dynamicFriction = 0.09f;
    }

    public void ButtonClicked(int index){
        GameObject newTrackPiece = GameObject.Instantiate(trackPrefabs[buttonTrackIndex[index]]);

        newTrackPiece.transform.position = currentTrack.endTransform.transform.position;
        newTrackPiece.transform.rotation = currentTrack.endTransform.transform.rotation;

        TrackComponent previousTrack = currentTrack;
        currentTrack = newTrackPiece.GetComponent<TrackComponent>();

        currentTrack.previousTrack = previousTrack;

        IEnumerator coroutine = DiscardAndDrawNewCard(index);
        StartCoroutine(coroutine);
    }

    public void DiscardAllCards(){
        ShuffleTrackDeck();

        for(int i = 0; i < 5; ++i){
            IEnumerator coroutine = DiscardAndDrawNewCard(i);
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator DiscardAndDrawNewCard(int index){
        selectionButtons[index].interactable = false;

        Timer discardTimer = new Timer(0.4f);
        discardTimer.Start();

        Image selectionButtonImage = selectionButtons[index].GetComponent<Image>();
        Image selectionChildImage = selectionButtonImage.transform.GetChild(0).GetComponent<Image>();

        while(!discardTimer.Finished()){
            float t = discardTimer.Parameterized();
            t = Mathf.Sqrt(t);

            // Rotate 0 to -45
            selectionButtonImage.rectTransform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, t * -45.0f));

            // Fall down -130 to -240
            selectionButtonImage.rectTransform.anchoredPosition = new Vector2(
                selectionButtonImage.rectTransform.anchoredPosition.x,
                Mathf.Lerp(-130.0f, -240.0f, t)
            );

            // Fade out
            selectionButtonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - t);
            // Also fade out sprite
            selectionChildImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - t);

            yield return null;
        }

        // Make sure they're fully faded out
        selectionButtonImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        selectionChildImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        Timer waitTimer = new Timer(1.5f);
        waitTimer.Start();

        while(!waitTimer.Finished()){
            yield return null;
        }

        FillOutSelectionButton(index);

        Timer showTimer = new Timer(0.4f);
        showTimer.Start();

        // Reset rotation
        selectionButtonImage.rectTransform.localRotation = new Quaternion();

        selectionButtons[index].interactable = true;

        while(!showTimer.Finished()){
            float t = showTimer.Parameterized();
            t *= t;

            // Fall down from top -80 to -130.0
            selectionButtonImage.rectTransform.anchoredPosition = new Vector2(
                selectionButtonImage.rectTransform.anchoredPosition.x,
                Mathf.Lerp(-80.0f, -130.0f, t)
            );

            // Fade in
            selectionButtonImage.color = new Color(1.0f, 1.0f, 1.0f, t);
            // Also fade in sprite
            selectionChildImage.color = new Color(1.0f, 1.0f, 1.0f, t);

            yield return null;
        }
    }

    public void ResetGame(){
        IEnumerator coroutine = ResetGameCoroutine();
        StartCoroutine(coroutine);
    }

    private IEnumerator ResetGameCoroutine(){
        runningTimer = false;
        float officialElapsed = Time.time - roundStartTime;

        Timer waitTimer = new Timer(2.5f);
        waitTimer.Start();

        GameObject playerBall = transform.GetChild(0).gameObject;
        Rigidbody playerBallBody = playerBall.GetComponent<Rigidbody>();
        playerBallBody.velocity = Vector3.zero;
        playerBallBody.constraints = RigidbodyConstraints.FreezePosition;

        bool hiScoreAchieved = false;
        if(officialElapsed > hiScore){
            hiScore = officialElapsed;
            hiScoreAchieved = true;

            hiScoreExcitementText.enabled = true;
        }

        // flash the stopped time for a second, and if it's a high score, show high score graphic
        // Also fade out to black
        while(!waitTimer.Finished()){
            float t = (waitTimer.Parameterized() * 0.5f * Mathf.Sin(Time.time * 30.0f)) + 1.0f;
            timerText.rectTransform.localScale = new Vector3(t, t, t);

            if(hiScoreAchieved){
                hiScoreExcitementText.color = Color.HSVToRGB(Time.time % 1.0f, 1.0f, 1.0f);
            }

            float fade_t = waitTimer.Parameterized();
            fade_t *= fade_t * fade_t;

            fadeLayer.color = new Color(0.0f, 0.0f, 0.0f, fade_t);

            yield return null;
        }

        if(hiScoreAchieved){
            hiScoreTimerText.enabled = true;
            hiScoreTimerText.text = officialElapsed.ToString("0.00");
        }

        timerText.text = "0.00";

        hiScoreExcitementText.enabled = false;
        timerText.rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        physicsMaterial.dynamicFriction = 0.09f;

        for(int i = 0; i < 5; ++i){
            FillOutSelectionButton(i);
        }

        // Kill all track pieces
        TrackComponent[] tracks = FindObjectsOfType<TrackComponent>();
        foreach(TrackComponent track in tracks){
            Destroy(track.gameObject);
        }

        // Teleport player to home
        playerBall.transform.localPosition = Vector3.zero;
        playerBall.transform.localRotation = Quaternion.identity;

        playerBall.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Create new root
        GameObject newTrackPiece = GameObject.Instantiate(trackPrefabs[0]);

        newTrackPiece.transform.position = rootStartPosition;
        newTrackPiece.transform.rotation = rootStartRotation;

        currentTrack = newTrackPiece.GetComponent<TrackComponent>();

        // Fade back in
        playerBallBody.constraints = RigidbodyConstraints.None;

        Timer fadeInTimer = new Timer(2.5f);
        fadeInTimer.Start();

        while(!fadeInTimer.Finished()){
            float t = fadeInTimer.Parameterized();
            t *= t * t;

            fadeLayer.color = new Color(0.0f, 0.0f, 0.0f, 1.0f - t);

            yield return null;
        }

        fadeLayer.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);


        roundStartTime = Time.time;
        runningTimer = true;
    }
}
