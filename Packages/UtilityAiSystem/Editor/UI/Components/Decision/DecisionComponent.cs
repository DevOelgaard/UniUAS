using UnityEngine.UIElements;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRxExtension;

internal class DecisionComponent : MainWindowComponent
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private TemplateContainer root;
    private VisualElement parametersContainer;

    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<AgentAction> agentActionCollection;
    private Decision model;
    internal DecisionComponent(Decision model): base(model)
    {
        this.model = model;
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("Parameters");

        considerationCollections = new CollectionComponent<Consideration>(model.Considerations, UASTemplateService.Instance.Considerations, "Consideration", "Considerations");
        root.Add(considerationCollections);

        agentActionCollection = new CollectionComponent<AgentAction>(model.AgentActions, UASTemplateService.Instance.AgentActions, "Action", "Actions");
        root.Add(agentActionCollection);

        SetParameters();

        Body.Clear();
        Body.Add(root);
    }

    private void SetParameters()
    {
        parametersContainer.Clear();

        foreach (var parameter in model.Parameters)
        {
            parametersContainer.Add(new ParameterComponent(parameter));
        }
    }

    ~DecisionComponent()
    {
        disposables.Clear();
    }
}