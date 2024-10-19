using FrameworkQ.Workflow.Test.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FrameworkQ.Workflow.Test.Controllers
{
    [ApiController]
    [Route("api/service")]
    public class ServiceController : ControllerBase
    {
        private Database _database;
        public ServiceController(Database database)
        {
            _database = database;
        }
        
        [HttpGet("workflows")]
        public IActionResult GetWorkflows()
        {
            return Ok(_database.GetWorkflowStates());
        }
        
        [HttpGet("workflow/{workflow_id}")]
        public IActionResult GetWorkflow(string workflow_id)
        {
            return Ok(_database.GetWorkflow(workflow_id));
        }
        
        [HttpPost("workflow/actions")]
        public IActionResult GetActionsForWorkflow([FromBody] WorkflowInfo data)
        {
            var config = _database.GetConfiguration();
            Orchestrator orchestrator = new Orchestrator();
            var actions =orchestrator.GetActionsAvailable(config, data.Context);
            return Ok(new { Received = actions });
        }
        
        [HttpPost("workflow/actions/exec")]
        public IActionResult ExecuteAction([FromBody] ActionInfo data)
        {
            var config = _database.GetConfiguration();
            Orchestrator orchestrator = new Orchestrator();
            var actions =orchestrator.ExecuteAction(config, data.WorkflowInfo.Context, data.Name);
            if (data.WorkflowInfo.WorkflowId == "")
            {
                data.WorkflowInfo.WorkflowId = Guid.NewGuid().ToString();
                data.WorkflowInfo.Context.WorkflowId = data.WorkflowInfo.WorkflowId;
                actions.WorkflowId = data.WorkflowInfo.WorkflowId;
            }
            // Save the workflow state
            _database.SaveWorkflow(config, data.WorkflowInfo.Context);
            
            return Ok(new { Received = actions });
        }
        
     

        [HttpPost]
        public IActionResult Post([FromBody] Dictionary<string, string> data)
        {
            // Process the data
            return Ok(new { Received = data });
        }
    }
}