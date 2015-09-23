using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SoundCloud
{

public class SCTranscoder : MonoBehaviour
{
    private const float TIMEOUT = 15.0f;

    public void Transcode(string mp3FilePath, string outputFilePath, Action<bool> callback)
    {
        StartCoroutine(ProcessTranscode(mp3FilePath, outputFilePath, callback));
    }

    private IEnumerator ProcessTranscode(string mp3FilePath, string outputFilePath, Action<bool> callback)
    {
        bool processComplete = false;
        bool transcoded = false;
        float timer = 0;

        System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process();
        ffmpeg.StartInfo.FileName = Application.dataPath + "/SoundCloud-Unity/FFMPEG/ffmpeg.exe";
        ffmpeg.StartInfo.Arguments = "-i \"" + mp3FilePath + "\" -y -c:a libvorbis -q:a 4 \"" + outputFilePath + "\"";
        ffmpeg.StartInfo.CreateNoWindow = true;
        ffmpeg.StartInfo.UseShellExecute = false;

        ffmpeg.EnableRaisingEvents = true;
        ffmpeg.Exited += (sender, args) =>
            {
                if(ffmpeg.ExitCode == 0)
                    transcoded = true;

                processComplete = true;
            };
        ffmpeg.Start();

        while (!processComplete && timer < TIMEOUT)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        if (transcoded)
        {
            Debug.Log("Transcode Complete (" + timer + " seconds)");
        }
        else if (processComplete && !transcoded)
        {
            Debug.LogError("Transcode Failed. Error code: " + ffmpeg.ExitCode);
        }
        else if (!processComplete)
        {
            Debug.LogError("Transcode Failed. Timed Out.");
            ffmpeg.Kill();
        }

        if (callback != null)
            callback(transcoded);
    }
}

}