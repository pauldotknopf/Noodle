using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Noodle.Web.Mvc
{
    public class EnrichedViewResult<T> : ViewResult
    {
        public EnrichedViewResult(string viewName, ViewDataDictionary viewData)
        {
            this.ViewName = viewName;
            this.ViewData = viewData;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (this.Model != null)
            {
                var enricher = DependencyResolver.Current.GetService<IViewModelEnricher<T>>();
                if (enricher != null)
                {
                    enricher.Enrich((T)this.Model);
                }
            }

            base.ExecuteResult(context);
        }
    }
}
