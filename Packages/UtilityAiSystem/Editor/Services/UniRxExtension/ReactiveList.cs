using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
