using System;
using System.Collections.Generic;
using UniRx;

namespace UniRxExtension
{
    [Serializable]
    public class ReactiveList<T>
    {
        private List<T> list;
        public IObservable<List<T>> OnValueChanged => onValueChanged;
        private Subject<List<T>> onValueChanged;

        public ReactiveList()
        {
            list = new List<T>();
            onValueChanged = new Subject<List<T>>();
        }

        public ReactiveList(List<T> list){
            this.list = list;
            onValueChanged = new Subject<List<T>>();
        }


        public List<T> Values => CopyList();

        private List<T> CopyList()
        {
            return new List<T>(list);
        }

        public void Add(T element)
        {
            list.Add(element);
            onValueChanged.OnNext(Values);
        }

        public void Remove(T element)
        {
            list.Remove(element);
            onValueChanged.OnNext(Values);
        }

        public void Clear()
        {
            var copyList = new List<T>(list);
            foreach(var element in copyList)
            {
                Remove(element);
            }
        }

        public void IncreaIndex(T element)
        {
            var index = list.IndexOf(element);
            if (index >= list.Count-1) return;
            var itemToReplace = list[index+1];
            list[index + 1] = element;
            list[index] = itemToReplace;
            onValueChanged.OnNext(list);
        }

        public void DecreaseIndex(T element)
        {
            var index = list.IndexOf(element);
            if (index <= 0) return;
            var itemToReplace = list[index - 1];
            list[index - 1] = element;
            list[index] = itemToReplace;
            onValueChanged.OnNext(list);
        }
        public int Count => list.Count;
    }
}
