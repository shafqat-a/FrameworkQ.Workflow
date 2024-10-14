namespace FrameworkQ.Workflow;

public class WorkflowConfiguration
{
    public string Name { get; set; }
    
    private Dictionary<string, WorkflowStep> _steps = new Dictionary<string, WorkflowStep>();
    private Dictionary<string, ActionConfig> _actions = new Dictionary<string, ActionConfig>();

    public Dictionary<string,WorkflowStep> Steps
    {
        get { return _steps; }
    }
    
    public Dictionary<string,ActionConfig> Actions
    {
        get { return _actions; }
    }
    
}