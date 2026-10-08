using OOP_Lab4.Models;
using OOP_Lab4.Services;

namespace OOP_Lab4.Tests;

public static class SelfTests
{
    private static int _passed;
    private static int _failed;

    public static int Run()
    {
        _passed = 0;
        _failed = 0;

        Console.WriteLine("🧪 Перевірка програми OOP-Lab4");
        Console.WriteLine();

        Check("Випадковий склад і унікальні картки", TestInitialStore);
        Check("Магазин: ліміт кошика та баланс", TestRealisticStore);
        Check("Магазин: умови початкового завдання", TestAssignmentStore);
        Check("Сортування останніх п'яти дописів", TestLatestPosts);
        Check("Унікальні реакції та коментарі", TestReactionsAndComments);
        Check("Видалення лише своїх дописів", TestPostOwnership);
        Check("Підписки та персональна стрічка", TestFollowsAndFeed);
        Check("ТОП-5 популярних дописів", TestTrending);

        Console.WriteLine();
        Console.WriteLine(
            $"Підсумок: {_passed} успішно, {_failed} помилок.");

        return _failed == 0 ? 0 : 1;
    }

    private static void Check(string title, Action test)
    {
        try
        {
            test();
            _passed++;
            Console.WriteLine($"✅ {title}");
        }
        catch (Exception ex)
        {
            _failed++;
            Console.WriteLine($"❌ {title}: {ex.Message}");
        }
    }

    private static void TestInitialStore()
    {
        Random random = new Random(12345);
        Store store = new Store(random);

        Assert(store.Products.Length >= 20,
            "Асортимент повинен містити не менше 20 позицій.");

        foreach (Product product in store.Products)
        {
            Assert(product.InitialQuantity >= 40 &&
                   product.InitialQuantity <= 250,
                $"Невірний залишок товару {product.Name}.");
        }

        Customer[] customers = store.CreateCustomers(1000, random);

        Assert(customers.Select(c => c.CardNumber).Distinct().Count() == 1000,
            "Номери карток не є унікальними.");

        foreach (Customer customer in customers)
        {
            Assert(customer.InitialMoney >= 1000 &&
                   customer.InitialMoney <= 10000,
                "Початкова сума поза межами 1000–10000.");
            Assert(customer.ShoppingItemLimit >= 1 &&
                   customer.ShoppingItemLimit <= 15,
                "Неправильний ліміт кошика.");
        }
    }

    private static void TestRealisticStore()
    {
        Random random = new Random(6789);
        Store store = new Store(random);
        Customer[] customers = store.CreateCustomers(100, random);

        RunSilently(() => store.RunSimulation(customers, random, true));

        int items = 0;
        decimal expenses = 0;

        foreach (Customer customer in customers)
        {
            Assert(customer.PurchasedItems <= customer.ShoppingItemLimit,
                "Перевищено індивідуальний ліміт кошика.");
            Assert(customer.PurchasedItems <= 15,
                "Покупець придбав більше 15 одиниць.");
            Assert(customer.Money >= 0,
                "Від'ємний залишок коштів.");
            Assert(customer.InitialMoney - customer.Money ==
                   customer.SpentMoney,
                "Баланс покупця не збігається з витратами.");

            Assert(customer.Cart.Sum(p => p.Quantity) ==
                   customer.PurchasedItems,
                "Кількість одиниць у чеку не збігається.");
            Assert(customer.Cart.Sum(p => p.TotalPrice) ==
                   customer.SpentMoney,
                "Сума чека не збігається.");

            items += customer.PurchasedItems;
            expenses += customer.SpentMoney;
        }

        Assert(items == store.TotalItemsSold,
            "Кількість проданих товарів не збігається.");
        Assert(expenses == store.Profit,
            "Сума витрат покупців не дорівнює прибутку магазину.");
        Assert(store.PayingCustomers ==
               customers.Count(c => c.PurchasedItems > 0),
            "Неправильно підраховано покупців з покупками.");

        Assert(store.Products.Sum(p => p.SoldQuantity) ==
               store.TotalItemsSold,
            "Загальний продаж не збігається з рухом складу.");

        foreach (Product product in store.Products)
        {
            Assert(product.Quantity >= 0,
                "Залишок товару став від'ємним.");
            Assert(product.SoldQuantity + product.Quantity ==
                   product.InitialQuantity,
                "Порушено баланс залишків.");
        }
    }

