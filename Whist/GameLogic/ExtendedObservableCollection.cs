using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whist.GameLogic
{
    static class ExtendedObservableCollection
    {
        public static void ForEach<T>(this ObservableCollection<T> observable, Action<T> action)
        {
            foreach (var cur in observable)
            {
                action(cur);
            }
        }

        public static ObservableCollection<T> GetRange<T>(this ObservableCollection<T> observable, int start, int end)
        {
            ObservableCollection<T> collection = new ObservableCollection<T>();
            for (int i = 0; i < observable.ToArray().Length; i++)
                if (i >= start && i <= end)
                    collection.Add(observable.ToArray()[i]);
            return collection;
        }

        public static void RemoveRange<T>(this ObservableCollection<T> observable, int start, int end)
        {
            observable = new ObservableCollection<T>(observable.Where((source, index) => (index >= start && index <= end)));
        }

        public static void Sort<T>(this ObservableCollection<T> observable) where T : IComparable
        {
            List<T> sorted = observable.OrderBy(x => x).ToList();

            int ptr = 0;
            while (ptr < sorted.Count)
            {
                if (!observable[ptr].Equals(sorted[ptr]))
                {
                    T t = observable[ptr];
                    observable.RemoveAt(ptr);
                    observable.Insert(sorted.IndexOf(t), t);
                }
                else
                {
                    ptr++;
                }
            }
        }
    }

    public static class ExtendedIList
    {
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (var cur in list)
            {
                action(cur);
            }
        }
    }
}
