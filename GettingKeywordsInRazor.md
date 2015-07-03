# Keywords #

## Get Parent Keyword ##
Before Razor 1.3.1, there was not a direct way to get the parent keyword of a given keyword.
If you're using 1.3 or earlier, you can use this function to get the parent keyword of a given keyword:

```
 @functions{
    public dynamic GetParentKeyword(dynamic kw){
        var parent = kw.TridionObject.ParentKeywords[0];
        return Models.GetKeyword(parent.Id);
    }
}  

```

You can then use this function in the following way
```
<tr>
   <th>Keyword</th><th>Parent Keyword</th>
</tr>
<tr>
 <td>@Fields.someKeyword</td><td>@GetParentKeyword(Fields.someKeyword)</td>
</tr>
```
Not only can you get the Parent Keyword value, but you could also get any content stored in a metadata schema that's attached to this parent keyword:
```
@GetParentKeyword(Fields.someKeyword).someMetadataField
```
## Get Related Keywords ##
This is another one that you don't have before 1.3.1
If you want to get related keywords, you can't do that with by simply doing @SomeKeyword.RelatedKeywords

Here's the function you'll want. Keep in mind that unlike the parent keyword function, this will _not_ return a single value. Instead, it's giving you a list.

```
@functions{
    public List<dynamic> GetRelatedKeyword(dynamic kw){
        var relatives = kw.TridionObject.RelatedKeywords;
        List<dynamic> listOfRelatives = new List<dynamic>();
        
        foreach (var relative in relatives) {
           listOfRelatives.Add(Models.GetKeyword(relative.Id));
        }
        return listOfRelatives;
    }
}
```

Usage would be as follows:
```
<ul>
    @foreach(var relative in GetRelatedKeyword(Fields.Person)){

    <li>@relative</li>
    }
</ul>
```
You can also access associated metadata just like you could with the Parent Keyword function:
```
<ul>
    @foreach(var relative in GetRelatedKeyword(Fields.Person)){

    <li>@relative.First_Name @relative.Last_Name</li>
    }
</ul>
```
## Get the Value of a Keyword ##
As it would turn out, there is no 'value' property in the Keyword class. Even though you see "Value" as a field when creating the Keyword, this is actually the title property. So if you're trying to get the "Value", your best bet is to try this:

```
@someKeywordField.Title
```

If you want something that'll be more semantic, you can try this out:
```
@functions{
    public dynamic GetKeywordValue(dynamic kw){
        var Value = kw.TridionObject.Title;
        return Value;
    }
} 
```

And use this like so:
```
@getKeywordValue(Fields.SomeKeywordField)
```