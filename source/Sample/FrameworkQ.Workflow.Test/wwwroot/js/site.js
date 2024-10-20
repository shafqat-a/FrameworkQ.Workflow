// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Services code
_ = console.log;
function getWorkflowById(workflow_id, fnSuccess, fnError) {
    strUrl = "/api/service/workflow/" + workflow_id;
    
    $.ajax({
        url: strUrl,
        type: "GET",
        contentType: "application/json",
        success: fnSuccess, error: fnError
    });
}

function getNewWorkflow(fnSuccess, fnError) {
    $.ajax({
        url: "/api/service/workflow",
        type: "GET",
        contentType: "application/json",
        success: fnSuccess, error: fnError
    });
}

function getActionVerbs(workflowInfo, fnSuccess, fnError) {
   
    $.ajax({
        url: "/api/service/workflow/actions",
        type: "POST",
        data: JSON.stringify(workflowInfo),
        contentType: "application/json",
        success: fnSuccess, error: fnError
    });
}


function loadWorkflow(workflowInfo) {
    _(["loadWorkflow",workflowInfo]);
   
    document.workflowInfo = workflowInfo;
    if (workflowInfo.context.variables != null) {
       $.each(workflowInfo.context.variables, function (key, value) {
           //_(["key is", key]);
            var inputFind = $('#' + key );
            //_(["inputFind", inputFind]);
            if (inputFind.length > 0) {
                if (inputFind.is(':checkbox')) {
                    //_(["Checkbox found", inputFind, inputFind.is(':checked')]);
                    if (value == "true") {
                        inputFind.prop('checked', true);
                    } else {
                        inputFind.prop('checked', false);
                    }
                } else if (inputFind.is(':text')) {
                   // _(["Setting value", inputFind, value]);
                    inputFind.val(value);
                }
            }
       });
    }
    
    // Action verbs code to only show if current step in on current page
    const pathname = window.location.pathname;
    const step = pathname.substring(1);
    _(["Step is ", step, workflowInfo.context.currentStepName, workflowInfo]);
    if (step == workflowInfo.context.currentStepName) {

        var actions = getActionVerbs(workflowInfo, function (response) {
            $("div_actions").html('');
            //_("actions are ", response.received);
            $.each(response.received, function (index, item) {
                var action_id = replaceAll(item, " ", "_");
                var action = '<button type="button" id="' + action_id + '" class="btn btn-primary" onclick="executeAction(\'' + item + '\')">' + item + '</button>&nbsp;';
                $("#div_actions").append(action);
            });
        });
    } else
    {
        // no action . just let users see
        var btn = '<button type="button" class="btn btn-primary" onclick="moveToCurrentStep(\''+ workflowInfo.workflowId  +'\');">Go to current step</button>&nbsp;';
        $("#div_actions").append(btn);
    }
}

function moveToCurrentStep (wfId) {
    window.setTimeout(function () {
        alert("Moving to current step");
        window.location.href = "/Steps?workflow_id=" + wfId;
    }, 1000);
}

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function loadVariablesFromPage(workflowInfo) {
    var inputFind = $('input[workflow_variable="true"]');
    $.each(inputFind, function (index, inputControl) {
       
        var key = $(inputControl).attr('id');
        var value = $(inputControl).val();
        if ($(inputControl).is(':checkbox')) {
            if ($(inputControl).is(':checked')) {
                value = "true";
            } else {
                value = "false";
            }
        }
        workflowInfo.context.variables[key] = value;
        
    });
}

function executeAction (action) {
    loadVariablesFromPage(document.workflowInfo);
    var workflowInfo = document.workflowInfo;
    var actioninfo = {"name":action, workflowInfo:document.workflowInfo};
    workflowInfo.context.action = action;
    $.ajax({
        url: "/api/service/workflow/actions/exec",
        type: "POST",
        data: JSON.stringify(actioninfo),
        contentType: "application/json",
        success: function (response) {
            processExecutionResult(response);
        }, error: function (error) {
            processExecutionError(error);
        }
    });
}

function forwardToWorkflowStep (workflow_id) {
    window.setTimeout(function () {
        //alert("Moving to next " + "/Steps?workflow_id=" + workflow_id);
        window.location.href = "/Steps?workflow_id=" + workflow_id;
    }, 2000);
}
function processExecutionResult (response) {
    if (response.received.isSuccess==true) {
        const url = new URL(window.location.href);
        var workflow_id = url.searchParams.get('workflow_id');
    
        forwardToWorkflowStep(workflow_id);        
    } else {
        processExecutionError(response);
    }
}
function processExecutionError(response){
    var text = response.received.failureReason;
    $.each(response.received.results, function (key, value) {
        text += "\n" + value;
    });
    alert(text);
}

$(document).ready(function () {

    _("Document ready");
    const url = new URL(window.location.href);
    var workflow_id = url.searchParams.get('workflow_id');

    if (workflow_id=="" || workflow_id==null) {
        workflow_id = 0;
    }
    _("Workflow ID: " + workflow_id);
    getWorkflowById(workflow_id, function (response) {
        // Resume workflow
        //_(["Workflow Info", response]);
        loadWorkflow(response);
    }, function (error) {
       // New Workflow
        console.log(error);
    });
});