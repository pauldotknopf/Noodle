using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Web.Mvc
{
    public interface IViewModelEnricher<TViewModel>
    {
        void Enrich(TViewModel viewModel);
    }
}
