using System.Xml;

public class SaxParsingStrategy : IXmlParsingStrategy
{
    public string GetStrategyName() => "SAX (XmlReader)";

    public List<Student> ParseStudents(string filePath, Dictionary<string, string> searchCriteria)
    {
        var results = new List<Student>();
        Student current = null;
        string currentElement = "";

        using (XmlReader reader = XmlReader.Create(filePath))
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        currentElement = reader.Name;

                        if (currentElement == "Student")
                        {
                            current = new Student();

                            // Читаємо атрибути (різна кількість!)
                            if (reader.HasAttributes)
                            {
                                while (reader.MoveToNextAttribute())
                                {
                                    switch (reader.Name)
                                    {
                                        case "faculty":
                                            current.Faculty = reader.Value;
                                            break;
                                        case "department":
                                            current.Department = reader.Value;
                                            break;
                                        case "semester":
                                            current.Semester = reader.Value;
                                            break;
                                    }
                                }
                                reader.MoveToElement();
                            }
                        }
                        break;

                    case XmlNodeType.Text:
                        if (current != null)
                        {
                            switch (currentElement)
                            {
                                case "FullName":
                                    current.FullName = reader.Value;
                                    break;
                                case "Subject":
                                    current.Subject = reader.Value;
                                    break;
                                case "Grade":
                                    current.Grade = int.Parse(reader.Value);
                                    break;
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (reader.Name == "Student" && current != null)
                        {
                            if (MatchesCriteria(current, searchCriteria))
                                results.Add(current);
                            current = null;
                        }
                        break;
                }
            }
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
