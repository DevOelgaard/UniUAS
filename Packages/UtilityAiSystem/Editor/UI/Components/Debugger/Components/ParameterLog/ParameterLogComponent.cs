using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;
using UnityEditor.UIElements;

internal class ParameterLogComponent : LogComponent
{
    private Label parameterLabel;
    public ParameterLogComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        parameterLabel = root.Q<Label>("Identifier-Label");
    }

    internal override void Display(ILogModel element)
    {
        var parameter = element as ParameterLog;
        this.style.display = DisplayStyle.Flex;
        //this.style.opacity = 1;
        parameterLabel.text = parameter.Name + ": " + parameter.Value;
    }
}