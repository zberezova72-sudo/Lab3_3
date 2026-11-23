
using System.Text;
using Lab33.BLL.Exceptions;
using Lab33.BLL.Services;
using Lab33.BLL.Validation;
using Lab33.PL.Views;

namespace Lab33.PL;

public static class Menu
{
    private static readonly EntityService Service = new();

    // --- Головне меню ---
    public static void MainMenu()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        while (true)
        {
            PrintMenu();
            int choice = ConsoleIO.ReadChoice("Ваш вибір", 0, 12);
            if (choice == 0)
                return;

            // Усі винятки предметної області обробляються тут, на рівні представлення
            try
            {
                Execute(choice);
            }
            catch (InvalidPersonDataException ex)
            {
                Console.WriteLine("Помилки в даних:");
                foreach (string error in ex.Errors)
                    Console.WriteLine($"  - {error}");
            }
            catch (DomainException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
            catch (EndOfStreamException)
            {
                return;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Частина 2. База даних осіб ===");
        Console.WriteLine($"Поточний файл: {Service.CurrentFilePath ?? "не відкрито"}");
        Console.WriteLine(" 1. Створити нову БД");
        Console.WriteLine(" 2. Відкрити існуючу БД");
        Console.WriteLine(" 3. Закрити БД");
        Console.WriteLine(" 4. Видалити файл БД");
        Console.WriteLine(" 5. Додати особу (студент / музикант / пілот)");
        Console.WriteLine(" 6. Показати всіх осіб");
        Console.WriteLine(" 7. Пошук за прізвищем");
        Console.WriteLine(" 8. Пошук за унікальним ідентифікатором");
        Console.WriteLine(" 9. Видалити особу");
        Console.WriteLine("10. Покататися на ковзанах");
        Console.WriteLine("11. Кількість студенток 1-го курсу в гуртожитку (з файлу)");
        Console.WriteLine("12. Додати готовий набір даних");
        Console.WriteLine(" 0. Вихід");
    }

    private static void Execute(int choice)
    {
        switch (choice)
        {
            case 1: CreateDatabase(); break;
            case 2: OpenDatabase(); break;
            case 3: Service.CloseDatabase(); Console.WriteLine("БД закрито."); break;
            case 4: DeleteDatabase(); break;
            case 5: AddPerson(); break;
            case 6: ShowAll(); break;
            case 7: FindByLastName(); break;
            case 8: FindById(); break;
            case 9: RemovePerson(); break;
            case 10: Skate(); break;
            case 11: ShowFirstYearFemaleDormStudents(); break;
            case 12: Console.WriteLine($"Додано записів: {Service.AddSampleData()}."); break;
        }
    }

    // --- Керування файлом ---
    private static void CreateDatabase()
    {
        (StorageFormat format, string directory, string fileName) = ReadFileSettings();
        Service.CreateDatabase(directory, fileName, format);
        Console.WriteLine($"БД створено: {Service.CurrentFilePath}");
    }

    private static void OpenDatabase()
    {
        (StorageFormat format, string directory, string fileName) = ReadFileSettings();
        Service.OpenDatabase(directory, fileName, format);
        Console.WriteLine($"БД відкрито: {Service.CurrentFilePath}, записів: {Service.GetAll().Count}");
    }

    private static void DeleteDatabase()
    {
        string path = Service.CurrentFilePath ?? string.Empty;
        if (Service.IsDatabaseOpen && !ConsoleIO.Confirm($"Видалити файл «{path}» безповоротно?"))
            return;
        Service.DeleteDatabase();
        Console.WriteLine("Файл видалено.");
    }

    private static (StorageFormat, string, string) ReadFileSettings()
    {
        Console.WriteLine("Тип файлу:");
        Console.WriteLine("  1. Бінарний (.bin)");
        Console.WriteLine("  2. XML (.xml)");
        Console.WriteLine("  3. JSON (.json)");
        Console.WriteLine("  4. Користувацький текстовий (.txt)");
        var format = (StorageFormat)(ConsoleIO.ReadChoice("Вибір", 1, 4) - 1);
        string directory = ConsoleIO.ReadLine("Папка", "Data");
        string fileName = ConsoleIO.ReadLine("Ім'я файлу (розширення можна не вказувати)");
        return (format, directory, fileName);
    }

    // --- Додавання ---
    private static void AddPerson()
    {
        Console.WriteLine("Тип особи: 1. Студент  2. Музикант  3. Пілот");
        int type = ConsoleIO.ReadChoice("Вибір", 1, 3);

        PersonView view = type switch
        {
            1 => ReadStudent(),
            2 => ReadMusician(),
            _ => ReadPilot()
        };
        Service.Add(ViewMapper.ToModel(view));
        Console.WriteLine("Особу додано.");
    }

    private static StudentView ReadStudent() => new()
    {
        LastName = ConsoleIO.ReadField(Service, PersonField.LastName, "Прізвище"),
        FirstName = ConsoleIO.ReadField(Service, PersonField.FirstName, "Ім'я"),
        Course = int.Parse(ConsoleIO.ReadField(Service, PersonField.Course, "Курс (1-6)")),
        StudentCard = ConsoleIO.ReadField(Service, PersonField.StudentCard, "Студентський квиток (напр. KB123456)"),
        Gender = ConsoleIO.ReadChoice("Стать (1 - чоловіча, 2 - жіноча)", 1, 2) == 2 ? GenderView.Female : GenderView.Male,
        Residence = ConsoleIO.ReadField(Service, PersonField.Residence,
            "Місце проживання (гуртожиток — номер.кімната, напр. 5.312; інакше — адреса)")
    };

    private static MusicianView ReadMusician() => new()
    {
        LastName = ConsoleIO.ReadField(Service, PersonField.LastName, "Прізвище"),
        FirstName = ConsoleIO.ReadField(Service, PersonField.FirstName, "Ім'я"),
        MusicianId = ConsoleIO.ReadField(Service, PersonField.MusicianId, "Посвідчення музиканта (напр. MZ0001)"),
        Instrument = ConsoleIO.ReadField(Service, PersonField.Instrument, "Інструмент")
    };

    private static PilotView ReadPilot() => new()
    {
        LastName = ConsoleIO.ReadField(Service, PersonField.LastName, "Прізвище"),
        FirstName = ConsoleIO.ReadField(Service, PersonField.FirstName, "Ім'я"),
        PilotLicense = ConsoleIO.ReadField(Service, PersonField.PilotLicense, "Посвідчення пілота (напр. PL12345)"),
        FlightHours = int.Parse(ConsoleIO.ReadField(Service, PersonField.FlightHours, "Годин нальоту"))
    };

    // --- Перегляд і пошук ---
    private static void ShowAll() => Print("Усі особи", Service.GetAll().Select(ViewMapper.ToView));

    private static void FindByLastName()
    {
        string lastName = ConsoleIO.ReadLine("Прізвище");
        Print($"Результат пошуку за прізвищем «{lastName}»", Service.FindByLastName(lastName).Select(ViewMapper.ToView));
    }

    private static void FindById()
    {
        string id = ConsoleIO.ReadLine("Ідентифікатор (квиток / посвідчення)");
        Console.WriteLine(ViewMapper.ToView(Service.FindById(id)));
    }

    // --- Видалення та вміння ---
    private static void RemovePerson()
    {
        string id = ConsoleIO.ReadLine("Ідентифікатор особи для видалення");
        Service.Remove(id);
        Console.WriteLine("Особу видалено.");
    }

    private static void Skate()
    {
        string id = ConsoleIO.ReadLine("Ідентифікатор особи");
        Console.WriteLine(Service.Skate(id));
    }

    // --- Операція за варіантом ---
    private static void ShowFirstYearFemaleDormStudents()
    {
        var students = Service.GetFirstYearFemaleDormStudents();
        Console.WriteLine($"Кількість студенток 1-го курсу, які проживають у гуртожитку: {students.Count}");
        foreach (var student in students)
            Console.WriteLine("  " + ViewMapper.ToView(student));
    }

    // --- Допоміжний вивід ---
    private static void Print(string title, IEnumerable<PersonView> views)
    {
        Console.WriteLine($"--- {title} ---");
        int number = 0;
        foreach (PersonView view in views)
            Console.WriteLine($"{++number}. {view}");
        if (number == 0)
            Console.WriteLine("(нічого не знайдено)");
    }
}
