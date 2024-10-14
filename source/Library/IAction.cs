namespace FrameworkQ.Workflow;

public interface IAction
{
    public string Name { get; set; }
    
    public InvokeAction OnActionCall { get; set; }
    public PreValidateAction OnPreActionCall { get; set; }
    
    ActionResult Invoke (WorkflowContext context);
    PreActionValidationResult Validate(WorkflowContext context);
}