# Razor Mediator for Tridion

## Razor Mediator Visual Studio Support ##
Please check out https://github.com/mvlasenko/TridionVSRazorExtension for some awesome Visual Studio support.  Develop your templates in Visual Studio with intellisense support, and even update your templates to Tridion!

*exported from google code on July 3rd, 2015*

The Razor Mediator allows you to use the power and flexibility of Razor Templating within your Tridion Compound Templates.

## Introduction
The Razor Template Engine has been introduced into the .NET community with much success, and you’ll especially find it common in almost every ASP.NET MVC project you come across now.  If you are not familiar with .NET and the Razor Templating syntax, then perhaps you have heard of Velocity?  Razor has been compared to Java’s Velocity template engine.  But why is it so popular?  Razor allows an easy to use syntax (pretty much C# syntax) for template scripting, but with a clean easy to read look and feel (so no, we’re not taking a step back into the days of VBScript templates!).


## The Power of the Razor Mediator
With Razor Templating, all of the above limitations are covered.  You actually have the power of .NET within your Razor 
Templates.  Using the base Tridion Razor Template and utilities that comes with the Razor Mediator, you’ll have quick and 
easy access to most of your Template Package items and fields right at your fingertips.  Because you are now able to do 
more and write less, the templating process can be done quicker than ever before.

Razor is a strongly typed, case sensitive, scripting language.  For example, for a Tridion DateField, you can format the 
date right from the template using 1<span>@Fields.NewsDate.ToString(“MM/dd/yyyy”)</span>1.  For a Tridion KeywordField, 
you have access to all of the Tridion keyword fields like `<span>@Fields.SomeKeywordField.Title</span>`.  Want to capitalize 
your header field? `<span>@Fields.Header.ToUpper()</span>`.  What about accessing fields of Component Links?  
`<div>@Fields.SomeComponentLink.FieldName</div>`.  You can even go as deep as you want with Component Links.  
`<div>@Fields.SomeComponentLink.AnotherComponentLink.FieldName</div>`.  You can also add your own scriptlet like helper 
functions right to your templates if you so desired.

Some of the Tridion types have also been wrapped by Model classes that provide easier functionality of accessing 
certain data, like Fields and Metadata (dynamic access that always returns the correct type in your templates).  
For these Model classes, you can always access the underlying Tridion Object through the TridionObject property.  
For example, `@Component.TridionObject.HasUsingItems()`.  The Component is of type ComponentModel (the special wrapper class),
while the TridionObject property actually returns the Tridion Component instance.  Most of the Model class’ ToString() 
method writes out the wrapped Tridion item’s Title property, with the exception of the ComponentModel, which will write 
out the Component’s Tcm Uri.

## Quick Sample
```
<div class=”@Component.Fields.NewsStyle”>
    <img src=”@Fields.HeaderImage.ID” alt=”@Fields.HeaderImage.AltText” />
    @* Note that Fields is just a shortcut to Component.Fields *@
    <h2>@Fields.Header</h2>
    <h5>@Fields.NewsDate.ToString(“dd/MM/yy hh:mm”)</h5>
    <div class=”body-text”>
        @Fields.BodyText
    </div>
    <ul>
    @* Now we'll loop over a ComponentLink field and grab the component title and a field *@
    @foreach (dynamic linkItem in Fields.RelatedItems) {
        <li>@linkItem.Title - @linkItem.Fields.Summary</li>
    }
    </ul>
</div>
```

## Full Documentation
You can find the current full documentation of the Razor Mediator [here](http://code.google.com/p/razor-mediator-4-tridion/downloads/detail?name=RazorMediatorDocumentation_v1.3.3.docx&can=2&q==).

## Simple Installation
Download the latest v1.3.3 (Released 6/11/13) installer [here](http://code.google.com/p/razor-mediator-4-tridion/downloads/detail?name=RazorMediatorInstaller_v1.3.3.1.msi&can=2&q=) See the [ChangeLog] for details on what has been fixed and updated in this version.

You can find previous version in the Releases section.

## Joining the Project
If you wish to become involved with the Razor Mediator and the direction of its future, feel free to submit pull requests!

A version 1.4 may be in the future with some latest fixes, however effort and direction will be on a complete rewrite in a new repository for Razor Mediator 2.0.

*Why a complete rewrite?*

As many developers know the plague of "Man... if I could do it over I would do XYZ instead.  What was I thinking here?!?", there
are many things that could improve not only performance but how templates are written, and in order to implement those
efficiencies properly, I firmly do believe the project rewritten from scratch is in order.  If 2.0 does ever make it to see
the light of day, I'd also want it to work well with [Alchemy](http://www.alchemywebstore.com) and proper tooling to assist developers.
