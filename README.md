# ExactJSON

A strict and intuitive JSON library for .NET

[![Build and Test](https://github.com/anton-abrm/exact-json/actions/workflows/build-and-tests.yml/badge.svg)](https://github.com/anton-abrm/exact-json/actions/workflows/build-and-tests.yml)

The goal of this project is to reduce any implicit conversions and make behavior more intuitive. 
This library also encourages a more restrictive serialization policy. 
If the serializer serializes an object, it guarantees that the object can be deserialized with the same options.

## Features

* Intuitive and consistent API
* JSON comparison without rounding issues
* Streaming reading and writing of JSON data
* Ability to save and restore the state of JSON reader
* Serialization and deserialization objects as JSON arrays
* Highly customizable output number format
* Preserving number format
* JSON pointer support
* JSON lines support

## Examples

### Difference without rounding
```c#
var node1 = JsonNode.Parse("[1E-30, 0.29999999999999999]");
var node2 = JsonNode.Parse("[1E-30, 0.3]");

var diffs = node1.Diff(node2);

foreach (var diff in diffs) {
    Console.WriteLine($"{ diff.Pointer } | { diff.Self } | { diff.Other }");
}

// Output:
// /1 | 0.29999999999999999 | 0.3
```
### Equality without rounding
```C#
var node1 = JsonNode.Parse("[1E-29, 0.29999999999999999]");
var node2 = JsonNode.Parse("[1E-30, 0.3]");

var nodesEqual = node1.Equals(node2);

Console.WriteLine($"Equal: {nodesEqual}");  

// Output: 
// Equal: False
```
### Number format preserving
```c#
var result = JsonNode.Parse("[1.00, 0.000, 1E+2]").ToString();

Console.WriteLine(result); 

// Output: 
// [1.00,0.000,1E+2]
```

### Customizing number format
```c#
public sealed class Numbers
{
    [JsonNode, JsonFormat("3")] // 010
    public int A { get; set; } = 10; 
    
    [JsonNode, JsonFormat(".2")] // 10.00
    public int B { get; set; } = 10; 
    
    [JsonNode, JsonFormat("E+2")] // 1E+01
    public int C { get; set; } = 10; 
    
    [JsonNode, JsonFormat("3.2E+2")] // 001.00E+01
    public int D { get; set; } = 10; 
}
        
public void Main()
{
    var serializer = new JsonSerializer();

    var json = serializer.Serialize<Numbers>(new Numbers());
    
    Console.WriteLine(json);
    
    // Output:
    // {"A":010,"B":10.00,"C":1E+01,"D":001.00E+01}
}
```

### Saving and restoring reader state
```c#
var reader = new JsonStringReader("[1, 2, 3]");
            
reader.Read();
reader.Read();

Console.WriteLine($"Value: {reader.Value}"); 

// Output: 
// 1

var state = reader.SaveState();

while (reader.Read()) {
}

state.Restore();

Console.WriteLine($"Value: {reader.Value}"); 

// Output: 
// 1

```

### JSON lines support
```c#
using var sw = new StringWriter();
using var jw = new JsonTextWriter(sw);

jw.WriteNumber(1);
jw.WriteNumber(2);
jw.WriteNumber(3);

var result = sw.ToString();

Console.WriteLine(result);

// Output:
// 1
// 2
// 3
```
### Serializing objects as tuples
```c#
[JsonTuple]
public struct Point
{
    [JsonNode]
    public double X { get; set; }
    
    [JsonNode]
    public double Y { get; set; }
}

public void Main()
{
    var point = new Point {
        X = 1,
        Y = 2
    };
    
    var serializer = new JsonSerializer();

    var result = serializer.Serialize<Point>(point);

    Console.WriteLine(result); 
    
    // Output: 
    // [1.0,2.0]
}
```

## License

This project is licensed under the [MIT License](LICENSE).