namespace FrameworkQ.Workflow;

public class TypeInfo
{
    public string ClassName { get; set; }
    public string AssemblyName { get; set; }
    
    override public string ToString()
    {
        return $"{ClassName}, {AssemblyName}";
    }
}