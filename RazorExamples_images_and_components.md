# Introduction #

Razor allows the template author to access content in a component using `@Fields`. Rendering images and linked components will involve this.


# Examples #
> ## Images ##

Suppose that a schema has two XML Fields:
  * `Thumbnail` (a required field)
  * `ThumnbailAlt` (for the alt text, not required )


```

  <img class="filmstrip__item__image" src="@Fields.Thumbnail" @(!String.IsNullOrEmpty(ThumbnailAlt) ? "alt=\""+ ThumbnailAlt +" \"" : String.Empty ))/>

```

## Component Links ##
Suppose that a schema has an XML field called `ComponentList`. This XML field is a field which is a repeatable component link to a component which contains four fields:
  * `Thumbnail`
  * `ThumbnailAlt`
  * `Header`
  * `link`

```
<ul class="list"
  @foreach (var compLink in Fields.ComponentList) {
    <li class="list__item  item">
      <a class="list__item__link" href="@compLink.link" title="@StripHtml(compLink.Header)">
        @if(compLink.Thumbnail != null){<img class="item__image" src="@compLink.Thumbnail" @(!String.IsNullOrEmpty(compLink.ThumbnailAlt) ? "alt=\""+ compLink.ThumbnailAlt +" \"" : String.Empty ))/>}
        @if(!String.IsNullOrEmpty(compLink.Header)){<h4 class="item__title">@compLink.Header</h4>}
      </a>
    </li>
  }
</ul>
      

```