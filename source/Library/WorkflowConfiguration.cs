using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FrameworkQ.Workflow;

public class WorkflowConfiguration
{
    private Dictionary<string, string> _parameters = new Dictionary<string, string>();
    private Dictionary<string, string> _variables = new Dictionary<string, string>();
    
    private Dictionary<string, WorkflowStep> _steps = new Dictionary<string, WorkflowStep>();
    private Dictionary<string, ActionConfig> _actions = new Dictionary<string, ActionConfig>();

    public Dictionary<string, string> Parameters
    {
        get { return _parameters; }
    }
    
    public Dictionary<string, string> Variables
    {
        get { return _variables; }
        set { _variables = value; }
    }
    
    public Dictionary<string,WorkflowStep> Steps
    {
        get { return _steps; }
    }
    
    public Dictionary<string,ActionConfig> Actions
    {
        get { return _actions; }
    }
    
}