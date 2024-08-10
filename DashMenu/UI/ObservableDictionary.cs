using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows;

namespace DashMenu.UI
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly object _lock = new object(); // Lock object for thread safety

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
        public void Add(TKey key, TValue value)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    _dictionary.Add(key, value);
                    // Find the index of the newly added item
                    int index = _dictionary.Keys.ToList().IndexOf(key);
                    // Notify that an item has been added
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        new KeyValuePair<TKey, TValue>(key, value),
                        index
                    ));
                    OnPropertyChanged(nameof(Count));
                    OnPropertyChanged("Item[]");
                }
            });
        }
        public bool Remove(TKey key)
        {
            // Use Dispatcher to ensure UI updates happen on the UI thread
            return Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lock)
                {
                    if (_dictionary.TryGetValue(key, out TValue value))
                    {
                        // Get the index before removal
                        int index = _dictionary.Keys.ToList().IndexOf(key);

                        // Remove the item from the dictionary
                        bool removed = _dictionary.Remove(key);

                        if (removed)
                        {
                            // Notify the collection view of the removal
                            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Remove,
                                new KeyValuePair<TKey, TValue>(key, value),
                                index
                            ));

                            // Raise property changed notifications
                            OnPropertyChanged(nameof(Count));
                            OnPropertyChanged("Item[]");

                            return true;
                        }
                    }

                    return false;
                }
            });
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
