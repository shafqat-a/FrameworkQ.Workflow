namespace FrameworkQ.Workflow;

public interface IActionFactory
{
    InvokeAction CreateAction(string actionName);
    public InvokeValidation CreateValidation(string actionName);
}