using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Program
{
    public static void Main()
    {
        Person angela = new Person
        {
            Name = "Angela",
            Title = "Boss"
        };
        Person bob = new Person
        {
            Name = "Bob",
            Title = "Middle Manager"
        };
        Person charles = new Person
        {
            Name = "Charles",
            Title = "Peon 1"
        };
        Person david = new Person
        {
            Name = "David",
            Title = "Peon 2"
        };

        Node<Person> root = new Node<Person>(angela);
        Node<Person> sub = root.Add(bob);
        sub.Add(charles);
        sub.Add(david);

        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new OrgChartConverter() },
            Formatting = Formatting.Indented
        };

        string json = JsonConvert.SerializeObject(root, settings);
        Console.WriteLine(json);
        Console.Read();
    }
}

public class OrgChartConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Node<Person>));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Node<Person> node = (Node<Person>)value;
        JObject obj = new JObject();
        obj.Add("Name", node.Value.Name);
        obj.Add("Subordinates", JArray.FromObject(node.ChildrenLeft, serializer));
        obj.WriteTo(writer);
    }

    public override bool CanRead
    {
        get { return false; }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

public class Person
{
    public string Name { get; set; }
    public string Title { get; set; }
}

public class Node<T> : IEnumerable<Node<T>>
{
    public T Value { get; set; }
    public List<Node<T>> ChildrenLeft { get; private set; }
    public List<Node<T>> ChildrenRight { get; private set; }


    public Node(T value)
    {
        Value = value;
        ChildrenLeft = new List<Node<T>>();
        ChildrenRight = new List<Node<T>>();
    }

    public Node<T> Add(T value)
    {
        var childNode = new Node<T>(value);
        ChildrenLeft.Add(childNode);
        return childNode;
    }

    public IEnumerator<Node<T>> GetEnumerator()
    {
        return ChildrenLeft.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

