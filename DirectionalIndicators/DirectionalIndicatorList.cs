using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

namespace DirectionalIndicators;

public enum IndicatorType
{
    Standard,
    Danger
}

public struct DirectionalIndicator
{
    public DirectionalIndicator(Transform sourceTransform, IndicatorType indicator)
    {
        _sourceTransform = sourceTransform;
        Indicator = indicator;
    }

    private Transform _sourceTransform;

    public Vector3 Pos { get { return _sourceTransform.position; } }
    public String Name { get { return _sourceTransform.name; } }
    public IndicatorType Indicator { get; }

    public String StringRepresentation { get
        {
            switch (Indicator)
            {
                case IndicatorType.Standard:
                    return "<b><color=yellow>⬤</color></b>";
                case IndicatorType.Danger:
                    return "<b><color=red>!</color></b>";
                default:
                    return "default - error";
            }
        } }

    public override string ToString() => $"({Pos}, {Indicator})";

    public override bool Equals(object obj)
    {
        if (!(obj is DirectionalIndicator))
            return false;

        DirectionalIndicator other = (DirectionalIndicator)obj;
        return (_sourceTransform == other._sourceTransform) && (Indicator == other.Indicator);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + _sourceTransform.position.GetHashCode();
            hash = hash * 23 + Indicator.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(DirectionalIndicator lhs, DirectionalIndicator rhs)
    {
        return (lhs._sourceTransform == rhs._sourceTransform) && (lhs.Indicator == rhs.Indicator);
    }
    public static bool operator !=(DirectionalIndicator lhs, DirectionalIndicator rhs)
    {
        return (lhs._sourceTransform != rhs._sourceTransform) || (lhs.Indicator != rhs.Indicator);
    }
}

public class DirectionalIndicatorList : IList<DirectionalIndicator>
{
    private volatile List<Tuple<DateTime, DirectionalIndicator>> collection = [];
    private readonly Timer timer;
    private readonly TimeSpan expiration;
    private readonly TimeSpan debounce;

    public DirectionalIndicatorList()
    {
        timer = new Timer
        {
            Interval = 1000
        };
        timer.Elapsed += new ElapsedEventHandler(RemoveExpiredElements);
        timer.Start();

        expiration = TimeSpan.FromMilliseconds(Constants.DefaultIndicatorExpireTimeMs);
        debounce = TimeSpan.FromMilliseconds(Constants.DefaultIndicatorDebounceTimeMs);
    }

    private void RemoveExpiredElements(object sender, EventArgs e)
    {
        for (int i = collection.Count - 1; i >= 0; i--)
        {
            if ((DateTime.Now - collection[i].Item1) >= expiration)
            {
                collection.RemoveAt(i);
            }
        }
    }

    public List<DirectionalIndicator> TakeLast(int number)
    {
        return collection
            .Where(element => DateTime.Now >= element.Item1)
            .OrderBy(element => element.Item1)
            .Select(element => element.Item2)
            .TakeLast(number)
            .ToList();
    }

    public DirectionalIndicator this[int index]
    {
        get { return collection[index].Item2; }
        set { collection[index] = new Tuple<DateTime, DirectionalIndicator>(DateTime.Now, value); }
    }

    public IEnumerator<DirectionalIndicator> GetEnumerator()
    {
        return collection.Select(x => x.Item2).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return collection.Select(x => x.Item2).GetEnumerator();
    }

    public void Add(DirectionalIndicator item)
    {
        // if item's already in the list add it only if near end of timer (avoid quickly repeating sfx for things like footsteps)
        int item_index = IndexOf(item);

        bool item_in_list = item_index != -1;

        if (item_in_list)
        {
            DateTime item_date = collection[item_index].Item1;
            bool item_added_recently = (DateTime.Now - item_date) <= debounce;

            if (item_added_recently)
            {
                Plugin.ManualLogSource.LogInfo("Already added recently, not adding " + item.Name);
                return; 
            }
        }

        Plugin.ManualLogSource.LogInfo("Adding DI for " + item.Name);
        collection.Add(new Tuple<DateTime, DirectionalIndicator>(DateTime.Now, item));
    }

    public int Count => collection.Count;

    public bool IsSynchronized => false;

    public bool IsReadOnly => false;

    public void CopyTo(DirectionalIndicator[] array, int index)
    {
        for (int i = 0; i < collection.Count; i++)
        {
            array[i + index] = collection[i].Item2;
        }
    }

    public bool Remove(DirectionalIndicator item)
    {
        bool contained = Contains(item);

        for (int i = collection.Count - 1; i >= 0; i--)
        {
            if (collection[i].Item2 == item)
            {
                collection.RemoveAt(i);
            }
        }

        return contained;
    }

    public void RemoveAt(int i)
    {
        collection.RemoveAt(i);
    }

    public bool Contains(DirectionalIndicator item)
    {
        for (int i = 0; i < collection.Count; i++)
        {
            if (collection[i].Item2 == item)
            {
                return true;
            }
        }

        return false;
    }

    public void Insert(int index, DirectionalIndicator item)
    {
        collection.Insert(index, new Tuple<DateTime, DirectionalIndicator>(DateTime.Now, item));
    }

    public int IndexOf(DirectionalIndicator item)
    {
        for (int i = 0; i < collection.Count; i++)
        {
            if (collection[i].Item2 == item)
            {
                return i;
            }
        }

        return -1;
    }

    public void Clear()
    {
        collection.Clear();
    }
}
