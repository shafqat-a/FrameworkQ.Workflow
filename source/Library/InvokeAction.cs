namespace FrameworkQ.Workflow;

public delegate ActionResult InvokeAction(WorkflowContext context, IAction action);
public delegate PreActionValidationResult InvokeValidation(WorkflowContext context, IAction action);