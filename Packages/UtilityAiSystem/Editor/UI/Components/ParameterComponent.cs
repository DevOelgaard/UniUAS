using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UniRx;

public class ParameterComponent: VisualElement
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public VisualElement field;
    public ParameterComponent(Parameter parameter)
    {
        var t = parameter.Value.GetType();
        if (t == typeof(double))
        {
            parameter.Value = Convert.ToSingle(parameter.Value);
            t = typeof(float);
        }
        if (t == typeof(int) || t == typeof(Int16) || t == typeof(Int32) || t == typeof(Int64))
        {
            var field = new IntegerFieldMinMax(parameter.Name);
            field.value =  Convert.ToInt32(parameter.Value);
            field.RegisterCallback<ChangeEvent<int>>(evt => parameter.Value = evt.newValue);
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    field.value = (int)v;
                })
                .AddTo(disposables);
            Add(field);
            this.field = field;
        }
        else if (t == typeof(float) || t == typeof(Single))
        {
            var field = new FloatFieldMinMax(parameter.Name);
            field.value = Convert.ToSingle(parameter.Value);
            field.RegisterCallback<ChangeEvent<float>>(evt => 
                    parameter.Value = evt.newValue
                );
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    field.value = (float)v;
                })
                .AddTo(disposables);
            Add(field);
            this.field = field;
        }
        else if (t == typeof(string))
        {
            var field = new TextField(parameter.Name);
            field.value = (string)parameter.Value;
            field.RegisterCallback<ChangeEvent<string>>(evt => parameter.Value = evt.newValue);
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    field.value = (string)v;
                })
                .AddTo(disposables);
            Add(field);
            this.field = field;

        }
        else if (t == typeof(long))
        {
            var field = new LongField(parameter.Name);
            field.value = (long)parameter.Value;
            field.RegisterCallback<ChangeEvent<long>>(evt => parameter.Value = evt.newValue);
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    field.value = (long)v;
                })
                .AddTo(disposables); 
            Add(field);
            this.field = field;
        }
        else if (t == typeof(bool))
        {
            var field = new Toggle(parameter.Name);
            field.value = (bool)parameter.Value;
            field.RegisterCallback<ChangeEvent<bool>>(evt => parameter.Value = evt.newValue);
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    field.value = (bool)v;
                })
                .AddTo(disposables);
            Add(field);
            this.field = field;
        }
    }

    ~ParameterComponent()
    {
        disposables.Clear();
    }
}
