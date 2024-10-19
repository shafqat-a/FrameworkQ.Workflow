using System.Collections.Generic;

namespace FrameworkQ.Workflow;

public class WorkflowContext
{
    public string WorkflowId { get; set; }
    public string CurrentStepName { get;  set; }
    public bool IsWorkflowComplete { get; set; }
    public Dictionary<string, string> Variables { get; set; }
}