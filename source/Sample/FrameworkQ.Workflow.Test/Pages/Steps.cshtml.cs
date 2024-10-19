using FrameworkQ.Workflow.Test.Controllers;
using FrameworkQ.Workflow.Test.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworkQ.Workflow.Test.Pages;

public class Steps : PageModel
{
    private ServiceController _controller;
    private string _workflowId;
    public Steps(ServiceController controller): base()
    {
        _controller = controller;
    }
    
    public void OnGet()
    {
        
        if (Request.Query.TryGetValue("workflow_id", out var workflowId))
        {
            try
            {
                this._workflowId = workflowId;
                if (this._workflowId != null)
                {
                    var result = _controller.GetWorkflow(workflowId) as OkObjectResult;
                    var workflwoInfo = result?.Value as WorkflowInfo;
                    if (workflwoInfo != null)
                    {
                        var stepname = workflwoInfo.Context.CurrentStepName;
                        if (workflwoInfo.Configuration.Steps.ContainsKey(stepname))
                        {
                            Response.Redirect($"/{stepname}?workflow_id={workflowId}");
                        }
                        else
                        {
                            // Start a new flow
                            Response.Redirect("/Step1");
                        }

                    }
                }
            }
            catch (Exception e)
            {
                // Start a new flow
                Response.Redirect("/Step1");
            }
            
        }
        
    }
}