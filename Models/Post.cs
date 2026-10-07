namespace OOP_Lab4.Models;

public class Post
{
    private static int _nextId = 1;

    public int Id { get; }
    public DateTime CreatedAt { get; }
    public string Text { get; }
    public int Likes { get; private set; }
    public List<Comment> Comments { get; }

    public Post(string text, DateTime createdAt, int initialLikes = 0)
    {
        Id = _nextId++;
        CreatedAt = createdAt;
        Text = text;
        Likes = Math.Max(0, initialLikes);
        Comments = new List<Comment>();
    }

    public void Like()
    {
        Likes++;
    }

    public void AddComment(Comment comment)
    {
        Comments.Add(comment);
    }

    public void Display(string ownerNickname)
    {
        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"Пост #{Id} | Автор: {ownerNickname}");
        Console.WriteLine($"Дата: {CreatedAt:dd.MM.yyyy HH:mm}");
        Console.WriteLine($"Текст: {Text}");
        Console.WriteLine($"Лайки: {Likes}");
        Console.WriteLine($"Коментарі: {Comments.Count}");

        if (Comments.Count == 0)
        {
            Console.WriteLine("  Коментарів поки немає.");
        }
        else
        {
            foreach (Comment comment in Comments)
            {
                Console.WriteLine($"  {comment.AuthorNickname} [{comment.CreatedAt:dd.MM.yyyy HH:mm}]: {comment.Text}");
            }
        }
    }
}
