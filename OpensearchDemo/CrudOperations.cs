using OpenSearch.Client;

namespace OpensearchDemo;

public class CrudOperations
{
    private IOpenSearchClient osClient;
    public CrudOperations()
    {
        osClient = new OpenSearchClient();
    }
         
    public bool Create(Student student)
    {
        var response = osClient.Index(student, i => i.Index("students"));
        return response.IsValid;
    }

    public bool Create(List<Student> students)
    {
        var response = osClient.IndexMany(students, "students");
        return response.IsValid;
    }
    public List<Student> Read(string fname)
    {
        var searchResponse = osClient.Search<Student>(s => s
            .Index("students")
            .Query(q => q
                .Match(m => m
                    .Field(fld => fld.FirstName)
                    .Query(fname))));
        return searchResponse.Documents.ToList();
    }

    public List<Student> ReadAll()
    {
        return osClient.Search<Student>(s => s
            .Index("students")).Documents.ToList(); ;
    }

    public bool Delete(int id
    )
    {
        var response = osClient.Delete(new DeleteRequest("students", id));
        return response.IsValid;
    }

    public bool Update(Student student)
    {
        var response = osClient.Update<Student>(student.Id, u => u
            .Index("students")
            .Doc(student)
            .DocAsUpsert(true)); // Set to true to insert if the document does not exist.
        return response.IsValid;
    }



    public List<Student> GenerateStudents()
    {
        return new List<Student>
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
            ,
            new() { Id = 400,
                FirstName = "Nikki",
                LastName = "Wolf",
                Gpa = 3.87,
                GradYear = 2020}
            ,
            new() { Id = 500,
                FirstName = "Nikki",
                LastName = "Wolf",
                Gpa = 3.87,
                GradYear = 2020}
            ,
            new() { Id = 600,
                FirstName = "Amie",
                LastName = "Cohen",
                Gpa = 8.87,
                GradYear = 2024}
        };
    }

}