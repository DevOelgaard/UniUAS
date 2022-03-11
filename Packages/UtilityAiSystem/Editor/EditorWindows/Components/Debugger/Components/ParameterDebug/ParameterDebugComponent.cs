using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;
using UnityEditor.UIElements;

internal class ParameterDebugComponent : VisualElement
{
    private Label parameterLabel;
    public ParameterDebugComponent(KeyValuePair<string,string> parameter)
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        parameterLabel = root.Q<Label>("Identifier-Label");
        parameterLabel.text = parameter.Key+": " + parameter.Value;
    }
}