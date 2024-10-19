using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        WorkflowContext context = new WorkflowContext();
        if (state.CurrentStepName != null)
        {
            context.Variables = state.Variables.ToDictionary(entry => entry.Key, entry => entry.Value);
            context.CurrentStepName = state.CurrentStepName;
            context.IsWorkflowComplete = state.IsWorkflowComplete;
            context.WorkflowId = state.WorkflowId;
        }
        
        return context;
    }
    
    public WorkflowState GetWorkflowState (WorkflowConfiguration config, WorkflowContext context)
    {
        if (context == null || context.CurrentStepName == null || !config.Steps.ContainsKey(context.CurrentStepName))
        {
            throw new ArgumentException("Invalid context or configuration.");
        }
        
        WorkflowState state = new WorkflowState
        {
            CurrentStepName = context.CurrentStepName,
            Variables = context.Variables.ToDictionary(entry => entry.Key, entry => entry.Value),
            IsWorkflowComplete = context.IsWorkflowComplete,
            WorkflowId = context.WorkflowId
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
        
        LoadActions(config, workflowConfig);
        LoadSteps(config, workflowConfig);

        return workflowConfig;
    }

    private  void LoadActions(dynamic config, WorkflowConfiguration workflowConfig)
    {
        foreach (var action in config.Actions)
        {
            ActionConfig act = new ActionConfig();
            System.Reflection.TypeInfo xtype = Type.GetType(action.Type.ToString());
            act.Type = new TypeInfo
            {
                ClassName = xtype.FullName,
                AssemblyName = xtype.Assembly.GetName().Name
            };
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
        if (context == null || context.CurrentStepName == null )
        {
            throw new ArgumentException("Invalid context or configuration.");
        }
        
        List<string> actions = new List<string>();
        
        if (context.IsWorkflowComplete)
        {
            return actions.ToArray();
        }
        
        foreach (var actionName in configuration.Steps[context.CurrentStepName].Actions)
        {
            var actionConfig = configuration.Actions[actionName.Value.MethodName];
            var validate = _actionFactory.CreateValidation(actionConfig.Type.ClassName + "," + actionConfig.Type.AssemblyName + "," + actionConfig.ValidationMethodName);
            
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

    public ActionResult ExecuteAction(WorkflowConfiguration configuration, WorkflowContext context, string actionVer)
    {
        if (context.IsWorkflowComplete)
        {
            throw new InvalidOperationException("Workflow is already complete.");
        }
        
        if (context == null || context.CurrentStepName == null || !configuration.Steps.ContainsKey(context.CurrentStepName))
        {
            throw new ArgumentException("Invalid context or configuration.");
        }
        
        if (!configuration.Steps[context.CurrentStepName].Actions.ContainsKey(actionVer))
        {
            throw new ArgumentException("Invalid action.");
        }
        
        var actionConfig = configuration.Actions[configuration.Steps[context.CurrentStepName].Actions[actionVer].MethodName];
        var invokeAction = _actionFactory.CreateAction(actionConfig.Type.ClassName + "," +
                                                       actionConfig.Type.AssemblyName + "," +
                                                       actionConfig.MethodName);
        WorkflowAction action = new WorkflowAction();
        action.Name = configuration.Steps[context.CurrentStepName].Actions[actionVer].MethodName;
        action.OnActionCall = invokeAction.Invoke;
        ActionResult result = action.Invoke(context);   
        
        if (result.IsSuccess)
        {
            context.CurrentStepName = configuration.Steps[result.ChangedStep].Name;
        }
        
        if (result.IsWorkflowComplete)
        {
            context.IsWorkflowComplete = true;
        }

        return result;
    }
}