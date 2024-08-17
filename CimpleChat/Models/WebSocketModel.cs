namespace CimpleChat.Models;

public class WebSocketModel<T>
{
    public string Type { get; set; }
    public T Data { get; set; }

    public WebSocketModel(string type, T data)
    {
        Type = type;
        Data = data;
    }
}