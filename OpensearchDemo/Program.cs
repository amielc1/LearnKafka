using OpensearchDemo;

namespace NetClientProgram;
  
internal class Program
{ 
    public static void Main(string[] args)
    {
        CrudOperations crudOperations = new CrudOperations();

        var students = crudOperations.GenerateStudents();
        crudOperations.Create(students);

        var allStudents = crudOperations.ReadAll();

        crudOperations.Create(new Student() { FirstName = "Amiel",LastName = "Cohen", Gpa = 99.5,GradYear = 2585,Id = 123});
        allStudents = crudOperations.ReadAll();
        var students1 = crudOperations.Read("Amiel");

        crudOperations.Update(new Student() { FirstName = "Daniel", LastName = "Cohen", Gpa = 99.5, GradYear = 2585, Id = 123 });
        allStudents = crudOperations.ReadAll();

        crudOperations.Delete(600);
        allStudents = crudOperations.ReadAll();
        Console.WriteLine("Indexing one student......"); 
    }
     
}