using System.Xml;

public class DomParsingStrategy : IXmlParsingStrategy
{
    public string GetStrategyName() => "DOM (XmlDocument)";

    public List<Student> ParseStudents(string filePath, Dictionary<string, string> searchCriteria)
    {
        var results = new List<Student>();
        var doc = new XmlDocument();
        doc.Load(filePath);

        XmlNodeList studentNodes = doc.SelectNodes("//Student");

        foreach (XmlNode node in studentNodes)
        {
            var student = new Student
            {
                // Атрибути
                Faculty = node.Attributes?["faculty"]?.Value ?? "",
                Department = node.Attributes?["department"]?.Value ?? "",
                Semester = node.Attributes?["semester"]?.Value ?? "",

                // Елементи
                FullName = node.SelectSingleNode("FullName")?.InnerText ?? "",
                Subject = node.SelectSingleNode("Subject")?.InnerText ?? "",
                Grade = int.TryParse(node.SelectSingleNode("Grade")?.InnerText, out int g) ? g : 0
            };

            if (MatchesCriteria(student, searchCriteria))
                results.Add(student);
        }

        return results;
    }

    private bool MatchesCriteria(Student student, Dictionary<string, string> criteria)
    {
        foreach (var criterion in criteria)
        {
            var value = criterion.Key switch
            {
                "Faculty" => student.Faculty,
                "Department" => student.Department,
                "Subject" => student.Subject,
                "Semester" => student.Semester,
                _ => ""
            };

            if (!string.IsNullOrEmpty(criterion.Value) &&
                !value?.Contains(criterion.Value, StringComparison.OrdinalIgnoreCase) == true)
                return false;
        }
        return true;
    }
}
