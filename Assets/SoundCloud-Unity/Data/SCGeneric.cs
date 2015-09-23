
namespace SoundCloud
{

public class SCGeneric : DataObject<SCGeneric>
{
    public const string API_CALL = "";

    public int id { get; protected set; }
    public string kind { get; protected set; }
    public string uri { get; protected set; }
}

}