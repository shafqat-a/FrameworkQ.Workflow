namespace FrameworkQ.Workflow;

public class WorkflowContext
{
    public WorkflowConfiguration Configuration { get;  set; }
    public WorkflowStep CurrentStep { get;  set; }
    public Dictionary<string, string> Variables{ get;  set; }
    
    public bool IsWorkflowComplete { get; set; }
}