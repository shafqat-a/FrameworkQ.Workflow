namespace FrameworkQ.Workflow;

public class PreActionValidationResult
{
    public bool IsSuccess { get; set; }
    public string FailureReason { get; set; }

    public static PreActionValidationResult Success()
    {
        PreActionValidationResult result = new PreActionValidationResult();
        result.IsSuccess = true;
        return result;
    }

    public static PreActionValidationResult Failed(string reason)
    {
        PreActionValidationResult result = new PreActionValidationResult();
        result.IsSuccess = false;
        result.FailureReason = reason;
        return result;
    }
}