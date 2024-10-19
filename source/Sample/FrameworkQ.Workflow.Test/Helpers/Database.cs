using System.Text;

namespace FrameworkQ.Workflow.Test.Helpers;

public class Database
{
    DirectoryInfo _directory;
    public Database()
    {
        
        // Initialize the database
        string folderLocation = Path.GetFullPath( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../DB"));
        _directory = new DirectoryInfo(folderLocation);
        Console.WriteLine(folderLocation);
    }
    
    public List<string> GetWorkflowStates()
    {
        return _directory.GetFiles().Select(x => x.Name).ToList();
    }

    public WorkflowConfiguration GetConfiguration()
    {
        Orchestrator orchestrator = new Orchestrator();
        string fileLocation = Path.GetFullPath( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../wwwroot/workflow.json"));
        using (FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
        {
            return orchestrator.LoadConfiguration(fs);
        }
    }

    public WorkflowInfo GetWorkflow(string workflowId)
    {
        WorkflowInfo workflowInfo = new WorkflowInfo();
        Orchestrator orchestrator = new Orchestrator();
        string fileLocation = Path.GetFullPath( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../wwwroot/workflow.json"));
        using (FileStream fs = new FileStream(fileLocation, FileMode.Open, FileAccess.Read))
        {
            workflowInfo.Configuration =  orchestrator.LoadConfiguration(fs);
            workflowInfo.Context = new WorkflowContext();
            workflowInfo.Context.Variables = new Dictionary<string, string>();
            workflowInfo.Context.WorkflowId = "";
            workflowInfo.WorkflowId = "";
            workflowInfo.Context.IsWorkflowComplete = false;
            workflowInfo.Context.CurrentStepName = workflowInfo.Configuration.Steps.First().Value.Name;
        }
        if (string.IsNullOrEmpty(workflowId) || workflowId == "0")
        {
            // with new context/state as not workflow id is provided
            workflowId = "";
            return workflowInfo;
        }
        else
        {
            // load existing workflow state
            string fileToRead = _directory.FullName + "/" + workflowId;
            WorkflowState state = new WorkflowState();
            state.Deserialize (File.OpenRead(fileToRead));
            var ctx= orchestrator.PrepareWorkflowContext(workflowInfo.Configuration, state);
            workflowInfo.Context = ctx;
            workflowInfo.WorkflowId = ctx.WorkflowId;
        }
        return workflowInfo;
    }


    public WorkflowContext SaveWorkflow(WorkflowConfiguration config, WorkflowContext context)
    {
        if ( context.WorkflowId == null || context.WorkflowId == "")
        {
            throw new ArgumentException("Invalid workflow id.");
        }
        
        Orchestrator orchestrator = new Orchestrator();
        var state = orchestrator.GetWorkflowState(config, context);
        string content = state.Serialize();
        
        var fileToDelete = _directory.FullName + "/" + context.WorkflowId;
        if (File.Exists(fileToDelete))
        {
            File.Delete(fileToDelete);
        }
    
        var fileToWrite = _directory.FullName + "/" + context.WorkflowId;
        File.WriteAllText(fileToWrite, content);
        return context;
    }
   
}