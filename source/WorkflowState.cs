namespace FrameworkQ.Workflow;

public class WorkflowState
{
    public Dictionary<string, string> WorkflowVariables{ get;  set; }
    public string CurrentStep { get;  set; }
    public Dictionary<string,string> RuntimeVariables { get; set; } 
    
}