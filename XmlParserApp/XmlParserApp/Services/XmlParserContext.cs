public class XmlParserContext
{
    private IXmlParsingStrategy _strategy;

    public void SetStrategy(IXmlParsingStrategy strategy)
    {
        _strategy = strategy;
    }

    public List<Student> ExecuteParsing(string filePath, Dictionary<string, string> searchCriteria)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Strategy not set");

        return _strategy.ParseStudents(filePath, searchCriteria);
    }

    public string GetCurrentStrategyName()
    {
        return _strategy?.GetStrategyName() ?? "No strategy";
    }
}
