public class Student
{
    public string FullName { get; set; }
    public string Faculty { get; set; }
    public string Department { get; set; }
    public string Subject { get; set; }
    public int Grade { get; set; }
    public string Semester { get; set; }

    public override string ToString()
    {
        return $"{FullName} - {Faculty} - {Subject}: {Grade}";
    }
}