    private static void TestAssignmentStore()
    {
        Random random = new Random(2468);
        Store store = new Store(random);
        Customer[] customers = store.CreateCustomers(1, random);

        RunSilently(() => store.RunSimulation(customers, random, false));

        Customer customer = customers[0];

        Assert(customer.PurchasedItems > 0,
            "Покупець нічого не придбав.");

        bool affordableProductExists =
            store.Products.Any(p => p.Quantity > 0 &&
                customer.CanBuy(p.Price));

        Assert(!affordableProductExists,
            "У режимі завдання покупки завершені завчасно.");
    }

    private static void TestLatestPosts()
    {
        User person = new User("Тест", "test@example.com", "@test");
        DateTime date = new DateTime(2026, 10, 1);

        person.CreatePost("Четвертий", date.AddDays(4), 0);
        person.CreatePost("Перший", date.AddDays(1), 0);
        person.CreatePost("Третій", date.AddDays(3), 0);
        person.CreatePost("Шостий", date.AddDays(6), 0);
        person.CreatePost("Другий", date.AddDays(2), 0);
        person.CreatePost("П'ятий", date.AddDays(5), 0);

        Post[] posts = person.GetLastPosts(5);

        Assert(posts.Length == 5, "Отримано не п'ять дописів.");
        Assert(posts[0].Text == "Шостий", "Порушено сортування за датою.");
        Assert(posts[4].Text == "Другий", "Неправильний п'ятий допис.");
    }

    private static void TestReactionsAndComments()
    {
        Post post = new Post("Новий допис 🎉", DateTime.Now, 3);
        int likes = post.Likes;

        Assert(post.TryAddReaction("@olena", "❤️"),
            "Першу реакцію не додано.");
        Assert(!post.TryAddReaction("@olena", "🔥"),
            "Повторна реакція одного користувача дозволена.");
        Assert(post.Likes == likes + 1,
            "Подвійний підрахунок вподобань.");

        post.AddComment(
            new Comment("@maksym", "Чудово! 👏", DateTime.Now));

        Assert(post.Comments.Count == 1,
            "Коментар не збережено.");
    }

    private static void TestPostOwnership()
    {
        User first = new User("Один", "one@example.com", "@one");
        User second = new User("Два", "two@example.com", "@two");

        Post post = first.CreatePost("Мій допис");

        Assert(!second.DeletePost(post.Id),
            "Інший користувач видалив чужий допис.");
        Assert(first.FindPost(post.Id) != null,
            "Чужий допис зник.");
        Assert(first.DeletePost(post.Id),
            "Власник не зміг видалити допис.");
        Assert(first.FindPost(post.Id) == null,
            "Видалений допис залишився у профілі.");
    }

    private static void TestFollowsAndFeed()
    {
        SocialNetwork network = new SocialNetwork();
        User viewer = network.Users[0];

        Assert(!viewer.Follow(viewer),
            "Користувач зміг підписатися на себе.");

        (User Author, Post Post)[] feed = network.GetFeed(viewer, 10);

        Assert(feed.Length == 10,
            "У початковій стрічці має бути 10 дописів.");

        foreach ((User author, Post post) in feed)
        {
            Assert(viewer.IsFollowing(author),
                "У стрічці є допис без підписки.");

            Assert(!ReferenceEquals(viewer, author),
                "Власний допис потрапив до стрічки підписок.");
        }

        for (int i = 1; i < feed.Length; i++)
        {
            Assert(feed[i - 1].Post.CreatedAt >= feed[i].Post.CreatedAt,
                "Стрічку не відсортовано за датою.");
        }

        User followed = network.Users[1];
        Assert(viewer.Unfollow(followed),
            "Не вдалося відписатися.");
        Assert(!viewer.IsFollowing(followed),
            "Підписка не була скасована.");
    }

    private static void TestTrending()
    {
        SocialNetwork network = new SocialNetwork();
        (User Author, Post Post)[] trending = network.GetTrending(5);

        Assert(trending.Length == 5,
            "Топ має містити п'ять дописів.");

        for (int i = 1; i < trending.Length; i++)
        {
            Assert(trending[i - 1].Post.Likes >= trending[i].Post.Likes,
                "Неправильне сортування за вподобаннями.");
        }
    }

    private static void RunSilently(Action action)
    {
        TextWriter previous = Console.Out;

        try
        {
            Console.SetOut(TextWriter.Null);
            action();
        }
        finally
        {
            Console.SetOut(previous);
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
