using SoundCloud;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioSource))]
public class Tester : MonoBehaviour
{
    #region Private Variables

    private Text console;
    private Text input;
    private AudioSource audioSource;

    private SCTrack currentTrack;

    private float[] musicSpectrum = new float[32];

    #endregion

    #region Unity Events

    public IEnumerator Start()
    {
        GameObject uiPanel = GameObject.Find("UIRoot/Panel");
        uiPanel.SetActive(false);

        while (!SCManager.initialized)
            yield return 0;

        uiPanel.SetActive(true);

        console = GameObject.Find("OutputPanel/Text").GetComponent<Text>();
        input = GameObject.Find("SoundCloudURL/Text").GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(ShowTrackProgress());
        StartCoroutine(AnalyzeMusicSpectrum());
    }

    #endregion

    #region Public Methods

    public void SignIn()
    {
        SCManager.Connect(ConnectCallback);
    }

    public void GenericDataCall()
    {
        SCManager.GetDataType(input.text, GetDataTypeCallback);
    }

    public void PlayTrack()
    {
        SCManager.GetTrack(input.text, PlayTrackDataCallback);
        SCManager.GetTrackAudioClip(input.text, GetTrackAudioClipCallback);
    }

    public void PauseTrack()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
        else
            audioSource.Play();
    }

    public void OpenArtistPage()
    {
        if (currentTrack != null)
            Application.OpenURL(currentTrack.user.permalink_url);
    }

    public void OpenTrackPage()
    {
        if (currentTrack != null)
            Application.OpenURL(currentTrack.permalink_url);
    }

    #endregion

    #region Private Methods

    private void ConnectCallback(bool success)
    {
        if (success)
            console.text = "Connection successful.";
        else
            console.text = "Connection failed.";
    }

    private void GetDataTypeCallback(string url, Type dataType)
    {
        if (dataType == typeof(SCUser))
            SCManager.GetUser(url, GetUserCallback);
        else if (dataType == typeof(SCTrack))
            SCManager.GetTrack(url, GetTrackCallback);
    }

    private void GetUserCallback(SCUser user)
    {
        console.text = user.ToString();
    }

    private void GetTrackCallback(SCTrack track)
    {
        console.text = track.ToString();
    }

    private void PlayTrackDataCallback(SCTrack track)
    {
        currentTrack = track;

        Text artist = GameObject.Find("TrackInfo/Artist/Text").GetComponent<Text>();
        Text trackname = GameObject.Find("TrackInfo/Track/Text").GetComponent<Text>();

        artist.text = track.user.username;
        trackname.text = track.title;

        SCManager.GetImage(track.artwork_url, GetAlbumArtCallback);
        SCManager.GetImage(track.waveform_url, GetWaveFormCallback);
    }

    private void GetTrackAudioClipCallback(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.volume = 1.0f;
        audioSource.time = 0;
        audioSource.Play();
    }

    private void GetAlbumArtCallback(Texture2D image)
    {
        RawImage art = GameObject.Find("TrackInfo/AlbumArt").GetComponent<RawImage>();
        art.texture = image;
    }

    private void GetWaveFormCallback(Texture2D image)
    {
        RawImage art = GameObject.Find("TrackInfo/WaveForm").GetComponent<RawImage>();
        art.texture = image;
    }

    private IEnumerator ShowTrackProgress()
    {
        Image trackProgress = GameObject.Find("TrackInfo/WaveFormProgress").GetComponent<Image>();

        while (true)
        {
            while (audioSource.isPlaying)
            {
                trackProgress.fillAmount = audioSource.time / audioSource.clip.length;
                yield return 0;
            }
            yield return 0;
        }
    }

    private IEnumerator AnalyzeMusicSpectrum()
    {
        Image bar = GameObject.Find("Visualization/Bar").GetComponent<Image>();

        Image[] levelBars = new Image[musicSpectrum.Length];
        levelBars[0] = bar;

        float barWidth = (1.0f / levelBars.Length);
        for (int i = 0; i < levelBars.Length; i++)
        {
            levelBars[i] = Instantiate<GameObject>(bar.gameObject).GetComponent<Image>();
            RectTransform rt = levelBars[i].GetComponent<RectTransform>();

            rt.SetParent(bar.transform.parent, true);
            rt.anchorMin = new Vector2(i * barWidth, 0);
            rt.anchorMax = new Vector2((i + 1) * barWidth, 1);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
        }

        Destroy(bar.gameObject);

        while (true)
        {
            while (audioSource.isPlaying)
            {
                float[] spectrum = new float[1024];
                audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

                for (int i = 0; i < musicSpectrum.Length; i++)
                {
                    musicSpectrum[i] = 0f;

                    int min = i * (1024 / musicSpectrum.Length);
                    int max = (i + 1) * (1024 / musicSpectrum.Length);

                    for (int j = min; j < max; j++)
                    {
                        musicSpectrum[i] += spectrum[j];
                    }

                    levelBars[i].fillAmount = Mathf.MoveTowards(levelBars[i].fillAmount, musicSpectrum[i], 0.1f);
                }

                yield return 0;
            }

            yield return 0;
        }
    }

    #endregion
}
