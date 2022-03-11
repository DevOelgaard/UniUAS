using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;

internal class UAIComponent : MainWindowComponent
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private DropdownDescriptionComponent<IUtilityContainerSelector> decisionDropdown;
    private DropdownDescriptionComponent<IUtilityContainerSelector> bucketDropdown;
    private DropdownDescriptionComponent<IUtilityScorer> utilityScorerDropdown;

    private CollectionComponent<Bucket> bucketCollections;

    internal UAIComponent(Ai aiModel) : base(aiModel)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        var scorerContainer = root.Q<VisualElement>("ScorersContainer");
        decisionDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>(ScorerService.Instance.ContainerSelectors, "Decision Selector", aiModel.BucketSelector.GetName());
        decisionDropdown
            .OnDropdownValueChanged
            .Subscribe(selector => aiModel.DecisionSelector = selector)
            .AddTo(subscriptions);
        scorerContainer.Add(decisionDropdown);

        bucketDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>(ScorerService.Instance.ContainerSelectors, "Bucket Selector", aiModel.BucketSelector.GetName());
        bucketDropdown
            .OnDropdownValueChanged
            .Subscribe(selector => aiModel.BucketSelector = selector)
            .AddTo(subscriptions);
        scorerContainer.Add(bucketDropdown);

        utilityScorerDropdown = new DropdownDescriptionComponent<IUtilityScorer>(ScorerService.Instance.UtilityScorers, "Utility Scorer", aiModel.UtilityScorer.GetName());
        utilityScorerDropdown
            .OnDropdownValueChanged
            .Subscribe(uS => aiModel.UtilityScorer = uS)
            .AddTo(subscriptions);
        scorerContainer.Add(utilityScorerDropdown);

        Body.Clear();
        Body.Add(root);

        UpdateAi(aiModel);
    }

    ~UAIComponent()
    {
        subscriptions.Clear();
    }

    internal void UpdateAi(Ai ai)
    {
        if (ai == null)
        {
            root.Clear();
        }
        else
        {
            {
                bucketCollections =
                    new CollectionComponent<Bucket>(ai.Buckets,
                    UASTemplateService.Instance.Buckets, "Bucket", "Buckets");
                root.Add(bucketCollections);
            }
        }
    }
}
