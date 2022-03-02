﻿using UnityEngine.UIElements;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
using UniRxExtension;

internal class DecisionComponent : MainWindowComponent
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private TemplateContainer root;

    private CollectionComponent<Consideration> considerationCollections;
    private CollectionComponent<AgentAction> agentActionCollection;
    private Decision model;
    internal DecisionComponent(Decision model): base(model)
    {
        this.model = model;
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        considerationCollections = new CollectionComponent<Consideration>(model.Considerations, UASTemplateService.Instance.Considerations, "Consideration-Template", "Considerations");
        root.Add(considerationCollections);

        agentActionCollection = new CollectionComponent<AgentAction>(model.AgentActions, UASTemplateService.Instance.AgentActions, "Action-Template", "Actions");
        root.Add(agentActionCollection);

        Body.Clear();
        Body.Add(root);
    }

    ~DecisionComponent()
    {
        disposables.Clear();
    }
}