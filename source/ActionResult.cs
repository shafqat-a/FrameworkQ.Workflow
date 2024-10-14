namespace FrameworkQ.Workflow;

public class ActionResult
{
    public bool IsSuccess { get; set; }
    public bool IsWorkflowComplete { get; set; }
    public string FailureReason { get; set; }

    public string ChangedStep
    {
        get;
        set; 
    }

    public IDictionary<string, object> Results {
        get;
        set;
    }

}