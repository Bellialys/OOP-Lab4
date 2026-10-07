namespace OOP_Lab4.Models;

public struct Comment
{
    public string AuthorNickname { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }

    public Comment(string authorNickname, string text, DateTime createdAt)
    {
        AuthorNickname = authorNickname;
        Text = text;
        CreatedAt = createdAt;
    }
}
