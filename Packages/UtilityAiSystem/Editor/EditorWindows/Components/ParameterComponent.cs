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
    private CompositeDisposable subscriptins = new CompositeDisposable();
    public ParameterComponent(Parameter parameter)
    {
        var t = parameter.Value.GetType();
        if (t == typeof(double))
        {
            parameter.Value = Convert.ToSingle(parameter.Value);
            t = typeof(float);
        }
        if (t == typeof(int))
        {
            var field = new IntegerField(parameter.Name);
            field.value = (int)parameter.Value;
            field.RegisterCallback<ChangeEvent<int>>(evt => parameter.Value = evt.newValue);
            parameter.ValueChanged
                .Subscribe(v =>
                {
                    field.value = (int)v;
                })
                .AddTo(subscriptins);
            Add(field);
        }
        else if (t == typeof(float))
        {
            var field = new FloatField(parameter.Name);
            field.value = (float)parameter.Value;
            field.RegisterCallback<ChangeEvent<float>>(evt => 
                    parameter.Value = evt.newValue
                );
            parameter.ValueChanged
                .Subscribe(v =>
                {
                    field.value = (float)v;
                })
                .AddTo(subscriptins);
            Add(field);
        }
        else if (t == typeof(string))
        {
            var field = new TextField(parameter.Name);
            field.value = (string)parameter.Value;
            field.RegisterCallback<ChangeEvent<string>>(evt => parameter.Value = evt.newValue);
            parameter.ValueChanged
                .Subscribe(v =>
                {
                    field.value = (string)v;
                })
                .AddTo(subscriptins);
            Add(field);

        } else if (t == typeof(long))
        {
            var field = new LongField(parameter.Name);
            field.value = (long)parameter.Value;
            field.RegisterCallback<ChangeEvent<long>>(evt => parameter.Value = evt.newValue);
            parameter.ValueChanged
                .Subscribe(v =>
                {
                    field.value = (long)v;
                })
                .AddTo(subscriptins); 
            Add(field);
        }
    }

    ~ParameterComponent()
    {
        subscriptins.Clear();
    }
}
