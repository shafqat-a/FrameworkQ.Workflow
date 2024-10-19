using System;
using System.Collections.Generic;
using System.Reflection;

namespace FrameworkQ.Workflow;

public class ActionFactory: IActionFactory
{
    private Dictionary<string, InvokeAction> _dictionary = new Dictionary<string, InvokeAction>();
    
    /// <summary>
    /// Expects full typename, assemblyname, methodname in action - unless customized
    /// </summary>
    /// <param name="actionName"></param>
    /// <returns></returns>
    public InvokeAction CreateAction(string actionName)
    {
        return CreateActionDelegate<InvokeAction>(actionName);
    }

    
    public InvokeValidation CreateValidation(string actionName)
    {
        return CreateActionDelegate<InvokeValidation>(actionName);
    }


    private T CreateActionDelegate<T>(string actionName) where T: System.Delegate
    {
        string[] items = actionName.Split(",");
        string fullTypename = items[0] + "," + items[1];
        Type type = Type.GetType(fullTypename);
        if (type == null)
        {
            throw new ArgumentException("Type not found: " + fullTypename);
        }
        MethodInfo methodInfo = type.GetMethod(items[2], BindingFlags.Instance | BindingFlags.Public);
        object instance = Activator.CreateInstance(type);
        T action = (T) Delegate.CreateDelegate(typeof(T), instance, methodInfo);
        return action;
    }

 
}