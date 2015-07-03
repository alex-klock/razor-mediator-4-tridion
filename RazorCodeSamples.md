# Using Statement Examples #
```
@using Tridion.Extensions.Mediators.Razor.Models
@using Tridion.ContentManager
@using Tridion.ContentManager.CommunicationManagement
@using Tridion.ContentManager.ContentManagement
@using Tridion.ContentManager.ContentManagement.Fields
@using Tridion.ContentManager.Publishing.Rendering
@using Tridion.ContentManager.Templating
@using System.Collections.Generic
@using System.Collections.Text
```

Output JSP/ASP Code:
```
@: <%=System.DateTime.Now().ToString()%>
```
@: tells the .NET Razor Engine to treat it like text