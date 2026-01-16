using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace DashMenu.UI
{
    public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private const string IndexerName = "Item[]";

        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly object _lock = new object();

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

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void NotifyCollectionModified()
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
        }

        public void Add(TKey key, TValue value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    _dictionary.Add(key, value);
                    int index = _dictionary.Keys.ToList().IndexOf(key);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        new KeyValuePair<TKey, TValue>(key, value),
                        index
                    ));
                    NotifyCollectionModified();
                }
            });
        }

        public bool Remove(TKey key)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    if (!_dictionary.TryGetValue(key, out TValue value))
                    {
                        return false;
                    }

                    int index = _dictionary.Keys.ToList().IndexOf(key);
                    bool removed = _dictionary.Remove(key);

                    if (removed)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            new KeyValuePair<TKey, TValue>(key, value),
                            index
                        ));
                        NotifyCollectionModified();
                    }

                    return removed;
                }
            });
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (_lock)
                    {
                        if (_dictionary.ContainsKey(key))
                        {
                            _dictionary[key] = value;
                            OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value));
                            OnPropertyChanged(IndexerName);
                        }
                        else
                        {
                            Add(key, value);
                        }
                    }
                });
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    _dictionary.Clear();
                    OnCollectionChanged(NotifyCollectionChangedAction.Reset, null);
                    NotifyCollectionModified();
                }
            });
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
