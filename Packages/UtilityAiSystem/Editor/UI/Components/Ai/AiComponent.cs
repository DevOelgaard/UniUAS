using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UniRxExtension;

internal class AiComponent : AiObjectComponent 
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private TemplateContainer root;
    private DropdownDescriptionComponent<IUtilityContainerSelector> decisionDropdown;
    private DropdownDescriptionComponent<IUtilityContainerSelector> bucketDropdown;
    private DropdownDescriptionComponent<IUtilityScorer> utilityScorerDropdown;
    private Ai aiModel;
    private VisualElement collectionsContainer;
    private Toggle playableToggle;

    private CollectionComponent<Bucket> bucketCollection;

    internal AiComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Clear();
        Body.Add(root);

        collectionsContainer = root.Q<VisualElement>("CollectionsContainer");
        playableToggle = root.Q<Toggle>("Playable-Toggle");

        bucketCollection = new CollectionComponent<Bucket>(UASTemplateService.Instance.Buckets, "Bucket", "Buckets");
        collectionsContainer.Add(bucketCollection);

        var scorerContainer = root.Q<VisualElement>("ScorersContainer");
        decisionDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>();
        scorerContainer.Add(decisionDropdown);
        bucketDropdown = new DropdownDescriptionComponent<IUtilityContainerSelector>();
        scorerContainer.Add(bucketDropdown);
        utilityScorerDropdown = new DropdownDescriptionComponent<IUtilityScorer>();
        scorerContainer.Add(utilityScorerDropdown);

        playableToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            if (aiModel == null) return;
            aiModel.IsPLayable = evt.newValue;
        });
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        aiModel = model as Ai;

        playableToggle.SetValueWithoutNotify(aiModel.IsPLayable);
        if (aiModel == null)
        {
            bucketCollection.SetElements(new ReactiveList<Bucket>());
        }
        else
        {
            bucketCollection.SetElements(aiModel.Buckets);
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

    ~AiComponent()
    {
        subscriptions.Clear();
    }
}
