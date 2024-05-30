using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Application.Common.Interfaces;

public interface ISimpleCache<T>
{
    Task<T> Get(string key);
    void Set(string key, T value);
    void Reset(string key);
}
