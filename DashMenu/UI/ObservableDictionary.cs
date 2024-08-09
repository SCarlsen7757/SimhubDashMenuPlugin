using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace DashMenu.UI
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly Dictionary<TKey, TValue> _dictionary;

        public ObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        public void Add(TKey key, TValue value)
        {
            //If the dictionary is used in WPF, then it's need to be added in the UI thread.
            Application.Current.Dispatcher.Invoke(() =>
            {
                _dictionary.Add(key, value);
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
            });
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var value) && _dictionary.Remove(key))
            {
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                if (_dictionary.ContainsKey(key))
                {
                    var oldValue = _dictionary[key];
                    _dictionary[key] = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value));
                    OnPropertyChanged("Item[]");
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            _dictionary.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged("Item[]");
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        public int Count => _dictionary.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}
