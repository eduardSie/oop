using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Text;
using System.Xml.Linq;

public class XslTransformer
{
    // Трансформация списка Student в HTML
    public void TransformToHtml(List<Student> students, string xslFilePath, string outputHtmlPath)
    {
        try
        {
            // Создаем временный XML из отфильтрованных результатов
            string tempXmlPath = Path.Combine(Path.GetTempPath(), $"filtered_{Guid.NewGuid()}.xml");

            // Генерируем XML из списка студентов
            CreateXmlFromStudents(students, tempXmlPath);

            // Выполняем трансформацию
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xslFilePath);

            using (XmlReader reader = XmlReader.Create(tempXmlPath))
            using (XmlWriter writer = XmlWriter.Create(outputHtmlPath, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            }))
            {
                xslt.Transform(reader, writer);
            }

            // Удаляем временный файл
            if (File.Exists(tempXmlPath))
                File.Delete(tempXmlPath);

            Console.WriteLine($"HTML файл успішно створено: {outputHtmlPath}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка трансформації: {ex.Message}", ex);
        }
    }

    // Создание XML из списка студентов
    private void CreateXmlFromStudents(List<Student> students, string xmlPath)
    {
        XDocument doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Students",
                students.Select(s => new XElement("Student",
                    new XAttribute("fullname", s.FullName ?? ""),
                    new XAttribute("faculty", s.Faculty ?? ""),
                    new XAttribute("department", s.Department ?? ""),
                    new XAttribute("subject", s.Subject ?? ""),
                    new XAttribute("grade", s.Grade),
                    new XAttribute("semester", s.Semester ?? "")
                ))
            )
        );

        doc.Save(xmlPath);
    }

    // Трансформация в строку HTML (опционально)
    public string TransformToHtmlString(List<Student> students, string xslFilePath)
    {
        try
        {
            string tempXmlPath = Path.Combine(Path.GetTempPath(), $"filtered_{Guid.NewGuid()}.xml");
            CreateXmlFromStudents(students, tempXmlPath);

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xslFilePath);

            using (StringWriter sw = new StringWriter())
            using (XmlReader reader = XmlReader.Create(tempXmlPath))
            using (XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            }))
            {
                xslt.Transform(reader, writer);

                // Удаляем временный файл
                if (File.Exists(tempXmlPath))
                    File.Delete(tempXmlPath);

                return sw.ToString();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка трансформації: {ex.Message}", ex);
        }
    }
}