using System.Collections.Generic;

namespace FrameworkQ.Workflow;

public class WorkflowStep
{
    public string Name { get;  set; }

    
    private Dictionary<string, ActionConfig> _actions = new Dictionary<string, ActionConfig>();

    public Dictionary<string, ActionConfig> Actions
    {
        get { return _actions; }
        set { _actions = value; }
    }
}