
namespace SoundCloud
{

public enum SCError : int
{
    OK = 0,                             // success
    Fail = 1,                           // generic failure
    NotStreamable = 2,                  // track is not streamable
    TranscodeTimeout = 3,               // transcode timed out
    TranscodeFailed = 4,                // transcode failed
}

}
