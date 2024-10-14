using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FrameworkQ.Workflow;

public class Orchestrator
{
    private IActionFactory _actionFactory = new ActionFactory();

    public void SetFactory(IActionFactory factory)
    {
        _actionFactory = factory;
    }
    
    
    
    public WorkflowContext PrepareWorkflowContext(WorkflowConfiguration config, WorkflowState state)
    {
        //if (state == null || state.CurrentStep == null || !config.Steps.ContainsKey(state.CurrentStep))
        
        
        WorkflowContext context = new WorkflowContext();
        context.Configuration = config;
        if (state.CurrentStep != null)
        {
            context.CurrentStep = config.Steps[state.CurrentStep];
        }

        context.Variables = state.WorkflowVariables;
        
        return context;
    }
    
    public WorkflowState GetWorkflowState (WorkflowConfiguration config, WorkflowContext context)
    {
        if (context == null || context.CurrentStep == null || !config.Steps.ContainsKey(context.CurrentStep.Name))
        {
            throw new ArgumentException("Invalid context or configuration.");
        }
        
        WorkflowState state = new WorkflowState
        {
            WorkflowVariables = context.Variables,
            CurrentStep = context.CurrentStep.Name,
            RuntimeVariables = config.Steps[context.CurrentStep.Name].RuntimeVariables
        };
        
        return state;
    }
    
    
    public  WorkflowConfiguration LoadConfiguration(Stream jsonContentStream)
    {
        string jsonContent = null;
        using (StreamReader reader = new StreamReader(jsonContentStream))
        {
             jsonContent = reader.ReadToEnd();
        }

        dynamic config = JsonConvert.DeserializeObject<dynamic>(jsonContent);
        WorkflowConfiguration workflowConfig = new WorkflowConfiguration();
        workflowConfig.Name = config.Name;

        
        LoadActions(config, workflowConfig);
        LoadSteps(config, workflowConfig);
        //UpdateStepActions(workflowConfig);  

        return workflowConfig;
    }

    private  void LoadActions(dynamic config, WorkflowConfiguration workflowConfig)
    {
        foreach (var action in config.Actions)
        {
            ActionConfig act = new ActionConfig();
            act.Type = Type.GetType(action.Type.ToString());
            act.MethodName = action.Name.ToString();
            act.ValidationMethodName = action.Validation.ToString();
            workflowConfig.Actions.Add(act.MethodName, act);
        }
    }

    private  void UpdateStepActions(WorkflowConfiguration workflowConfig)
    {
        foreach (var step in workflowConfig.Steps.Values)
        {
            foreach (var actionName in step.Actions.Keys.ToList())
            {
                if (workflowConfig.Actions.TryGetValue(actionName, out var actionConfig))
                {

                    step.Actions[actionName] = actionConfig;
                }
                else
                {
                    step.Actions.Remove(actionName);
                }
            }
        }
    }
    private  void LoadSteps(dynamic config, WorkflowConfiguration workflowConfig)
    {
        foreach (var step in config.Steps)
        {
            WorkflowStep workflowStep = new WorkflowStep();
            workflowStep.Name = step.Name;
            var enumerable = step.Variables;
            foreach (var variable in enumerable)
            {
                workflowStep.Parameters.Add(variable.Name.ToString(), variable.Value.ToString());
            }
            
            JArray actionsArray = (JArray)step["Actions"];
            foreach (JObject action in actionsArray)
            {
                foreach (var property in action.Properties())
                {
                    ActionConfig act = workflowConfig.Actions[property.Value.ToString()];
                    workflowStep.Actions.Add(property.Name, act);
                }
            }
            
            workflowConfig.Steps.Add(workflowStep.Name, workflowStep);
        }
    }

    public string[] GetActionsAvailable(WorkflowConfiguration configuration, WorkflowContext context)
    {
        if (context == null || context.CurrentStep == null || !configuration.Steps.ContainsKey(context.CurrentStep.Name))
        {
            throw new ArgumentException("Invalid context or configuration.");
        }
        
        List<string> actions = new List<string>();
        
        if (context.IsWorkflowComplete)
        {
            return actions.ToArray();
        }
        
        foreach (var actionName in context.CurrentStep.Actions)
        {
            var actionConfig = configuration.Actions[actionName.Value.MethodName];
            var validate = _actionFactory.CreateValidation(actionConfig.Type.FullName + "," + actionConfig.Type.Assembly.GetName().Name + "," + actionConfig.ValidationMethodName);
            
            WorkflowAction action = new WorkflowAction();
            action.Name = actionName.Key;
            action.OnPreActionCall = validate.Invoke;
            PreActionValidationResult result = action.Validate(context);
            if (result.IsSuccess)
            {
                actions.Add(actionName.Key);
            }
        }
        
        return actions.ToArray();
    }

    public bool ExecuteAction(WorkflowConfiguration configuration, WorkflowContext context, string actionVer)
    {
        if (context.IsWorkflowComplete)
        {
            throw new InvalidOperationException("Workflow is already complete.");
        }
        
        if (context == null || context.CurrentStep == null || !configuration.Steps.ContainsKey(context.CurrentStep.Name))
        {
            throw new ArgumentException("Invalid context or configuration.");
        }
        
        if (!context.CurrentStep.Actions.ContainsKey(actionVer))
        {
            throw new ArgumentException("Invalid action.");
        }
        
        var actionConfig = configuration.Actions[context.CurrentStep.Actions[actionVer].MethodName];
        var invokeAction = _actionFactory.CreateAction(actionConfig.Type.FullName + "," +
                                                       actionConfig.Type.Assembly.GetName().Name + "," +
                                                       actionConfig.MethodName);
        WorkflowAction action = new WorkflowAction();
        action.Name = context.CurrentStep.Actions[actionVer].MethodName;
        action.OnActionCall = invokeAction.Invoke;
        ActionResult result = action.Invoke(context);   
        if (result.IsSuccess)
        {
            context.CurrentStep = configuration.Steps[result.ChangedStep];
        }
        
        if (result.IsWorkflowComplete)
        {
            context.IsWorkflowComplete = true;
        }
        
        return !result.IsWorkflowComplete;
    }
}