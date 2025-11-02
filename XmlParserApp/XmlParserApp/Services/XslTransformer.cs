using System.Xml;
using System.Xml.Xsl;
using System.IO;

public class XslTransformer
{
    public void TransformToHtml(string xmlFilePath, string xslFilePath, string outputHtmlPath)
    {
        try
        {
            // Створюємо трансформатор
            XslCompiledTransform xslt = new XslCompiledTransform();

            // Завантажуємо XSL
            xslt.Load(xslFilePath);

            // Виконуємо трансформацію
            using (XmlReader reader = XmlReader.Create(xmlFilePath))
            using (XmlWriter writer = XmlWriter.Create(outputHtmlPath, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            }))
            {
                xslt.Transform(reader, writer);
            }

            Console.WriteLine($"HTML файл успішно створено: {outputHtmlPath}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка трансформації: {ex.Message}", ex);
        }
    }

    public string TransformToHtmlString(string xmlFilePath, string xslFilePath)
    {
        try
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xslFilePath);

            using (StringWriter sw = new StringWriter())
            using (XmlReader reader = XmlReader.Create(xmlFilePath))
            using (XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            }))
            {
                xslt.Transform(reader, writer);
                return sw.ToString();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка трансформації: {ex.Message}", ex);
        }
    }
}