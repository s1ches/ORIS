using MyProgram.Models;
using MyTemplates;

namespace MyProgram;

class Program
{
    static void Main()
    {
        var mapper = new Mapper();
        
        Console.WriteLine(mapper.Ex1("Лейсан"));
        Console.WriteLine(mapper.Ex2(new StudentEx2{Address = "Ул.Пушкина"}));
        Console.WriteLine(mapper.Ex3(new StudentEx3()));
    }
}