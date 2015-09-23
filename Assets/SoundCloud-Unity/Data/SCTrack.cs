
namespace SoundCloud
{

public class SCTrack : DataObject<SCTrack>
{
    public const string API_CALL = "http://api.soundcloud.com/tracks/{0}?client_id={1}";

    public int id { get; protected set; }
    public string created_at { get; protected set; }
    public int user_id { get; protected set; }
    public int duration { get; protected set; }
    public bool commentable { get; protected set; }
    public string state { get; protected set; }
    public string sharing { get; protected set; }
    public string tag_list { get; protected set; }
    public string permalink { get; protected set; }
    public string description { get; protected set; }
    public bool streamable { get; protected set; }
    public bool downloadable { get; protected set; }
    public string genre { get; protected set; }
    public string release { get; protected set; }
    public string purchase_url { get; protected set; }
    public string label_id { get; protected set; }
    public string label_name { get; protected set; }
    public string isrc { get; protected set; }
    public string video_url { get; protected set; }
    public string track_type { get; protected set; }
    public string key_signature { get; protected set; }
    public int bpm { get; protected set; }
    public string title { get; protected set; }
    public string release_year { get; protected set; }
    public string release_month { get; protected set; }
    public string release_day { get; protected set; }
    public string original_format { get; protected set; }
    public string original_content_size { get; protected set; }
    public string permalink_url { get; protected set; }
    public string uri { get; protected set; }
    public string embeddable_by { get; protected set; }
    public string artwork_url { get; protected set; }
    public string waveform_url { get; protected set; }
    public string download_url { get; protected set; }
    public string stream_url { get; protected set; }
    public string license { get; protected set; }
    public int comment_count { get; protected set; }
    public int download_count { get; protected set; }
    public int playback_count { get; protected set; }
    public int favoritings_count { get; protected set; }
    public SCUser user { get; protected set; }

    // authenticated requests only
    public bool user_favorite { get; protected set; }
}

}
