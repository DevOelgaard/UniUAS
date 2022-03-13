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
    private Ai aiModel;

    private CollectionComponent<Bucket> bucketCollection;

    internal UAIComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Clear();
        Body.Add(root);

        bucketCollection = new CollectionComponent<Bucket>(UASTemplateService.Instance.Buckets, "Bucket", "Buckets");
        var scorerContainer = root.Q<VisualElement>("ScorersContainer");
        decisionDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>();
        scorerContainer.Add(decisionDropdown);
        bucketDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>();
        scorerContainer.Add(bucketDropdown);
        utilityScorerDropdown = new DropdownDescriptionComponent<IUtilityScorer>();
        scorerContainer.Add(utilityScorerDropdown);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var ai = model as Ai;
        if (ai == null)
        {
            bucketCollection.SetElements(new ReactiveList<Bucket>());
        }
        else
        {
            bucketCollection.SetElements(ai.Buckets);
        }
        subscriptions.Clear();

        decisionDropdown.UpdateUi(ScorerService.Instance.ContainerSelectors, "Decision Selector", aiModel.BucketSelector.GetName());
        decisionDropdown
            .OnDropdownValueChanged
            .Subscribe(selector => {
                if (aiModel != null)
                {
                    aiModel.DecisionSelector = selector;
                }
            })
            .AddTo(subscriptions);

        bucketDropdown.UpdateUi(ScorerService.Instance.ContainerSelectors, "Bucket Selector", aiModel.BucketSelector.GetName());
        bucketDropdown
            .OnDropdownValueChanged
            .Subscribe(selector =>
            {
                if (aiModel != null)
                {
                    aiModel.BucketSelector = selector;
                }
            })
            .AddTo(subscriptions);

        utilityScorerDropdown.UpdateUi(ScorerService.Instance.UtilityScorers, "Utility Scorer", aiModel.UtilityScorer.GetName());
        utilityScorerDropdown
            .OnDropdownValueChanged
            .Subscribe(uS =>
            {
                if (aiModel != null)
                {
                    aiModel.UtilityScorer = uS;
                }
            })
            .AddTo(subscriptions);
    }

    ~UAIComponent()
    {
        subscriptions.Clear();
    }
}
