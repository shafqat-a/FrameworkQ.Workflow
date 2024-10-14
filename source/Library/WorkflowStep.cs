namespace FrameworkQ.Workflow;

public class WorkflowStep
{
    public string Name { get; internal set; }

    private Dictionary<string, string> _parameters = new Dictionary<string, string>();
    private Dictionary<string, string> _runtimeVariables = new Dictionary<string, string>();
    private Dictionary<string, ActionConfig> _actions = new Dictionary<string, ActionConfig>();
    public Dictionary<string, string> Parameters
    {
        get { return _parameters; }
    }
    
    public Dictionary<string, string> RuntimeVariables
    {
        get { return _runtimeVariables; }
    }

    public Dictionary<string, ActionConfig> Actions
    {
        get { return _actions; }
    }
}