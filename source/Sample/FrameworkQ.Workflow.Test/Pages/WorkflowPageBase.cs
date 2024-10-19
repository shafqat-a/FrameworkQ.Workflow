using FrameworkQ.Workflow.Test.Controllers;
using FrameworkQ.Workflow.Test.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrameworkQ.Workflow.Test.Pages;

public class WorkflowPageBase : PageModel
{
    
    public string WorkflowId { get; set; }
    
    public void OnGet()
    {
        if (Request.Query.TryGetValue("workflow_id", out var workflowId))
        {
            
        }
        
        
        
    }
}