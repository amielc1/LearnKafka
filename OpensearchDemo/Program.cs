using OpenSearch.Client;
using OpenSearch.Net;

namespace NetClientProgram;

public class Student
{
    public int Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int GradYear { get; init; }
    public double Gpa { get; init; }
}

internal class Program
{
    private static IOpenSearchClient osClient = new OpenSearchClient();

    public static void Main(string[] args)
    {
        Console.WriteLine("Indexing one student......");
        var student = new Student
        {
            Id = 100,
            FirstName = "Paulo",
            LastName = "Santos",
            Gpa = 3.93,
            GradYear = 2021
        };
        var response = osClient.Index(student, i => i.Index("students"));
        Console.WriteLine(response.IsValid ? "Response received" : "Error");

        Console.WriteLine("Searching for one student......");
        SearchForOneStudent();

        Console.WriteLine("Searching using low-level client......");
        SearchLowLevel();

        Console.WriteLine("Indexing an array of Student objects......");
        var studentArray = new Student[]
        {
            new() { Id = 200,
                    FirstName = "Shirley",
                    LastName = "Rodriguez",
                    Gpa = 3.91,
                    GradYear = 2019},
            new() { Id = 300,
                    FirstName = "Nikki",
                    LastName = "Wolf",
                    Gpa = 3.87,
                    GradYear = 2020}
        };
        var manyResponse = osClient.IndexMany(studentArray, "students");
        Console.WriteLine(manyResponse.IsValid ? "Response received" : "Error");
    }

    private static void SearchForOneStudent()
    {
        var searchResponse = osClient.Search<Student>(s => s
                                .Index("students")
                                .Query(q => q
                                    .Match(m => m
                                        .Field(fld => fld.LastName)
                                        .Query("Santos"))));

        PrintResponse(searchResponse);
    }

    //private static void SearchForAllStudentsWithANonEmptyLastName()
    //{
    //    var searchResponse = osClient.Search<Student>(s => s
    //                            .Index("students")
    //                            .Query(q => q
    //                                            .Bool(b => b
    //                                                .Must(m => m.Exists(fld => fld.))
    //                                                .MustNot(m => m.Term(t => t.Verbatim().Field(fld => fld.LastName).Value(string.Empty)))
    //                                            )));

    //    PrintResponse(searchResponse);
    //}

    private static void SearchLowLevel()
    {
        // Search for the student using the low-level client
        var lowLevelClient = osClient.LowLevel;

        var searchResponseLow = lowLevelClient.Search<SearchResponse<Student>>
            ("students",
            PostData.Serializable(
                new
                {
                    query = new
                    {
                        match = new
                        {
                            lastName = new
                            {
                                query = "Santos"
                            }
                        }
                    }
                }));

        PrintResponse(searchResponseLow);
    }

    private static void PrintResponse(ISearchResponse<Student> response)
    {
        if (response.IsValid)
        {
            foreach (var s in response.Documents)
            {
                Console.WriteLine($"{s.Id} {s.LastName} " +
                    $"{s.FirstName} {s.Gpa} {s.GradYear}");
            }
        }
        else
        {
            Console.WriteLine("Student not found.");
        }
    }
}