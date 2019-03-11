using NavomiApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NavomiApi.Interfaces
{
    public interface ISalesForceService
    {
         Task<bool> loginToSalesForce();
        Task<IEnumerable<Record>> GetRecords(string objectType);
    }
}
