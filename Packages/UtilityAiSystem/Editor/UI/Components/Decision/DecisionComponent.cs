using UnityEngine.UIElements;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRxExtension;
using UnityEngine;

internal class DecisionComponent : AiObjectComponent 
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private TemplateContainer root;
    private VisualElement parametersContainer;

    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<AgentAction> agentActionCollection;
    private Decision decision;
    internal DecisionComponent(): base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("Parameters");

        considerationCollections = new CollectionComponent<Consideration>(UASTemplateService.Instance.Considerations, "Consideration", "Considerations");
        root.Add(considerationCollections);

        agentActionCollection = new CollectionComponent<AgentAction>(UASTemplateService.Instance.AgentActions, "Action", "Actions");
        root.Add(agentActionCollection);

        Body.Clear();
        Body.Add(root);
    }
    protected override void UpdateInternal(AiObjectModel model)
    {
        this.decision = model as Decision;
        considerationCollections.SetElements(decision.Considerations);
        agentActionCollection.SetElements(decision.AgentActions);
        SetParameters();

    }

    private void SetParameters()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        parametersContainer.Clear();

        foreach (var parameter in decision.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            parametersContainer.Add(pC);
        }
    }



    ~DecisionComponent()
    {
        disposables.Clear();
    }
}