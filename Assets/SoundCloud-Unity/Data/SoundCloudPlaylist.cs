
namespace SoundCloud
{

public class SoundCloudPlaylist : DataObject<SoundCloudPlaylist>
{
    public const string API_CALL = "http://api.soundcloud.com/playlists/{0}?client_id={1}";

    public int id { get; protected set; }
}

}