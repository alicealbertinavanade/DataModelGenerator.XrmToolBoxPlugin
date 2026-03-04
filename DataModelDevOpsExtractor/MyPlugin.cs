using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DataModelDevOpsExtractor
{
    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Data Model DevOps Extractor"),
     ExportMetadata("Description", "Estrai data model da DevOps e genera Excel"),
     // Please specify the base64 content of a 32x32 pixels image
     ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAABsSURBVFhH7ZTBCsAwCEPtPla/SX92A7G7jdEeVGjexeKlIcTQ8QxmvuNdwhWzjNcBVfVFFiLis9wBCIAA9MCISVl9MO/fzPxvhBACIKBcwGcPzHv9Y7U32vUAmnA7A7tOIQP9HciijQOnQ/QATUM2wyg3zoIAAAAASUVORK5CYII="),
     // Please specify the base64 content of a 80x80 pixels image
     ExportMetadata("BigImageBase64", "iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAYAAACOEfKtAAAAAXNSR0IArs4c6QAAAWpJREFUeF7t3Elug0AARFG4rDkTXDZZZElHQvoJxvC8L9O8LgYzeJ58ksCc0sITwFgCgACjQIzPr9frK37Ho+MA4/QDBBgFYlwD/wNwXdf4tfeML8uyW7FhAwGOCwAwbhgAAUaBGNdAgFEgxjUQYBSIcQ0EGAViXAMBRoEY10CAUSDGDzcwLudRcRdU43QDBBgFYlwDK+AoP7rZ/rTL/KMj7rZtu0dhhs/GAJwmgHHTBAgwCsS4BgKMAjGugQCjQIxrIMAoEOMaCDAKxLgGAowCMa6BAKNAjGsgwCgQ4xr4CYCjWYrj3sXfdS/mlAYCnMZvrB+9qQQQ4K97HJtw3BmfAhjHeOk4wDg9AAFGgRjXwE8A/OvzwHf96hhZn9JAgBc7kdbAuN8BCPBH4OjFhOh16fgpB5FLC8TBAQQYBWJcA68GGMdzi3h6T+QWAnElAAKMAjGugWcBxuU8Ku6PuON0AwQYBWJcAyPgN+KWrilW8C5WAAAAAElFTkSuQmCC"),
     ExportMetadata("BackgroundColor", "Lavender"),
     ExportMetadata("PrimaryFontColor", "Black"),
     ExportMetadata("SecondaryFontColor", "Gray")]
    public class MyPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new PluginControl();
        }
        public MyPlugin()
        {
        }

        // opzionale ma consigliato:
        //public override void UpdateConnection(
        //    Microsoft.Xrm.Sdk.IOrganizationService newService,
        //    XrmToolBox.Extensibility.ConnectionDetail detail,
        //    string actionName,
        //    object parameter)
        //{
        //    base.UpdateConnection(newService, detail, actionName, parameter);
        //    // qui gestisci connessione
        //}
    }
}
