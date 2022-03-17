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
    //private DropdownDescriptionComponent<UtilityContainerSelector> decisionDropdown;
    //private DropdownDescriptionComponent<UtilityContainerSelector> bucketDropdown;
    private DropdownContainerComponent<UtilityContainerSelector> bucketDropdown;
    private DropdownContainerComponent<UtilityContainerSelector> decisionDropdown;
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
        
        bucketDropdown = new DropdownContainerComponent<UtilityContainerSelector>("Bucket Selector");
        settingsContainer.Add(bucketDropdown);
        decisionDropdown = new DropdownContainerComponent<UtilityContainerSelector>("Decision Selector");
        settingsContainer.Add(decisionDropdown);

        utilityScorerDropdown = new DropdownDescriptionComponent<IUtilityScorer>();
        settingsContainer.Add(utilityScorerDropdown);
        settingsContainer.name = "SettingsContainer";

        bucketDropdown.name = "DropdownScorerCollection";
        decisionDropdown.name = "DropdownScorerCollection";
        utilityScorerDropdown.name = "DropdownScorerCollection";

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

        var currentDecisionIndex = aiModel.DecisionSelectors.IndexOf(aiModel.CurrentDecisionSelector);
        decisionDropdown.UpdateUi(aiModel.DecisionSelectors, currentDecisionIndex);

        decisionDropdown
            .OnSelectedObjectChanged
            .Subscribe(selector => {
                if (aiModel != null)
                {
                    aiModel.CurrentDecisionSelector = selector;
                }
            })
            .AddTo(subscriptions);

        var currentBucketindex = aiModel.BucketSelectors.IndexOf(aiModel.CurrentBucketSelector);
        bucketDropdown.UpdateUi(aiModel.BucketSelectors,currentDecisionIndex);
        bucketDropdown
            .OnSelectedObjectChanged
            .Subscribe(selector =>
            {
                if (aiModel != null)
                {
                    aiModel.CurrentBucketSelector = selector;
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
