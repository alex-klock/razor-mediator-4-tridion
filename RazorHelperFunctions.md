# Introduction #

Here are some helper functions that can be used interchangeably on most projects.

# Output the TCM ID of a given component #

## Helper Function ##
```
@{
    var thisTCM = Component.ID.ToString().Replace(":", "-");
}
@helper getTCM(dynamic Item){
	var itemTCM = Item.ID.ToString().Replace(":","-");
	@itemTCM
}

```
## Use in TBB ##
This works great with the HTML5 "data" attribute, where you just write `data-`[attribute](custom.md). Even though it's an HTML5 attribute, it works just fine in older browsers.

```
<div class="internalContent clearfix" data-tcm="@thisTCM"> </div>
```

If you want to output the TCM of a linked component, you can use this as such
```
	<div class="calloutItem" data-tcm="@getTCM(Fields.ComponentLink)"></div>
```