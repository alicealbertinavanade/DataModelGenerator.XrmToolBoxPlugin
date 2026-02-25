using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace Avanade.XrmToolbox.DataModelDevOpsExtractor
{
    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Data Model DevOps Extractor"),
     ExportMetadata("Description", "Estrai data model da DevOps e genera Excel"),
     ExportMetadata("BackgroundColor", "Lavender"),
     ExportMetadata("PrimaryFontColor", "Black"),
     ExportMetadata("SecondaryFontColor", "Gray")]
    public class DataModelDevOpsExtractorPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new PluginControl();
        }
    }
}