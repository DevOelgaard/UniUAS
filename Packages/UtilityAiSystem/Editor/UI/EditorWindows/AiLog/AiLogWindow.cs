using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UnityEditor;
using UnityEngine.UIElements;

internal class AiLogWindow : SplitViewWindowDropDownSelection<IAgent>
{
    protected override ReactiveList<IAgent> GetLeftPanelElements(string identifier)
    {
        return AgentManager.Instance.GetAgentsByIdentifier(identifier);
    }

    protected override string GetNameFromElement(IAgent element)
    {
        return element.Model.Name;
    }

    protected override RightPanelComponent<IAgent> GetRightPanelComponent()
    {
        return new LoggerComponent();
    }
}