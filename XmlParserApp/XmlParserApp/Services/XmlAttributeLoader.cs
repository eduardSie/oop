using System.Xml.Linq;

// Клас для динамічного завантаження значень атрибутів з XML файлу
public class XmlAttributeLoader
{
    public Dictionary<string, HashSet<string>> LoadAvailableAttributes(string xmlFilePath)
    {
        var attributes = new Dictionary<string, HashSet<string>>();

        try
        {
            var doc = XDocument.Load(xmlFilePath);
            var students = doc.Descendants("Student");

            foreach (var student in students)
            {
                // Збираємо всі атрибути
                foreach (var attr in student.Attributes())
                {
                    if (!attributes.ContainsKey(attr.Name.LocalName))
                        attributes[attr.Name.LocalName] = new HashSet<string>();

                    if (!string.IsNullOrWhiteSpace(attr.Value))
                        attributes[attr.Name.LocalName].Add(attr.Value);
                }

                // Збираємо значення з елементів, які можуть бути атрибутами
                var faculty = student.Element("Faculty")?.Value;
                if (!string.IsNullOrWhiteSpace(faculty))
                {
                    if (!attributes.ContainsKey("faculty"))
                        attributes["faculty"] = new HashSet<string>();
                    attributes["faculty"].Add(faculty);
                }

                var department = student.Element("Department")?.Value;
                if (!string.IsNullOrWhiteSpace(department))
                {
                    if (!attributes.ContainsKey("department"))
                        attributes["department"] = new HashSet<string>();
                    attributes["department"].Add(department);
                }

                var semester = student.Element("Semester")?.Value;
                if (!string.IsNullOrWhiteSpace(semester))
                {
                    if (!attributes.ContainsKey("semester"))
                        attributes["semester"] = new HashSet<string>();
                    attributes["semester"].Add(semester);
                }

                var subject = student.Element("Subject")?.Value;
                if (!string.IsNullOrWhiteSpace(subject))
                {
                    if (!attributes.ContainsKey("subject"))
                        attributes["subject"] = new HashSet<string>();
                    attributes["subject"].Add(subject);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка завантаження атрибутів: {ex.Message}", ex);
        }

        return attributes;
    }

    public List<string> GetUniqueValues(string xmlFilePath, string attributeName)
    {
        var values = new HashSet<string>();

        try
        {
            var doc = XDocument.Load(xmlFilePath);
            var students = doc.Descendants("Student");

            foreach (var student in students)
            {
                // Спочатку перевіряємо атрибути
                var attrValue = student.Attribute(attributeName)?.Value;
                if (!string.IsNullOrWhiteSpace(attrValue))
                    values.Add(attrValue);

                // Потім перевіряємо елементи (з великої літери)
                var capitalizedName = char.ToUpper(attributeName[0]) + attributeName.Substring(1);
                var elemValue = student.Element(capitalizedName)?.Value;
                if (!string.IsNullOrWhiteSpace(elemValue))
                    values.Add(elemValue);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка отримання значень: {ex.Message}", ex);
        }

        return values.OrderBy(v => v).ToList();
    }
}