namespace OOP_Lab4.Models;

public class User
{
    private readonly HashSet<string> _following =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public string Name { get; }
    public string Email { get; }
    public string Nickname { get; }
    public string Avatar { get; }
    public string Bio { get; }
    public List<Post> Posts { get; }
    public int FollowingCount => _following.Count;

    public User(
        string name,
        string email,
        string nickname,
        string avatar = "🙂",
        string bio = "")
    {
        if (string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(nickname))
        {
            throw new ArgumentException("Дані користувача не можуть бути порожніми.");
        }

        Name = name.Trim();
        Email = email.Trim();
        Nickname = nickname.StartsWith("@") ? nickname : "@" + nickname;
        Avatar = avatar;
        Bio = bio;
        Posts = new List<Post>();
    }

    public bool IsFollowing(User other)
    {
        return _following.Contains(other.Nickname);
    }

    public bool Follow(User other)
    {
        if (ReferenceEquals(this, other))
            return false;

        return _following.Add(other.Nickname);
    }

    public bool Unfollow(User other)
    {
        return _following.Remove(other.Nickname);
    }

    public Post CreatePost(string text)
    {
        return CreatePost(text, DateTime.Now, 0);
    }

    public Post CreatePost(string text, DateTime createdAt, int initialLikes)
    {
        Post post = new Post(text, createdAt, initialLikes);
        Posts.Add(post);
        return post;
    }

    public bool DeletePost(int postId)
    {
        for (int i = 0; i < Posts.Count; i++)
        {
            if (Posts[i].Id == postId)
            {
                Posts.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    public Post? FindPost(int postId)
    {
        foreach (Post post in Posts)
        {
            if (post.Id == postId)
                return post;
        }

        return null;
    }

    public Post[] GetLastPosts(int count)
    {
        if (count <= 0 || Posts.Count == 0)
            return Array.Empty<Post>();

        List<Post> ordered = new List<Post>(Posts);
        ordered.Sort((a, b) =>
        {
            int byDate = b.CreatedAt.CompareTo(a.CreatedAt);
            return byDate != 0 ? byDate : b.Id.CompareTo(a.Id);
        });

        int resultCount = Math.Min(count, ordered.Count);
        Post[] result = new Post[resultCount];

        for (int i = 0; i < result.Length; i++)
            result[i] = ordered[i];

        return result;
    }

    public void DisplayProfile()
    {
        Console.WriteLine(
            $"{Avatar} {Name} ({Nickname}) | " +
            $"Дописів: {Posts.Count} | Підписок: {FollowingCount}");
        Console.WriteLine($"   ✉️  {Email}");
        Console.WriteLine($"   📝  {Bio}");
    }
}
