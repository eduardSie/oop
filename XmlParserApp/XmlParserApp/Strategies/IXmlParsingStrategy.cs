public interface IXmlParsingStrategy
{
    List<Student> ParseStudents(string filePath, Dictionary<string, string> searchCriteria);
    string GetStrategyName();
}