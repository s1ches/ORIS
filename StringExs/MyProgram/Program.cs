using MyProgram.Models;
using MyTemplates;


namespace MyProgram;

class Program
{
    static void Main()
    {
        var make = new Worker();
        
        Console.WriteLine(make.Ex1("Лейсан"));
        Console.WriteLine(make.Ex2(new StudentEx2{Address = "Ул.Пушкина"}));
        Console.WriteLine(make.Ex3(new StudentEx3()));

        var table = new Table();
        table.Students.Add(new StudentEx4{FIO = "Лейсан Нонская" , Grade = 0});
        table.Students.Add(new StudentEx4{FIO = "Никита Евстягин" , Grade = 100});
        table.Students.Add(new StudentEx4{FIO = "Иван Сосорин" , Grade = 71});
        table.Students.Add(new StudentEx4{FIO = "Лев Коснырев" , Grade = 56});

        Console.WriteLine(make.Ex4(table));
        
    }
}