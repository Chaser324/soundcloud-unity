
namespace SoundCloud
{

public class SCPlaylist : DataObject<SCPlaylist>
{
    public const string API_CALL = "http://api.soundcloud.com/playlists/{0}?client_id={1}";

    public int id { get; protected set; }
}

}