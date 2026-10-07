using OOP_Lab4.Models;

namespace OOP_Lab4.Services;

public class SocialNetwork
{
    public List<User> Users { get; }
    private User _currentUser;

    public SocialNetwork()
    {
        Users = CreateUsers();
        _currentUser = Users[0];
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("       СИМУЛЯЦІЯ СОЦІАЛЬНОЇ МЕРЕЖІ");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Поточний користувач: {_currentUser.Name} ({_currentUser.Nickname})");
            Console.WriteLine();
            Console.WriteLine("1 - Показати всіх користувачів");
            Console.WriteLine("2 - Отримати останні 5 постів іншого користувача");
            Console.WriteLine("3 - Поставити лайк посту іншого користувача");
            Console.WriteLine("4 - Написати коментар до поста іншого користувача");
            Console.WriteLine("5 - Створити новий пост");
            Console.WriteLine("6 - Видалити свій пост");
            Console.WriteLine("7 - Показати свої пости");
            Console.WriteLine("8 - Змінити поточного користувача");
            Console.WriteLine("0 - Повернутися до головного меню");

            int choice = ConsoleHelper.ReadIntInRange("Ваш вибір: ", 0, 8);

            switch (choice)
            {
                case 1:
                    ShowAllUsers();
                    ConsoleHelper.Pause();
                    break;
                case 2:
                    ShowLastFivePosts();
                    ConsoleHelper.Pause();
                    break;
                case 3:
                    LikeOtherUserPost();
                    ConsoleHelper.Pause();
                    break;
                case 4:
                    CommentOtherUserPost();
                    ConsoleHelper.Pause();
                    break;
                case 5:
                    CreateOwnPost();
                    ConsoleHelper.Pause();
                    break;
                case 6:
                    DeleteOwnPost();
                    ConsoleHelper.Pause();
                    break;
                case 7:
                    ShowOwnPosts();
                    ConsoleHelper.Pause();
                    break;
                case 8:
                    ChangeCurrentUser();
                    break;
                case 0:
                    return;
            }
        }
    }

    private List<User> CreateUsers()
    {
        User olena = new User("Олена Коваль", "olena.koval@example.com", "@olena");
        User maksym = new User("Максим Бондар", "maksym.bondar@example.com", "@maksym");
        User iryna = new User("Ірина Шевченко", "iryna.shevchenko@example.com", "@iryna");

        olena.CreatePost("Сьогодні почала вивчати C#.", new DateTime(2026, 9, 28, 10, 15, 0), 4);
        olena.CreatePost("Перший клас у моєму навчальному проєкті готовий.", new DateTime(2026, 9, 30, 14, 20, 0), 7);
        olena.CreatePost("Розбираюся з конструкторами та властивостями.", new DateTime(2026, 10, 1, 18, 5, 0), 5);
        olena.CreatePost("Сьогодні практика з масивами.", new DateTime(2026, 10, 3, 11, 30, 0), 9);
        olena.CreatePost("Написала невелику консольну програму.", new DateTime(2026, 10, 5, 16, 45, 0), 12);
        Post o6 = olena.CreatePost("Готую практичне завдання з ООП.", new DateTime(2026, 10, 7, 9, 10, 0), 15);

        maksym.CreatePost("Ранкова кава і трохи програмування.", new DateTime(2026, 9, 27, 8, 40, 0), 6);
        maksym.CreatePost("Спробував новий підхід до структури проєкту.", new DateTime(2026, 9, 29, 19, 0, 0), 8);
        maksym.CreatePost("Сьогодні повторюю цикли for і while.", new DateTime(2026, 10, 1, 12, 35, 0), 3);
        Post m4 = maksym.CreatePost("Зробив симуляцію магазину.", new DateTime(2026, 10, 3, 15, 20, 0), 11);
        maksym.CreatePost("Тестую роботу зі структурами.", new DateTime(2026, 10, 5, 17, 50, 0), 10);
        maksym.CreatePost("Наступний крок — модель соціальної мережі.", new DateTime(2026, 10, 7, 10, 25, 0), 14);

        iryna.CreatePost("Планую навчання на цей тиждень.", new DateTime(2026, 9, 26, 9, 0, 0), 5);
        iryna.CreatePost("Практикую методи у C#.", new DateTime(2026, 9, 29, 13, 10, 0), 7);
        iryna.CreatePost("Розібралася, чим клас відрізняється від об'єкта.", new DateTime(2026, 10, 2, 10, 30, 0), 13);
        iryna.CreatePost("Сьогодні працюю з колекціями.", new DateTime(2026, 10, 4, 12, 0, 0), 8);
        iryna.CreatePost("Додала коментарі до постів.", new DateTime(2026, 10, 6, 18, 15, 0), 16);
        Post i6 = iryna.CreatePost("Проєкт уже майже готовий до тестування.", new DateTime(2026, 10, 7, 11, 40, 0), 18);

        o6.AddComment(new Comment("@maksym", "Успіхів із завданням!", new DateTime(2026, 10, 7, 10, 5, 0)));
        m4.AddComment(new Comment("@iryna", "Цікаво подивитися на результат.", new DateTime(2026, 10, 3, 16, 0, 0)));
        i6.AddComment(new Comment("@olena", "Чекаю на фінальну версію.", new DateTime(2026, 10, 7, 12, 0, 0)));

        return new List<User> { olena, maksym, iryna };
    }

    private void ShowAllUsers()
    {
        Console.WriteLine();
        Console.WriteLine("========== КОРИСТУВАЧІ ==========");

        for (int i = 0; i < Users.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            Users[i].DisplayProfile();
        }
    }

    private void ShowLastFivePosts()
    {
        User? otherUser = SelectOtherUser();
        if (otherUser == null)
            return;

        Post[] posts = otherUser.GetLastPosts(5);

        Console.WriteLine();
        Console.WriteLine($"Останні {posts.Length} постів користувача {otherUser.Nickname}:");
        DisplayPosts(otherUser, posts);
    }

    private void LikeOtherUserPost()
    {
        User? otherUser = SelectOtherUser();
        if (otherUser == null)
            return;

        Post? post = SelectPost(otherUser);
        if (post == null)
            return;

        post.Like();

        Console.WriteLine($"Лайк додано до поста #{post.Id} користувача {otherUser.Nickname}. Тепер лайків: {post.Likes}.");
    }

    private void CommentOtherUserPost()
    {
        User? otherUser = SelectOtherUser();
        if (otherUser == null)
            return;

        Post? post = SelectPost(otherUser);
        if (post == null)
            return;

        string text = ConsoleHelper.ReadNonEmptyString("Введіть текст коментаря: ");
        Comment comment = new Comment(_currentUser.Nickname, text, DateTime.Now);
        post.AddComment(comment);

        Console.WriteLine($"Коментар додано до поста #{post.Id}.");
    }

    private void CreateOwnPost()
    {
        string text = ConsoleHelper.ReadNonEmptyString("Введіть текст нового поста: ");
        Post post = _currentUser.CreatePost(text);
        Console.WriteLine($"Новий пост створено. ID поста: {post.Id}.");
    }

    private void DeleteOwnPost()
    {
        if (_currentUser.Posts.Count == 0)
        {
            Console.WriteLine("У вас немає постів для видалення.");
            return;
        }

        ShowOwnPosts();
        int postId = ConsoleHelper.ReadPositiveInt("Введіть ID свого поста для видалення: ");

        if (_currentUser.DeletePost(postId))
            Console.WriteLine($"Пост #{postId} видалено.");
        else
            Console.WriteLine("Пост із таким ID не знайдено серед ваших постів.");
    }

    private void ShowOwnPosts()
    {
        Console.WriteLine();
        Console.WriteLine($"========== ПОСТИ {_currentUser.Nickname} ==========");

        if (_currentUser.Posts.Count == 0)
        {
            Console.WriteLine("Постів поки немає.");
            return;
        }

        Post[] posts = _currentUser.GetLastPosts(_currentUser.Posts.Count);
        DisplayPosts(_currentUser, posts);
    }

    private void ChangeCurrentUser()
    {
        ShowAllUsers();

        int number = ConsoleHelper.ReadIntInRange("Оберіть номер користувача: ", 1, Users.Count);
        _currentUser = Users[number - 1];

        Console.WriteLine($"Поточний користувач змінений на {_currentUser.Nickname}.");
    }

    private User? SelectOtherUser()
    {
        ShowAllUsers();

        int number = ConsoleHelper.ReadIntInRange("Оберіть номер іншого користувача: ", 1, Users.Count);
        User selectedUser = Users[number - 1];

        if (selectedUser == _currentUser)
        {
            Console.WriteLine("Для цієї дії потрібно вибрати іншого користувача, а не себе.");
            return null;
        }

        return selectedUser;
    }

    private Post? SelectPost(User user)
    {
        if (user.Posts.Count == 0)
        {
            Console.WriteLine("У вибраного користувача немає постів.");
            return null;
        }

        Post[] posts = user.GetLastPosts(user.Posts.Count);
        DisplayPosts(user, posts);

        int postId = ConsoleHelper.ReadPositiveInt("Введіть ID поста: ");
        Post? post = user.FindPost(postId);

        if (post == null)
            Console.WriteLine("Пост із таким ID у вибраного користувача не знайдено.");

        return post;
    }

    private static void DisplayPosts(User owner, Post[] posts)
    {
        if (posts.Length == 0)
        {
            Console.WriteLine("Постів немає.");
            return;
        }

        foreach (Post post in posts)
        {
            post.Display(owner.Nickname);
        }

        Console.WriteLine(new string('-', 70));
    }
}
