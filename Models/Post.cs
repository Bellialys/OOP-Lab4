namespace OOP_Lab4.Models;

public class Post
{
    private static int _nextId = 1;
    private readonly int _initialLikes;
    private readonly Dictionary<string, string> _reactions =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public int Id { get; }
    public DateTime CreatedAt { get; }
    public string Text { get; }
    public int Likes => _initialLikes + _reactions.Count;
    public List<Comment> Comments { get; }
    public int ReactionCount => _reactions.Count;

    public Post(string text, DateTime createdAt, int initialLikes = 0)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Допис не може бути порожнім.", nameof(text));

        Id = _nextId++;
        CreatedAt = createdAt;
        Text = text.Trim();
        _initialLikes = Math.Max(0, initialLikes);
        Comments = new List<Comment>();
    }

    public bool TryAddReaction(string nickname, string emoji)
    {
        if (string.IsNullOrWhiteSpace(nickname) ||
            string.IsNullOrWhiteSpace(emoji))
        {
            return false;
        }

        return _reactions.TryAdd(nickname, emoji);
    }

    public string GetReactionOf(string nickname)
    {
        return _reactions.TryGetValue(nickname, out string? emoji)
            ? emoji
            : "";
    }

    public void AddComment(Comment comment)
    {
        if (string.IsNullOrWhiteSpace(comment.Text))
            throw new ArgumentException("Коментар не може бути порожнім.");

        Comments.Add(comment);
    }

    public void Display(User author)
    {
        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine($"{author.Avatar} {author.Name}  {author.Nickname}");
        Console.WriteLine($"🗓️  {CreatedAt:dd.MM.yyyy HH:mm}  •  Допис #{Id}");
        Console.WriteLine();
        Console.WriteLine($"   {Text}");
        Console.WriteLine();
        Console.WriteLine($"❤️  {Likes}    💬 {Comments.Count}");

        if (Comments.Count > 0)
        {
            Console.WriteLine("   ── Коментарі ──");
            foreach (Comment comment in Comments)
            {
                Console.WriteLine(
                    $"   💬 {comment.AuthorNickname} " +
                    $"[{comment.CreatedAt:dd.MM.yyyy HH:mm}]: {comment.Text}");
            }
        }
    }
}
