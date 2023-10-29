using MyProgram.Models;
using MyTemplates;


namespace MyProgram;

class Program
{
    static void Main()
    {
        var mapper = new ClassWork();
        
        Console.WriteLine(mapper.Ex1("Лейсан"));
        Console.WriteLine(mapper.Ex2(new StudentEx2{Address = "Ул.Пушкина"}));
        Console.WriteLine(mapper.Ex3(new StudentEx3()));

        var table = new Table();
        table.Students.Add(new StudentEx4{FIO = "Лейсан Нонская" , Grade = 0});
        table.Students.Add(new StudentEx4{FIO = "Никита Евстягин" , Grade = 100});
        table.Students.Add(new StudentEx4{FIO = "Иван Сосорин" , Grade = 71});
        table.Students.Add(new StudentEx4{FIO = "Лев Коснырев" , Grade = 56});

        Console.WriteLine(mapper.Ex4(table));
        
    }
}