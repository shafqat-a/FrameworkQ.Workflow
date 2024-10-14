namespace FrameworkQ.Workflow;

public class WorkflowAction: IAction
{
    public string Name { get; set; }
    public InvokeAction OnActionCall { get; set; }
    public PreValidateAction OnPreActionCall { get; set; }
    public ActionResult Invoke(WorkflowContext context)
    {
        return this.OnActionCall(context, this);
    }

    public PreActionValidationResult Validate(WorkflowContext context)
    {
        return this.OnPreActionCall(context, this);
    }
}