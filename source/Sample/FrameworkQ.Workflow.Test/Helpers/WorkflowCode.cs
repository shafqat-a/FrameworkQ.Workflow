namespace FrameworkQ.Workflow.Test.Helpers;

public class WorkflowCode
{
    public PreActionValidationResult ValidateAlwaysTrue(WorkflowContext context, IAction action)
    {
        return PreActionValidationResult.Success();
    }

    public PreActionValidationResult ValidateAlwaysFalse(WorkflowContext context, IAction action, string failureReason)
    {
        return PreActionValidationResult.Failed(failureReason);
    }

    public ActionResult InvokeAlwaysTrue(WorkflowContext context, IAction action)
    {
        return new ActionResult() { IsSuccess = true, Results = new Dictionary<string, object>() };
    }

    public ActionResult InvokeAlwaysFalse(WorkflowContext context, IAction action, string failureReason)
    {
        return new ActionResult()
        {
            IsSuccess = false, FailureReason = failureReason, Results = new Dictionary<string, object>()
        };
    }
    
    public PreActionValidationResult Validate_Step1(WorkflowContext context, IAction action)
    {
            return PreActionValidationResult.Success();
    }
    
    public ActionResult Workflow_Step1(WorkflowContext context, IAction action)
    {
        string firstName  = context.Variables["first_name"] ;
        string lastName  = context.Variables["last_name"] ;
        string email = context.Variables["email"];

        if (!"Step1".Equals(context.CurrentStepName))
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "Invalid step" };
        }
        
        ActionResult result = new ActionResult();
        result.Results = new Dictionary<string, object>();
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
        {
            if (string.IsNullOrEmpty(firstName))
            {
                result.Results.Add("first_name", "First name is required");
            }
            if (string.IsNullOrEmpty(lastName))
            {
                result.Results.Add("last_name", "Last name is required");
            }
            if (string.IsNullOrEmpty(email))
            {
                result.Results.Add("email", "Email is required");
            }
            
            result.IsSuccess = false;
            result.FailureReason = "One or more required fields missing";
        }
        else
        {
            result.IsSuccess = true;
            result.ChangedStep = "Step2";
        }

        return result;
    }
    
    public ActionResult Workflow_Step2(WorkflowContext context, IAction action)
    {
        if (!"Step2".Equals(context.CurrentStepName))
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "Invalid step" };
        }

        string agreeTerms = "";
        if (context.Variables.ContainsKey("agree_terms"))
        {
            agreeTerms = context.Variables["agree_terms"];
        }
        if (agreeTerms=="true")
        {
            return new ActionResult() { IsSuccess = true, ChangedStep = "Step3" };
        }
        else
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "You must agree to the terms" };
        }

    }

    public ActionResult Workflow_Step3_Complete(WorkflowContext context, IAction action)
    {
        if (!"Step3".Equals(context.CurrentStepName))
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "Invalid step" };
        }
       
        return new ActionResult() { IsSuccess = true, ChangedStep = "Step4", IsWorkflowComplete = true};
    }
    
    public ActionResult Workflow_Step3_Restart(WorkflowContext context, IAction action)
    {
        if (!"Step3".Equals(context.CurrentStepName))
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "Invalid step" };
        }
        context.Variables["first_name"] = "";
        context.Variables["last_name"] = "";
        context.Variables["email"] = "";
        context.Variables["agree_terms"] = "false";
        return new ActionResult() { IsSuccess = true, ChangedStep = "Step1"};
    }
    
    public ActionResult Workflow_Step3_Back(WorkflowContext context, IAction action)
    {
        if (!"Step3".Equals(context.CurrentStepName))
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "Invalid step" };
        }
        
        return new ActionResult() { IsSuccess = true, ChangedStep = "Step2"};
    }
    
    public ActionResult Workflow_Step2_Back(WorkflowContext context, IAction action)
    {
        if (!"Step2".Equals(context.CurrentStepName))
        {
            return new ActionResult() { IsSuccess = false, FailureReason = "Invalid step" };
        }
        
        return new ActionResult() { IsSuccess = true, ChangedStep = "Step1"};
    }

}