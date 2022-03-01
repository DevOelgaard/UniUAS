using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using System;

public class ListViewComponent : VisualElement
{
    private TemplateContainer root;

    private Button toggleViewButton;
    private Button removeButton;

    public IObservable<bool> OnRemoveClicked => onRemoveClicked;
    private Subject<bool> onRemoveClicked = new Subject<bool>(); 

    private VisualElement centerContainer;
    private FoldableComponent foldableComponent;
    public FoldableComponent FoldableComponent {
        get => foldableComponent;
        set
        {
            foldableComponent = value;
            UpdateContent();
        } 
    }

    public ListViewComponent(FoldableComponent foldableComponent)
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        toggleViewButton = root.Q<Button>("ToggleView-Button");
        removeButton = root.Q<Button>("Remove-Button");
        centerContainer = root.Q<VisualElement>("Center");
        
        FoldableComponent = foldableComponent;
        toggleViewButton.RegisterCallback<MouseUpEvent>(_ => FoldableComponent.Toggle());
        removeButton.RegisterCallback<MouseUpEvent>(_ => {
                onRemoveClicked.OnNext(true);
            });
    }

    private void UpdateContent()
    {
        centerContainer.Clear();
        centerContainer.Add(FoldableComponent);
    }
}
