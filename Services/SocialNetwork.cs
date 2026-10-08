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
            ConsoleUI.WriteBanner(
                "SOCIAL HUB",
                "СИМУЛЯЦІЯ СОЦІАЛЬНОЇ МЕРЕЖІ");

            Console.WriteLine(
                $"👤 Ви увійшли як {_currentUser.Avatar} " +
                $"{_currentUser.Name} ({_currentUser.Nickname})");
            Console.WriteLine(
                $"📝 Дописів: {_currentUser.Posts.Count}   " +
                $"👥 Підписок: {_currentUser.FollowingCount}   " +
                $"⭐ Підписників: {CountFollowers(_currentUser)}");
            Console.WriteLine();

            ConsoleUI.WriteMenuItem(1, "📰 Моя стрічка (дописи підписок)");
            ConsoleUI.WriteMenuItem(2, "🔍 Останні 5 дописів іншого користувача");
            ConsoleUI.WriteMenuItem(3, "❤️  Вподобати або відреагувати на допис");
            ConsoleUI.WriteMenuItem(4, "💬 Прокоментувати чужий допис");
            ConsoleUI.WriteMenuItem(5, "✍️  Створити власний допис");
            ConsoleUI.WriteMenuItem(6, "🗑️  Видалити свій допис");
            ConsoleUI.WriteMenuItem(7, "👤 Мій профіль та дописи");
            ConsoleUI.WriteMenuItem(8, "👥 Користувачі та підписки");
            ConsoleUI.WriteMenuItem(9, "🔄 Змінити користувача");
            ConsoleUI.WriteMenuItem(10, "🔥 Популярні дописи");
            ConsoleUI.WriteMenuExit("⬅️  Повернутися до головного меню");
            Console.WriteLine();

            int choice = ConsoleHelper.ReadIntInRange(
                "Ваш вибір: ", 0, 10);

            switch (choice)
            {
                case 1:
                    ShowFeed();
                    ConsoleHelper.Pause();
                    break;
                case 2:
                    ShowLastFivePosts();
                    ConsoleHelper.Pause();
                    break;
                case 3:
                    ReactToOtherPost();
                    ConsoleHelper.Pause();
                    break;
                case 4:
                    CommentOtherPost();
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
                    ShowOwnProfile();
                    ConsoleHelper.Pause();
                    break;
                case 8:
                    ManageSubscriptions();
                    ConsoleHelper.Pause();
                    break;
                case 9:
                    ChangeCurrentUser();
                    break;
                case 10:
                    ShowTrending();
                    ConsoleHelper.Pause();
                    break;
                case 0:
                    return;
            }
        }
    }

    public (User Author, Post Post)[] GetFeed(User viewer, int count = 10)
    {
        if (count <= 0)
            return Array.Empty<(User, Post)>();

        List<(User Author, Post Post)> entries =
            new List<(User Author, Post Post)>();

        foreach (User user in Users)
        {
            if (!viewer.IsFollowing(user))
                continue;

            foreach (Post post in user.Posts)
                entries.Add((user, post));
        }

        return SortAndTake(entries, count);
    }

    public (User Author, Post Post)[] GetTrending(int count = 5)
    {
        if (count <= 0)
            return Array.Empty<(User, Post)>();

        List<(User Author, Post Post)> entries =
            new List<(User Author, Post Post)>();

        foreach (User user in Users)
        {
            foreach (Post post in user.Posts)
                entries.Add((user, post));
        }

        entries.Sort((a, b) =>
        {
            int byLikes = b.Post.Likes.CompareTo(a.Post.Likes);
            if (byLikes != 0)
                return byLikes;

            int byDate = b.Post.CreatedAt.CompareTo(a.Post.CreatedAt);
            return byDate != 0 ? byDate : b.Post.Id.CompareTo(a.Post.Id);
        });

        return Take(entries, count);
    }

    private static List<User> CreateUsers()
    {
        User olena = new User(
            "Олена Коваль",
            "olena.koval@example.com",
            "@olena",
            "👩‍💻",
            "Вивчаю C#, люблю котів 🐱 та каву ☕");
        User maksym = new User(
            "Максим Бондар",
            "maksym.bondar@example.com",
            "@maksym",
            "🧑‍💻",
            "Код, ігри 🎮 та вечірні прогулянки 🌙");
        User iryna = new User(
            "Ірина Шевченко",
            "iryna.shevchenko@example.com",
            "@iryna",
            "👩‍🎨",
            "Малюю 🎨, читаю 📚 і знайомлюся з ООП");
        User artem = new User(
            "Артем Мельник",
            "artem.melnyk@example.com",
            "@artem",
            "🧑‍🚀",
            "Технології 🚀, музика 🎵 та нові ідеї");

        DateTime today = DateTime.Today;

        olena.CreatePost("Перший крок у світі C#! 👩‍💻 #навчання", today.AddDays(-9).AddHours(10), 4);
        olena.CreatePost("Мій перший клас уже працює! 🎉", today.AddDays(-7).AddHours(14), 7);
        olena.CreatePost("Сьогодні вивчаю конструктори та властивості 📚", today.AddDays(-5).AddHours(17), 5);
        olena.CreatePost("Кава ☕ і практика з масивами — гарне поєднання!", today.AddDays(-3).AddHours(11), 9);
        olena.CreatePost("Написала маленьку консольну програму 💻✨", today.AddDays(-1).AddHours(16), 12);
        Post olenaLatest = olena.CreatePost("Готую практичне з ООП. Усе вийде! 🚀", today.AddHours(9), 15);

        maksym.CreatePost("Ранкова кава ☕ та трохи коду.", today.AddDays(-8).AddHours(8), 6);
        maksym.CreatePost("Новий підхід до структури проєкту 🧩", today.AddDays(-6).AddHours(19), 8);
        maksym.CreatePost("Повторюю цикли for і while 🔁", today.AddDays(-4).AddHours(12), 3);
        Post maxStore = maksym.CreatePost("Зробив симуляцію магазину 🛒✅", today.AddDays(-3).AddHours(15), 11);
        maksym.CreatePost("Тестую структури у C# 🧪", today.AddDays(-1).AddHours(18), 10);
        maksym.CreatePost("Наступний крок — соціальна мережа! 🌐", today.AddHours(10), 14);

        iryna.CreatePost("Планую навчання на тиждень 📅", today.AddDays(-9).AddHours(9), 5);
        iryna.CreatePost("Практикую методи в C# 🎯", today.AddDays(-7).AddHours(13), 7);
        iryna.CreatePost("Класи та об'єкти більше не плутаю 🙌", today.AddDays(-5).AddHours(10), 13);
        iryna.CreatePost("День колекцій і списків 📚", today.AddDays(-3).AddHours(12), 8);
        iryna.CreatePost("Коментарі до постів працюють! 💬", today.AddDays(-1).AddHours(18), 16);
        Post irynaLatest = iryna.CreatePost("Моя нова ілюстрація готова 🎨✨", today.AddHours(11), 18);

        artem.CreatePost("Привіт, світ! 👋🌍", today.AddDays(-8).AddHours(12), 6);
        artem.CreatePost("Слухаю улюблену музику 🎵", today.AddDays(-6).AddHours(16), 8);
        artem.CreatePost("Ідея нового проєкту 💡", today.AddDays(-4).AddHours(14), 10);
        artem.CreatePost("Сьогодні розбираюся з GitHub 🐙", today.AddDays(-2).AddHours(17), 12);
        artem.CreatePost("Все вдалося з першого разу! 🎉", today.AddDays(-1).AddHours(20), 9);
        artem.CreatePost("Найкращий день для нових експериментів 🔥", today.AddHours(12), 17);

        olenaLatest.AddComment(
            new Comment("@maksym", "Успіхів! 💪", today.AddHours(10)));
        maxStore.AddComment(
            new Comment("@iryna", "Круто, хочу подивитися! 😍", today.AddDays(-2).AddHours(10)));
        irynaLatest.AddComment(
            new Comment("@olena", "Дуже гарно! ❤️", today.AddHours(12)));

        olena.Follow(maksym);
        olena.Follow(iryna);
        maksym.Follow(olena);
        maksym.Follow(artem);
        iryna.Follow(olena);
        iryna.Follow(maksym);
        artem.Follow(iryna);
        artem.Follow(olena);

        return new List<User> { olena, maksym, iryna, artem };
    }

    private void ShowUsers()
    {
        ConsoleUI.WriteSection("КОРИСТУВАЧІ");

        for (int i = 0; i < Users.Count; i++)
        {
            User user = Users[i];
            string followState = ReferenceEquals(user, _currentUser)
                ? " (це ви)"
                : _currentUser.IsFollowing(user)
                    ? " ✅ підписані"
                    : "";

            Console.WriteLine(
                $" [{i + 1}] {user.Avatar} {user.Name}  " +
                $"{user.Nickname}{followState}");
            Console.WriteLine(
                $"     📝 {user.Posts.Count} дописів   " +
                $"⭐ {CountFollowers(user)} підписників");
        }
    }

    private int CountFollowers(User target)
    {
        int count = 0;

        foreach (User user in Users)
        {
            if (user.IsFollowing(target))
                count++;
        }

        return count;
    }

    private void ShowFeed()
    {
        ConsoleUI.WriteSection("📰 МОЯ СТРІЧКА");

        (User Author, Post Post)[] feed =
            GetFeed(_currentUser, 10);

        if (feed.Length == 0)
        {
            ConsoleUI.WriteWarning(
                "Стрічка порожня. Підпишіться на користувачів у меню [8].");
            return;
        }

        ConsoleUI.WriteInfo(
            $"Останні {feed.Length} дописів від ваших підписок:");
        DisplayPosts(feed);
        QuickInteract(feed);
    }

    private void ShowLastFivePosts()
    {
        User? selectedUser = SelectOtherUser();
        if (selectedUser == null)
            return;

        Post[] posts = selectedUser.GetLastPosts(5);

        ConsoleUI.WriteSection(
            $"🔍 ОСТАННІ {posts.Length} ДОПИСІВ {selectedUser.Nickname}");
        DisplayPosts(selectedUser, posts);
    }

    private void ReactToOtherPost()
    {
        User? selectedUser = SelectOtherUser();
        if (selectedUser == null)
            return;

        Post? selectedPost = SelectPost(selectedUser);
        if (selectedPost == null)
            return;

        ReactToPost(selectedUser, selectedPost);
    }

    private void ReactToPost(User author, Post post)
    {
        if (ReferenceEquals(author, _currentUser))
        {
            ConsoleUI.WriteWarning(
                "Реакції доступні лише до дописів іншого користувача.");
            return;
        }

        ConsoleUI.WriteSection($"РЕАКЦІЯ НА ДОПИС #{post.Id}");
        ConsoleUI.WriteMenuItem(1, "❤️  Подобається");
        ConsoleUI.WriteMenuItem(2, "🔥 Вогонь");
        ConsoleUI.WriteMenuItem(3, "😂 Смішно");
        ConsoleUI.WriteMenuItem(4, "👏 Браво");
        ConsoleUI.WriteMenuExit("Скасувати");

        int choice = ConsoleHelper.ReadIntInRange(
            "Оберіть реакцію: ", 0, 4);

        if (choice == 0)
            return;

        string emoji = choice switch
        {
            1 => "❤️",
            2 => "🔥",
            3 => "😂",
            _ => "👏"
        };

        if (post.TryAddReaction(_currentUser.Nickname, emoji))
        {
            ConsoleUI.WriteSuccess(
                $"{emoji} Реакцію додано! Уподобань: {post.Likes}.");
        }
        else
        {
            ConsoleUI.WriteWarning(
                $"Ви вже відреагували на цей допис: " +
                $"{post.GetReactionOf(_currentUser.Nickname)}.");
        }
    }

    private void CommentOtherPost()
    {
        User? selectedUser = SelectOtherUser();
        if (selectedUser == null)
            return;

        Post? selectedPost = SelectPost(selectedUser);
        if (selectedPost == null)
            return;

        CommentPost(selectedUser, selectedPost);
    }

    private void CommentPost(User author, Post post)
    {
        if (ReferenceEquals(author, _currentUser))
        {
            ConsoleUI.WriteWarning(
                "У цьому завданні коментуємо дописи інших користувачів.");
            return;
        }

        string commentText = ConsoleHelper.ReadText(
            "💬 Ваш коментар (до 160 символів): ", 160);

        post.AddComment(
            new Comment(_currentUser.Nickname, commentText, DateTime.Now));

        ConsoleUI.WriteSuccess(
            $"✅ Коментар додано до допису #{post.Id}!");
    }

    private void CreateOwnPost()
    {
        ConsoleUI.WriteSection("✍️  СТВОРИТИ ДОПИС");
        ConsoleUI.WriteInfo(
            "Можна використовувати смайлики 😊, хештеги #ООП та звичайний текст.");

        string text = ConsoleHelper.ReadText(
            "Ваш допис (до 280 символів): ", 280);

        Post post = _currentUser.CreatePost(text);

        ConsoleUI.WriteSuccess(
            $"✅ Допис #{post.Id} опубліковано від {_currentUser.Nickname}!");
        post.Display(_currentUser);
    }

    private void DeleteOwnPost()
    {
        Post[] myPosts = _currentUser.GetLastPosts(_currentUser.Posts.Count);

        if (myPosts.Length == 0)
        {
            ConsoleUI.WriteWarning("У вас немає дописів для видалення.");
            return;
        }

        ConsoleUI.WriteSection("🗑️  ВИДАЛИТИ СВІЙ ДОПИС");
        DisplayPosts(_currentUser, myPosts);

        int postId = ConsoleHelper.ReadIntInRange(
            "ID допису (0 — скасувати): ", 0, int.MaxValue);

        if (postId == 0)
            return;

        Post? post = _currentUser.FindPost(postId);

        if (post == null)
        {
            ConsoleUI.WriteWarning(
                "Такий допис вам не належить або не існує.");
            return;
        }

        ConsoleUI.WriteWarning(
            $"Підтвердити видалення допису #{post.Id}?");
        int confirm = ConsoleHelper.ReadIntInRange(
            "1 — так, 0 — ні: ", 0, 1);

        if (confirm == 1 && _currentUser.DeletePost(postId))
            ConsoleUI.WriteSuccess("✅ Допис видалено.");
        else
            ConsoleUI.WriteInfo("Видалення скасовано.");
    }

    private void ShowOwnProfile()
    {
        ConsoleUI.WriteSection("👤 МІЙ ПРОФІЛЬ");
        _currentUser.DisplayProfile();

        Console.WriteLine(
            $"   ⭐ Підписників: {CountFollowers(_currentUser)}");

        Post[] posts =
            _currentUser.GetLastPosts(_currentUser.Posts.Count);

        ConsoleUI.WriteSection("МОЇ ДОПИСИ");
        DisplayPosts(_currentUser, posts);
    }

    private void ManageSubscriptions()
    {
        ShowUsers();

        int selected = ConsoleHelper.ReadIntInRange(
            "Номер користувача (0 — назад): ",
            0,
            Users.Count);

        if (selected == 0)
            return;

        User other = Users[selected - 1];

        if (ReferenceEquals(other, _currentUser))
        {
            ConsoleUI.WriteWarning("На себе підписатися неможливо.");
            return;
        }

        ConsoleUI.WriteSection($"ПРОФІЛЬ {other.Nickname}");
        other.DisplayProfile();
        ConsoleUI.WriteLabel(
            "Підписників:",
            CountFollowers(other).ToString());

        string action = _currentUser.IsFollowing(other)
            ? "Відписатися"
            : "Підписатися";

        ConsoleUI.WriteMenuItem(1, action);
        ConsoleUI.WriteMenuExit("Повернутися");

        int choice = ConsoleHelper.ReadIntInRange(
            "Ваш вибір: ", 0, 1);

        if (choice == 0)
            return;

        if (_currentUser.IsFollowing(other))
        {
            _currentUser.Unfollow(other);
            ConsoleUI.WriteInfo(
                $"Ви відписалися від {other.Nickname}.");
        }
        else
        {
            _currentUser.Follow(other);
            ConsoleUI.WriteSuccess(
                $"✅ Тепер ви підписані на {other.Nickname}!");
        }
    }

    private void ChangeCurrentUser()
    {
        ShowUsers();

        int selected = ConsoleHelper.ReadIntInRange(
            "Оберіть номер користувача (0 — назад): ",
            0,
            Users.Count);

        if (selected == 0)
            return;

        _currentUser = Users[selected - 1];

        ConsoleUI.WriteSuccess(
            $"✅ Ви увійшли як {_currentUser.Nickname}.");
    }

    private void ShowTrending()
    {
        ConsoleUI.WriteSection("🔥 ПОПУЛЯРНІ ДОПИСИ");

        (User Author, Post Post)[] trending = GetTrending(5);

        ConsoleUI.WriteInfo("ТОП-5 за кількістю вподобань:");
        DisplayPosts(trending);
        QuickInteract(trending);
    }

    private void QuickInteract((User Author, Post Post)[] feed)
    {
        Console.WriteLine();
        ConsoleUI.WriteInfo(
            "Введіть ID чужого допису, щоб поставити реакцію або коментар.");

        int id = ConsoleHelper.ReadIntInRange(
            "ID допису (0 — назад): ", 0, int.MaxValue);

        if (id == 0)
            return;

        foreach ((User author, Post post) in feed)
        {
            if (post.Id != id)
                continue;

            if (ReferenceEquals(author, _currentUser))
            {
                ConsoleUI.WriteWarning(
                    "Це ваш допис. Для взаємодії оберіть чужий.");
                return;
            }

            ConsoleUI.WriteMenuItem(1, "❤️  Реакція");
            ConsoleUI.WriteMenuItem(2, "💬 Коментар");
            ConsoleUI.WriteMenuExit("Скасувати");

            int action = ConsoleHelper.ReadIntInRange(
                "Ваш вибір: ", 0, 2);

            if (action == 1)
                ReactToPost(author, post);
            else if (action == 2)
                CommentPost(author, post);

            return;
        }

        ConsoleUI.WriteWarning(
            "Допис із таким ID не знайдено у показаному списку.");
    }

    private User? SelectOtherUser()
    {
        while (true)
        {
            ShowUsers();

            int selected = ConsoleHelper.ReadIntInRange(
                "Номер іншого користувача (0 — назад): ",
                0,
                Users.Count);

            if (selected == 0)
                return null;

            User other = Users[selected - 1];

            if (!ReferenceEquals(other, _currentUser))
                return other;

            ConsoleUI.WriteWarning(
                "Оберіть іншого користувача, а не себе.");
        }
    }

    private Post? SelectPost(User author)
    {
        Post[] posts = author.GetLastPosts(author.Posts.Count);

        if (posts.Length == 0)
        {
            ConsoleUI.WriteWarning(
                "У цього користувача поки немає дописів.");
            return null;
        }

        DisplayPosts(author, posts);

        int id = ConsoleHelper.ReadIntInRange(
            "ID допису (0 — назад): ", 0, int.MaxValue);

        if (id == 0)
            return null;

        Post? post = author.FindPost(id);

        if (post == null)
            ConsoleUI.WriteWarning("Допис не знайдено.");

        return post;
    }

    private static void DisplayPosts(User owner, Post[] posts)
    {
        if (posts.Length == 0)
        {
            ConsoleUI.WriteInfo("Дописів поки немає.");
            return;
        }

        foreach (Post post in posts)
            post.Display(owner);

        Console.WriteLine("────────────────────────────────────────────────────────────");
    }

    private static void DisplayPosts((User Author, Post Post)[] posts)
    {
        if (posts.Length == 0)
        {
            ConsoleUI.WriteInfo("Дописів поки немає.");
            return;
        }

        foreach ((User author, Post post) in posts)
            post.Display(author);

        Console.WriteLine("────────────────────────────────────────────────────────────");
    }

    private static (User Author, Post Post)[] SortAndTake(
        List<(User Author, Post Post)> entries,
        int count)
    {
        entries.Sort((a, b) =>
        {
            int byDate = b.Post.CreatedAt.CompareTo(a.Post.CreatedAt);
            return byDate != 0 ? byDate : b.Post.Id.CompareTo(a.Post.Id);
        });

        return Take(entries, count);
    }

    private static (User Author, Post Post)[] Take(
        List<(User Author, Post Post)> entries,
        int count)
    {
        int length = Math.Min(entries.Count, count);
        (User Author, Post Post)[] result =
            new (User Author, Post Post)[length];

        for (int i = 0; i < length; i++)
            result[i] = entries[i];

        return result;
    }
}
