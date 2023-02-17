using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Stroge
{
    public interface IStorageService : IStorage
    {
        public string StrogeName { get;}
    }
}
