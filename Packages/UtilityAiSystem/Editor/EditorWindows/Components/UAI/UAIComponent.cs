using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;

internal class UAIComponent : MainWindowComponent
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private DropdownDescriptionComponent<IUtilityContainerSelector> decisionDropdown;
    private DropdownDescriptionComponent<IUtilityContainerSelector> bucketDropdown;
    private DropdownDescriptionComponent<IUtilityScorer> utilityScorerDropdown;

    private CollectionComponent<Bucket> bucketCollections;
    internal UAIComponent(UAIModel model) : base(model)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        var scorerContainer = root.Q<VisualElement>("ScorersContainer");
        decisionDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>(ScorerService.Instance.ContainerSelectors, "Decision Selector", model.BucketSelector.GetName());
        decisionDropdown
            .OnDropdownValueChanged
            .Subscribe(selector => model.DecisionSelector = selector)
            .AddTo(subscriptions);
        scorerContainer.Add(decisionDropdown);

        bucketDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>(ScorerService.Instance.ContainerSelectors, "Bucket Selector", model.BucketSelector.GetName());
        bucketDropdown
            .OnDropdownValueChanged
            .Subscribe(selector => model.BucketSelector = selector)
            .AddTo(subscriptions);
        scorerContainer.Add(bucketDropdown);

        utilityScorerDropdown = new DropdownDescriptionComponent<IUtilityScorer>(ScorerService.Instance.UtilityScorers, "Utility Scorer", model.UtilityScorer.GetName());
        utilityScorerDropdown
            .OnDropdownValueChanged
            .Subscribe(uS => model.UtilityScorer = uS)
            .AddTo(subscriptions);
        scorerContainer.Add(utilityScorerDropdown);

        bucketCollections =
            new CollectionComponent<Bucket>(model.Buckets,
            UASTemplateService.Instance.Buckets, "Bucket-Template", "Buckets");
        root.Add(bucketCollections);
        Body.Clear();
        Body.Add(root);
    }

    ~UAIComponent()
    {
        subscriptions.Clear();
    }
}
