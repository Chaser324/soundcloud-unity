
namespace SoundCloud
{

public class SCComment : DataObject<SCComment>
{
    public const string API_CALL = "http://api.soundcloud.com/comments/{0}?client_id={1}";

    public int id { get; protected set; }
}

}