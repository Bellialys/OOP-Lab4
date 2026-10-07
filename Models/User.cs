namespace OOP_Lab4.Models;

public class User
{
    public string Name { get; }
    public string Email { get; }
    public string Nickname { get; }
    public List<Post> Posts { get; }

    public User(string name, string email, string nickname)
    {
        Name = name;
        Email = email;
        Nickname = nickname.StartsWith("@") ? nickname : "@" + nickname;
        Posts = new List<Post>();
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

        int resultCount = Math.Min(count, Posts.Count);
        Post[] result = new Post[resultCount];

        for (int i = 0; i < resultCount; i++)
        {
            result[i] = Posts[Posts.Count - 1 - i];
        }

        return result;
    }

    public void DisplayProfile()
    {
        Console.WriteLine($"{Name,-20} | {Email,-30} | {Nickname,-15} | Постів: {Posts.Count}");
    }
}
