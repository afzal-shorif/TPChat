namespace CimpleChat.Models.Chat;

public class WebSocketResponse<T>
{
    public string Type { get; set; }
    public T Data { get; set; }

    public WebSocketResponse(string type, T data)
    {
        Type = type;
        Data = data;
    }
}


// web socket response for message

/*
 // Single Message
{
    Type: "Message",
    Data: 
    {
        MessageType: "SingleMessage",
        MessageInfo:
        {
            Message: { message model object },
            User: { user model object}
        }
    }
}

// Multiple Message
{
    Type: "Message",
    Data: 
    {
        MessageType: "MultipleMessage",
        MessageInfo: 
        [
            {
                Message: { message model object },
                User: { user model object}
            },
            {
                Message: { message model object },
                User: { user model object}
            },
        ]
        
    }
}

// Announced Message
{
    Type: "Message",
    Data: 
    {
        MessageType: "AnnouncedMessage",
        MessageInfo:
        {
            Content: ""
        }
    }
}

*/