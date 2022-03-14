using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentLogComponent : AiObjectLogComponent
{
    private AiLogComponent aiLogComponent;

    public AgentLogComponent(): base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);
        aiLogComponent = new AiLogComponent();
        root.Add(aiLogComponent);
    }

    protected override void UpdateUiInternal(AiObjectLog aiObjectLog)
    {
        var a = aiObjectLog as AgentLog;
        aiLogComponent.UpdateUi(a.Ai);
    }
}