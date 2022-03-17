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
    private IDisposable bucketTabSub;
    private TemplateContainer root;
    private DropdownDescriptionComponent<UtilityContainerSelector> decisionDropdown;
    private DropdownDescriptionComponent<UtilityContainerSelector> bucketDropdown;
    private DropdownDescriptionComponent<IUtilityScorer> utilityScorerDropdown;
    private Ai aiModel;
    private VisualElement collectionsContainer;
    private Toggle playableToggle;

    private TabViewComponent tabView;
    private Button bucketTab;
    private Button settingsTab;

    private CollectionComponent<Bucket> bucketCollection;

    internal AiComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Clear();
        Body.Add(root);
        styleSheets.Add(StylesService.GetStyleSheet("AiObjectComponent"));
        collectionsContainer = root.Q<VisualElement>("CollectionsContainer");

        tabView = new TabViewComponent();
        collectionsContainer.Add(tabView);

        bucketCollection = new CollectionComponent<Bucket>(UASTemplateService.Instance.Buckets, "Bucket", "Buckets");

        var settingsContainer = new VisualElement();

        playableToggle = new Toggle("Playable");
        playableToggle.name = "Playable-Toggle";
        decisionDropdown = new DropdownDescriptionComponent<UtilityContainerSelector>();
        settingsContainer.Add(decisionDropdown);
        bucketDropdown = new DropdownDescriptionComponent<UtilityContainerSelector>();
        settingsContainer.Add(bucketDropdown);
        utilityScorerDropdown = new DropdownDescriptionComponent<IUtilityScorer>();
        settingsContainer.Add(utilityScorerDropdown);

        settingsContainer.name = "SettingsContainer";

        bucketTab = tabView.AddTabGroup("Buckets", bucketCollection);
        settingsTab = tabView.AddTabGroup("Settings", settingsContainer);

        playableToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            if (aiModel == null) return;
            aiModel.IsPLayable = evt.newValue;
        });
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        aiModel = model as Ai;
        ScoreContainer.Add(playableToggle);

        bucketTab.text = "Buckets (" + aiModel.Buckets.Count + ")";
        bucketTabSub?.Dispose();
        bucketTabSub = aiModel.Buckets.OnValueChanged
            .Subscribe(list => bucketTab.text = "Buckets (" + list.Count + ")");
        
        
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
        bucketTabSub?.Dispose();
        subscriptions.Clear();
    }
}
