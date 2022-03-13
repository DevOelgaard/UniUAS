using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using UniRx;
using UniRxExtension;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CollectionComponent<T> : VisualElement where T : AiObjectModel
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private CompositeDisposable listViewSubscriptions = new CompositeDisposable();
    private IDisposable collectionUpdatedSub;

    private TemplateContainer root;

    private Button sortCollectionButton;
    private VisualElement tempHeader;
    private PopupField<string> addCopyPopup;

    private Label elementsLabel;
    private ScrollView elementsBody;

    private ReactiveList<T> collection;
    private ReactiveList<AiObjectModel> templates;

    public CollectionComponent(ReactiveList<AiObjectModel> templates, string tempLabel, string elementsLabel, string dropDownLabel = "Templates")
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType());
        Add(root);

        this.collection = new ReactiveList<T>();
        this.templates = templates;

        sortCollectionButton = root.Q<Button>("SortCollection-Button");
        this.elementsLabel = root.Q<Label>("Elements-Label");
        elementsBody = root.Q<ScrollView>("ElementsBody");
        
        tempHeader = root.Q<VisualElement>("TempHeader");
        addCopyPopup = new PopupField<string>("Add " + tempLabel);
        tempHeader.Add(addCopyPopup);

        addCopyPopup.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (evt.newValue == null) return;
            AddCopy(evt.newValue);
            addCopyPopup.value = null;
        });

        this.templates = templates;
        this.templates.OnValueChanged
            .Subscribe(_ => InitAddCopyPopup())
            .AddTo(subscriptions);

        InitAddCopyPopup();

        this.elementsLabel.text = elementsLabel;

        var t = collection.GetType();
        if (t == typeof(ReactiveList<Consideration>))
        {
            sortCollectionButton.text = Consts.Text_Button_SortByPerformance;
            sortCollectionButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                var cast = collection as ReactiveList<Consideration>;
                var sortedList = cast.Values.OrderBy(c => c.PerformanceTag).ToList();
                cast.Clear();
                sortedList.ForEach(c => cast.Add(c));
            });
        }
        else if (t == typeof(ReactiveList<Bucket>))
        {
            sortCollectionButton.text = Consts.Text_Button_SortByWeight;
            sortCollectionButton.RegisterCallback<MouseUpEvent>(evt =>
            {
                var cast = collection as ReactiveList<Bucket>;
                var sortedList = cast.Values.OrderByDescending(b => b.Weight.Value).ToList();
                cast.Clear();
                sortedList.ForEach(c => cast.Add(c));
            });
        }
        else
        {
            sortCollectionButton.style.display = DisplayStyle.None;
        }


        //SetElements(collection.Values);
    }

    private void InitAddCopyPopup()
    {
        var namesFromFiles = AssetDatabaseService.GetActivateableTypes(typeof(T));
        addCopyPopup.choices = namesFromFiles
            .Where(t => !t.Name.Contains("Mock") && !t.Name.Contains("Stub"))
            .Select(t => t.Name)
            .ToList();


        foreach(var template in templates.Values)
        {
            addCopyPopup.choices.Add(template.Name);
        }
    }

    private void AddCopy(string name)
    {
        T element = templates.Values.FirstOrDefault(t => t.Name == name) as T;

        if (element == null)
        {
            element = AssetDatabaseService.GetInstanceOfType<T>(name);
        }

        element = (T)element.Clone();
        AddElement(element);
    }

    internal void AddElement(T element)
    {
        collection.Add(element);
    }

    internal void SetElements(ReactiveList<T> elements)
    {

        this.collection = elements;
        collectionUpdatedSub?.Dispose();
        collectionUpdatedSub = collection.OnValueChanged
            .Subscribe(_ => UpdateCollection());
        UpdateCollection();
    }

    private void UpdateCollection()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        elementsBody.Clear();
        listViewSubscriptions.Clear();
        foreach (var element in this.collection.Values)
        {
            var folded = new MainWindowFoldedComponent();
            var expanded = MainWindowService.GetComponent(element);
            var listView = new ListViewComponent();

            folded.UpdateUi(element);
            expanded.UpdateUi(element);
            listView.UpdateUi(expanded, folded);

            listView.OnRemoveClicked
                .Subscribe(_ => collection.Remove(element))
                .AddTo(listViewSubscriptions);

            listView.OnUpClicked
                .Subscribe(_ => collection.DecreaseIndex(element))
                .AddTo(listViewSubscriptions);

            listView.OnDownClicked
                .Subscribe(_ => collection.IncreaIndex(element))
                .AddTo(listViewSubscriptions);

            elementsBody.Add(listView);
        }
    }

    private void ClearSubscriptions()
    {
        collectionUpdatedSub?.Dispose();
        listViewSubscriptions.Clear();
        subscriptions.Clear();
    }

    ~CollectionComponent()
    {
        ClearSubscriptions();
    }
}