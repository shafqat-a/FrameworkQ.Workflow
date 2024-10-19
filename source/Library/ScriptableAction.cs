using System;

namespace FrameworkQ.Workflow;

public class ScriptableAction : IAction
{
    public string Name { get; set; }
    public InvokeAction OnActionCall { get; set; }
    public PreValidateAction OnPreActionCall { get; set; }
    public ActionResult Invoke(WorkflowContext context)
    {
        if (this.OnActionCall != null)
        {
            return OnActionCall(context, this);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public PreActionValidationResult Validate(WorkflowContext context)
    {
        if (this.OnPreActionCall != null)
        {
            return OnPreActionCall(context, this);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}