using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace UfaSmartCity.Models
{
    public class ViewDataContainer
    {
        public ViewDataDictionary ViewData { get; set; }
        public ViewDataContainer(ViewDataDictionary viewData)
        {
            ViewData = new ViewDataDictionary(viewData);
        }
    }
}