using UnityEngine.UIElements;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRxExtension;

public class DecisionComponent : MainWindowComponent
{
    private TemplateContainer root;

    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<AgentAction> agentActionCollection;
    public DecisionComponent(Decision model): base(model)

    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        considerationCollections = new CollectionComponent<Consideration>(model.Considerations, UASTemplateService.Instance.Considerations, "Consideration-Template", "Considerations");
        root.Add(considerationCollections);

        agentActionCollection = new CollectionComponent<AgentAction>(model.AgentActions, UASTemplateService.Instance.AgentActions, "Action-Template", "Actions");
        root.Add(agentActionCollection);

        Body.Clear();
        Body.Add(root);
    }
}