using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using UniRx;
using UniRxExtension;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class CollectionComponent<T> : VisualElement where T : AiObjectModel
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private CompositeDisposable listViewSubscriptions = new CompositeDisposable();
    private IDisposable collectionUpdatedSub;

    private TemplateContainer root;

    //private Label tempLabel;
    //private Button addcopyButton;
    private Button sortCollectionButton;
    private VisualElement tempHeader;
    //private VisualElement tempBody;
    private PopupField<string> addCopyPopup;

    private Label elementsLabel;
    private ScrollView elementsBody;
    //private DropdownField elementsDropdown;

    private ReactiveList<T> collection;
    private ReactiveList<AiObjectModel> templates;
    //private List<string> dropDownChoices;

    public CollectionComponent(ReactiveList<T> collection, ReactiveList<AiObjectModel> templates, string tempLabel, string elementsLabel, string dropDownLabel = "Templates")
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType());
        Add(root);

        this.collection = collection;
        this.templates = templates;

        sortCollectionButton = root.Q<Button>("SortCollection-Button");
        this.elementsLabel = root.Q<Label>("Elements-Label");
        elementsBody = root.Q<ScrollView>("ElementsBody");
        
        tempHeader = root.Q<VisualElement>("TempHeader");
        addCopyPopup = new PopupField<string>("Add " + tempLabel);
        tempHeader.Add(addCopyPopup);

        addCopyPopup.RegisterCallback<ChangeEvent<string>>(evt =>
         {
             AddCopy(evt.newValue);
         });
        this.templates = templates;

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

        collectionUpdatedSub?.Dispose();
        collectionUpdatedSub = collection.OnValueChanged
            .Subscribe(elements => UpdateElements(elements));
        UpdateElements(collection.Values);
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
        T aiObject = templates.Values.FirstOrDefault(t => t.Name == name) as T;

        //var numberOfIdenticalNames = collection
        //    .Values
        //    .Where(e => e.Name.Contains(aiObject.Name))
        //    .ToList()
        //    .Count;

        //if (numberOfIdenticalNames > 0)
        //{
        //    aiObject.Name += "("+numberOfIdenticalNames+")";
        //}
            

        if (aiObject != null)
        {
            aiObject = (T)aiObject.Clone();
        } else
        {
            aiObject = AssetDatabaseService.CreateInstanceOfType<T>(name);
        }
        collection.Add(aiObject);
        
    }

    private void UpdateElements(List<T> elements)
    {
        elementsBody.Clear();
        listViewSubscriptions.Clear();
        foreach (var element in elements)
        {
            var folded = new MainWindowFoldedComponent(element);
            var expanded = MainWindowService.GetComponent(element);
            var foldAble = new FoldableComponent(expanded, folded);
            var listView = new ListViewComponent(foldAble);

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