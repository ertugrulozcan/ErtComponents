using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Eigen.Core.Utility
{
    public interface ISearchable
    {
        string SearchKey { get; }
        ICommand SearchResultSelectCommand { get; }
    }
}
