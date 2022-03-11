using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ResponseFunctionDebugComponent: VisualElement
{
    private Label typeLabel;
    private VisualElement body;
    public ResponseFunctionDebugComponent(ResponseFunctionDebug rf)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        typeLabel = root.Q<Label>("Type-Label");
        body = root.Q<VisualElement>("Body");
        typeLabel.text = rf.Type.ToString();

        foreach(var p in rf.Parameters.Parameters)
        {
            var pComp = new ParameterDebugComponent(p);
            body.Add(pComp);
        }
    }
}