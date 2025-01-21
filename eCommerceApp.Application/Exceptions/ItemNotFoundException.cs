using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceApp.Infrastructure.Exceptions
{
    public class ItemNotFoundException(string message) : Exception(message)
    {
    }
}
