using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace FrameworkQ.Workflow;

public class WorkflowState
{
    public string WorkflowId { get; set; }
   
    public string CurrentStepName { get;  set; }
    public Dictionary<string,string> Variables { get; set; } 
    
    public bool IsWorkflowComplete { get; set; }
    
    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }   
    
    public void Deserialize(Stream stream)
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            string json = reader.ReadToEnd();
            Deserialize(json);
        }
    }
    
    public void Deserialize(string json)
    {
        var state = JsonConvert.DeserializeObject<WorkflowState>(json);
        WorkflowId = state.WorkflowId;
        CurrentStepName = state.CurrentStepName;
        Variables = state.Variables.ToDictionary(x => x.Key, x => x.Value);
        IsWorkflowComplete = state.IsWorkflowComplete;
    }
}