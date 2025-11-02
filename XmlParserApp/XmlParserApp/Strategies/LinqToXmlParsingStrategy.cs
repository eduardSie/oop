using System.Xml.Linq;

public class LinqToXmlParsingStrategy : IXmlParsingStrategy
{
    public string GetStrategyName() => "LINQ to XML";

    public List<Student> ParseStudents(string filePath, Dictionary<string, string> searchCriteria)
    {
        var doc = XDocument.Load(filePath);

        var query = from student in doc.Descendants("Student")
                    let faculty = student.Attribute("faculty")?.Value ?? ""
                    let department = student.Attribute("department")?.Value ?? ""
                    let semester = student.Attribute("semester")?.Value ?? ""
                    let fullName = student.Element("FullName")?.Value ?? ""
                    let subject = student.Element("Subject")?.Value ?? ""
                    let grade = int.TryParse(student.Element("Grade")?.Value, out int g) ? g : 0
                    where MatchesCriteria(faculty, department, subject, semester, searchCriteria)
                    select new Student
                    {
                        Faculty = faculty,
                        Department = department,
                        Semester = semester,
                        FullName = fullName,
                        Subject = subject,
                        Grade = grade
                    };

        return query.ToList();
    }

    private bool MatchesCriteria(string faculty, string department, string subject,
                                 string semester, Dictionary<string, string> criteria)
    {
        foreach (var criterion in criteria)
        {
            var value = criterion.Key switch
            {
                "Faculty" => faculty,
                "Department" => department,
                "Subject" => subject,
                "Semester" => semester,
                _ => ""
            };

            if (!string.IsNullOrEmpty(criterion.Value) &&
                !value?.Contains(criterion.Value, StringComparison.OrdinalIgnoreCase) == true)
                return false;
        }
        return true;
    }
}
