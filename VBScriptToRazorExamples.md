# Introduction #

Some examples to help migration of VBScript code to Razor

# Details #

<h3>Component Text Field</h3>
```
VBScript:
[%=Component.Fields("summary").value(1)%]```
```
Razor:
@Fields.summary```

<h3>Display a Date field:</h3>
```
VBScript:
[%=FormatDateTime(Component.Fields("publishedAt").value(1),2)
```
```
Razor:
@Fields.publishedAt.ToString("M/d/yyyy")
```

<h3>Component Link field:</h3>
```
VBScript:
[%
set ACompLinkField = Component.Fields("ACompLinkField").Value(1)
WriteOut ACompLinkField.Fields("Header").Value(1)
set ACompLinkField = Nothing
%]
```
```
Razor:
@Fields.ACompLinkField.Fields.Header
```

<h3>Loop through Component Link field</h3>
```

VBScript:
<ul>
[%
For each c in Component.Fields("SomeCompLinks").Value
%]
<li>[%=c.Title%] : [%=c.Fields("SubHeader").Value(1)%]

Unknown end tag for &lt;/li&gt;


[%
Next
%]


Unknown end tag for &lt;/ul&gt;


```
```

Razor:
<ul>
@foreach (var c in Field.SomeCompLinks) {
<li>@c.Title : @c.Fields.SubHeader

Unknown end tag for &lt;/li&gt;


}


Unknown end tag for &lt;/ul&gt;


```

<h3>Conditional output</h3>
```
VBScript:
[%
if(Component.Fields("Subheader").Value(1) = "") then
WriteOut Component.Fields("Subheader").Value(1)
else
WriteOut Page.Title
end if
```

```

Razor:
@(Fields.SubHeader ?? Page.Title)
```

<h3>Checking for field exists and empty value</h3>
```

VBScript:
[%
If(Not Component.Fields("Subheader") Is Nothing) then
If(Component.Fields("Subheader").Value(1) != "") then
WriteOut "<h2>" &amp; Component.Fields("Subheader").Value(1) &amp; "

Unknown end tag for &lt;/h2&gt;

"
End If
End If
%]
```
```

Razor:
@if (Fields.SubHeader != null) {
<h2>@Fields.SubHeader

Unknown end tag for &lt;/h2&gt;


}
```

<h3>Variable</h3>
```

VBScript:
[%
dim name : name = Component.Fields("subtitle").value(1)
%]
```
```

Razor:
@{
var name = Fields.subtitle
}
```

<h3>Comments</h3>
```

VBScript:
' This is a comment
```
```

Razor:
@* This is a comment *@
```

<h3>Functions</h3>
```

VBScript:
Function Test(name)
Test = "Hello" & name & "!"
End Function
<div>[%=Test("Administrator")%]

Unknown end tag for &lt;/div&gt;


```

```

Razor:
@functions {  public string Test(string name) {
return "Hello " + name + "!";
} }
<div>@Test("Administrator")

Unknown end tag for &lt;/div&gt;


```

<h3>Multi-value count - they look very familiar</h3>

```

VBScript:
[%
if (Component.Fields("SomeMultiValuedField").Value.Count > 0) then %]
We Have Item! Total: [%=Component.Fields("SomeMultiValuedField").Value.Count
end if
%]
```
```

Razor:
@if (Fields.SomeMultiValuedField.Count > 0) {
We Have Item! Total: @Fields.SomeMultiValuedField.Count
}
```

note: I like the fluent Razor syntax here and saving keystrokes with the @ syntax

<h3>Checking if a field exists when using 1 template with multiple schemas:</h3>
```

VBScript:
[%
if (Not Component.Fields("Subheader") Is Nothing) then
%]
<div>ItemField 'SubHeader' Exists and Has Value of:
[%=Component.Fields("Subheader").Value(1)%]

Unknown end tag for &lt;/div&gt;


[%
end if
%]
```
```

Razor:
@if (Fields.HasField("SubHeader")) {
<div>ItemField 'SubHeader' Exists and Has Value of:
@Fields.SubHeader

Unknown end tag for &lt;/div&gt;


}
```


---


Accessing Page object from Component Template
```

VBScript:
[%
If(IsObject(Page)) then
%]
<h3>[%=Page.MetaDataFields("PageSubHeader").Value(1)%]

Unknown end tag for &lt;/h3&gt;


[%
End If
%]
```
```

Razor:
@if (Page != null) {
<h3>@Page.MetaData.PageSubHeader

Unknown end tag for &lt;/h3&gt;


}
```

<h3>RenderMode</h3>
```

VBScript:
[%
if(RenderMode = "Publish") then
%]
<span>This is a way to render this only when publishing and not previewing.

Unknown end tag for &lt;/span&gt;


[%
end if
%]
```
```

Razor:
@if (RenderMode.Equals("Publish")) {<span>This is a way to render this only when publishing and not previewing.

Unknown end tag for &lt;/span&gt;

}
```

<h3>Extras that are not available in VBScript</h3>

<h3>Logging</h3>
```

@Log.Debug("This is a debug statement")
```

<h3>Get another Component</h3>
```

VBScript:
set otherComponent = tdse.GetObject("tcm:23-233")
WriteOut otherComponent.Fields("SubjectHeader").Value(1)
set otherComponent = nothing
```
```

Razor:
@Models.GetComponent("tcm:23-233").Fields.SubjectHeader
```